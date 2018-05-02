using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codelyn.Console.TryCatchFinallyAnalysis
{
    public class TryAnalyzer
    {
        public List<RuleViolation> RuleViolations { get; set; } = new List<RuleViolation>();
        public List<AnalyzeContext> AnalyzedContexts { get; set; } = new List<AnalyzeContext>();

        public Project Project { get; }
        public Document Document { get; }
        public SemanticModel DocumentSemanticModel { get; }
        public TryAnalyzer(Project project,
            Document document)
        {
            Project = project;
            Document = document;
            DocumentSemanticModel = document.GetSemanticModelAsync().Result;
        }
        public AnalyzeContext AnalyzeTryStatements(TryStatementSyntax tryStatementSyntax)
        {
            var tryContext = new AnalyzeContext(Project, Document);
            
            var tryStatement = new TryStatement();
            tryContext.Node = tryStatementSyntax;
            tryContext.TryStatement = tryStatement;

            var (@class, declaration) = GetPlaceOfTryStatement(tryStatementSyntax, tryContext);
            tryContext.Class = @class;
            tryContext.Declaration = declaration;

            tryStatement.CatchClauses = AnalyzeCatchClauses(tryStatementSyntax, tryContext);
            tryStatement.FinallyClause = AnalyzeFinallyClause(tryStatementSyntax, tryContext);

            RuleViolations.AddRange(tryContext.TryStatement.CatchClauses.SelectMany(c => c.RuleViolations));
            AnalyzedContexts.Add(tryContext);

            return tryContext;
        }

        private (ISymbol @class, ISymbol declaration) GetPlaceOfTryStatement(TryStatementSyntax tryStatementSyntax, AnalyzeContext context)
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

            return (@class: classSymbol, declaration: methodSymbol);
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
                var catchClause = AnalyzeCatchClause(tryStatementSyntax, catchClauseSyntax, context);
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


            catchClause.RuleViolations = new CatchClauseComments().AnalyzeSyntaxNode(catchClauseSyntax)
                .Concat(new CatchClauseEmptyBody().AnalyzeSyntaxNode(catchClauseSyntax))
                .Concat(new CatchClauseGeneralExceptionAnalyzer().AnalyzeSyntaxNode(catchClauseSyntax))
                .Concat(new CatchClauseLosingStackTraceThrows().AnalyzeSyntaxNode(catchClauseSyntax))
                .Concat(new CatchClauseNoDeclaration().AnalyzeSyntaxNode(catchClauseSyntax));

            return catchClause;
        }
    }
}
