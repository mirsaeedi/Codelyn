using Codelyn.Console;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Codelyn
{
    internal class AnalyzerManager
    {
        private string _solutionPath
        public List<RuleViolation> RuleViolations { get; set; } = new List<RuleViolation>();
        public List<AnalyzeContext> AnalyzedContexts { get; set; } = new List<AnalyzeContext>();

        public AnalyzerManager(string solutionPath)
        {
            _solutionPath = solutionPath;
        }

        internal List<AnalyzeContext> Analyze()
        {
            var msWorkspace = MSBuildWorkspace.Create();

            var solution = msWorkspace.OpenSolutionAsync(_solutionPath).Result;

            foreach (var project in solution.Projects)
            {
                AnalyzeProject(project);
            }

            return AnalyzedContexts;
        }

        private void AnalyzeProject(Project project)
        {
            foreach (var document in project.Documents)
            {
                AnalyzeDocument(project, document);
            }
        }

        private void AnalyzeDocument(Project project, Document document)
        {

            var tree = document.GetSyntaxTreeAsync().Result;

            if (tree.GetRoot().Language != "C#")
                return;

            var analyzeContext = new AnalyzeContext(project, document);
            var walker = new TryStatementSyntaxWalker(analyzeContext, AnalyzeTryStatements);
            walker.Visit((CompilationUnitSyntax)tree.GetRoot());
        }

        private void AnalyzeTryStatements(TryStatementSyntax tryStatementSyntax, AnalyzeContext context)
        {

            var tryContext = new AnalyzeContext(context.Project,context.Document);
            AnalyzedContexts.Add(tryContext);
            var tryStatement = new TryStatement();
            tryContext.TryStatementSyntaxNode = tryStatementSyntax;
            tryContext.TryStatement = tryStatement;

            var (@class, declaration) = GetPlaceOfTryStatement(tryStatementSyntax, tryContext);
            tryContext.Class = @class;
            tryContext.Declaration = declaration;

            tryStatement.CatchClauses = AnalyzeCatchClauses(tryStatementSyntax, tryContext);
            tryStatement.FinallyClause = AnalyzeFinallyClause(tryStatementSyntax, tryContext);
            RuleViolations.AddRange(tryContext.TryStatement.CatchClauses.SelectMany(c=>c.RuleViolations));

        }

        private (ISymbol @class,ISymbol declaration) GetPlaceOfTryStatement(TryStatementSyntax tryStatementSyntax,AnalyzeContext context)
        {   
            SyntaxNode methodDeclarationSyntaxNode = null;

            var parent = tryStatementSyntax.Parent;
            do
            {
                if (parent.IsKind(SyntaxKind.MethodDeclaration))
                {
                    methodDeclarationSyntaxNode = parent as MethodDeclarationSyntax;
                }

                if (parent.IsKind(SyntaxKind.ConstructorDeclaration))
                {
                    methodDeclarationSyntaxNode = parent as ConstructorDeclarationSyntax;
                }

                if (parent.IsKind(SyntaxKind.DestructorDeclaration))
                {
                    methodDeclarationSyntaxNode = parent as DestructorDeclarationSyntax;
                }

                if (parent.IsKind(SyntaxKind.PropertyDeclaration))
                {
                    methodDeclarationSyntaxNode = parent as PropertyDeclarationSyntax;
                }

                if (parent.IsKind(SyntaxKind.FieldDeclaration))
                {
                    methodDeclarationSyntaxNode = parent as FieldDeclarationSyntax;
                }

                parent = parent.Parent;

            } while (methodDeclarationSyntaxNode is null);

            var methodSymbol = context.DocumentSemanticModel
                .GetDeclaredSymbol(methodDeclarationSyntaxNode);

            SyntaxNode classDeclarationSyntaxNode = null;
            
            parent = tryStatementSyntax.Parent;
            do
            {

                if (parent.IsKind(SyntaxKind.ClassDeclaration))
                {
                    classDeclarationSyntaxNode = parent as ClassDeclarationSyntax;
                }

                if (parent.IsKind(SyntaxKind.StructDeclaration))
                {
                    classDeclarationSyntaxNode = parent as StructDeclarationSyntax;
                }

                parent = parent.Parent;
            } while (classDeclarationSyntaxNode is null);

            var classSymbol = context.DocumentSemanticModel
                .GetDeclaredSymbol(classDeclarationSyntaxNode);

            return (@class:classSymbol, declaration: methodSymbol);
        }

        private FinallyClause AnalyzeFinallyClause(TryStatementSyntax tryStatementSyntax, AnalyzeContext context)
        {
            if (tryStatementSyntax.Finally == null)
                return null;

            var finallyClause = new FinallyClause();
            finallyClause.StatementsCount = tryStatementSyntax.Finally.Block.Statements.Count;

            return finallyClause;
            
        }

        private CatchClause[] AnalyzeCatchClauses(TryStatementSyntax tryStatementSyntax, AnalyzeContext context)
        {
            var catchClauses = new List<CatchClause>();

            foreach (var catchClauseSyntax in tryStatementSyntax.Catches)
            {
                var catchClause = AnalyzeCatchClause(tryStatementSyntax,catchClauseSyntax,context);
                catchClauses.Add(catchClause);
            }

            return catchClauses.ToArray();
        }

        private CatchClause AnalyzeCatchClause(TryStatementSyntax tryStatementSyntax, CatchClauseSyntax catchClauseSyntax, AnalyzeContext context)
        {
            var catchClause = new CatchClause();
            catchClause.HasIdentifier = catchClauseSyntax.Declaration?.Identifier.Kind() != SyntaxKind.None;
            if (catchClauseSyntax.Declaration?.Type != null)
            {
                catchClause.ExceptionType = context.DocumentSemanticModel
                .GetTypeInfo(catchClauseSyntax.Declaration.Type)
                .Type
                ?.Name;
            }

            catchClause.StatementsCount = catchClauseSyntax.Block.Statements.Count;

            catchClause.Comments = catchClauseSyntax
                .DescendantTrivia()
                .Where(s => s.Kind() == SyntaxKind.SingleLineCommentTrivia
                || s.Kind() == SyntaxKind.MultiLineCommentTrivia)
                .OfType<SyntaxTrivia>()
                .Select(s => s.ToString())
                .ToArray();

            var invocationExpressions = catchClauseSyntax.Block?.DescendantNodes()
                .OfType<InvocationExpressionSyntax>();

            var identifiers = invocationExpressions
                .SelectMany(n => n.DescendantNodes().OfType<IdentifierNameSyntax>());

            catchClause.ContainsLog = identifiers.Any(n => n.Identifier.ValueText.ToLower().Contains("log")
            || n.Identifier.ValueText.ToLower().Contains("WriteLine"));


            catchClause.RuleViolations =  new CatchClauseComments().AnalyzeSyntaxNode(catchClauseSyntax)
                .Concat(new CatchClauseEmptyBody().AnalyzeSyntaxNode(catchClauseSyntax))
                .Concat(new CatchClauseGeneralExceptionAnalyzer().AnalyzeSyntaxNode(catchClauseSyntax))
                .Concat(new CatchClauseLosingStackTraceThrows().AnalyzeSyntaxNode(catchClauseSyntax))
                .Concat(new CatchClauseNoDeclaration().AnalyzeSyntaxNode(catchClauseSyntax));

            return catchClause;
        }
    }
}