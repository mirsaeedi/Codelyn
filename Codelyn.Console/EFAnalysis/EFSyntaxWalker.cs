using Codelyn.Console.EFAnalysis;
using Codelyn.Console.TryCatchFinallyAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codelyn.Console
{
    public class EFSyntaxWalker : CSharpSyntaxWalker
    {
        private readonly EFAnalyzer _efAnalyzer;

        public EFSyntaxWalker(EFAnalyzer efAnalyzer)
        {
            _efAnalyzer = efAnalyzer;
        }

        public override void VisitForStatement(ForStatementSyntax node)
        {
            var blockSyntax = node.Statement as BlockSyntax;

            AnalyzeLoop(blockSyntax);

            base.VisitForStatement(node);
        }

        private void AnalyzeLoop(BlockSyntax blockSyntax)
        {
            var context = new AnalyzeContext(_efAnalyzer.Project,
               _efAnalyzer.Document);
            _efAnalyzer.AnalyzedContexts.Add(context);

            var methodInvocations = blockSyntax.DescendantNodes()
                .Where(q => q.Kind() == SyntaxKind.InvocationExpression)
                .OfType<InvocationExpressionSyntax>()
                .ToArray();

            foreach (var methodInvocation in methodInvocations)
            {
                var detectAddRemoveInsideLoopAnalyzer = new DetectAddRemoveInsideLoopAnalyzer(_efAnalyzer.DocumentSemanticModel);
                context.RuleViolations.AddRange(detectAddRemoveInsideLoopAnalyzer.AnalyzeSyntaxNode(methodInvocation));

                var detectSaveInsideLoopAnalyzer = new DetectSaveInsideLoopAnalyzer(_efAnalyzer.DocumentSemanticModel);
                context.RuleViolations.AddRange(detectSaveInsideLoopAnalyzer.AnalyzeSyntaxNode(methodInvocation));
            }

            var objectCreations = blockSyntax.DescendantNodes()
                .Where(q => q.Kind() == SyntaxKind.ObjectCreationExpression)
                .OfType<ObjectCreationExpressionSyntax>()
                .ToArray();

            foreach (var objectCreation in objectCreations)
            {
                var detectDbContextCreationInsideLoopAnalyzer = new DetectDbContextCreationInsideLoopAnalyzer(_efAnalyzer.DocumentSemanticModel);
                context.RuleViolations.AddRange(detectDbContextCreationInsideLoopAnalyzer.AnalyzeSyntaxNode(objectCreation));
            }
        }

        public override void VisitWhileStatement(WhileStatementSyntax node)
        {
            var blockSyntax = node.Statement as BlockSyntax;

            AnalyzeLoop(blockSyntax);
            base.VisitWhileStatement(node);
        }

        public override void VisitForEachStatement(ForEachStatementSyntax node)
        {
            var blockSyntax = node.Statement as BlockSyntax;

            AnalyzeLoop(blockSyntax);

            base.VisitForEachStatement(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var context = new AnalyzeContext(_efAnalyzer.Project,
                _efAnalyzer.Document);
            _efAnalyzer.AnalyzedContexts.Add(context);

            var detectDbContextUsageInsideMvcControllerAnalyzer = 
                new DetectDbContextUsageInsideMvcControllerAnalyzer(_efAnalyzer.DocumentSemanticModel);

            context.RuleViolations.AddRange(detectDbContextUsageInsideMvcControllerAnalyzer.AnalyzeSyntaxNode(node));

            var methodInvocations = node.DescendantNodes()
                .Where(n => n.Kind() == SyntaxKind.InvocationExpression)
                .OfType<InvocationExpressionSyntax>()
                .ToArray();

            foreach (var methodInvocation in methodInvocations)
            {
                var detectSyncronousMethodCalls = new DetectSyncronousMethodCalls(_efAnalyzer
                    .DocumentSemanticModel);
                context.RuleViolations.AddRange(detectSyncronousMethodCalls.AnalyzeSyntaxNode(methodInvocation));
            }

            var objectCreations = node.DescendantNodes()
                .Where(q => q.Kind() == SyntaxKind.ObjectCreationExpression)
                .OfType<ObjectCreationExpressionSyntax>()
                .ToArray();

            foreach (var objectCreation in objectCreations)
            {
                var detectDbContextCreationWithoutUsingAnalyzer = 
                    new DetectDbContextCreationWithoutUsingAnalyzer(_efAnalyzer.DocumentSemanticModel);

                context.RuleViolations.AddRange(detectDbContextCreationWithoutUsingAnalyzer.AnalyzeSyntaxNode(objectCreation));
            }

            base.VisitMethodDeclaration(node);
        }


        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var context = new AnalyzeContext(_efAnalyzer.Project,
                _efAnalyzer.Document);
            _efAnalyzer.AnalyzedContexts.Add(context);

            var fieldDeclarationSyntaxes = node.DescendantNodes()
              .Where(n => n.Kind() == SyntaxKind.FieldDeclaration)
              .OfType<FieldDeclarationSyntax>()
              .ToArray();

            foreach (var fieldDeclarationSyntax in fieldDeclarationSyntaxes)
            {
                var detectDbContextInitializationAsClassFieldAnalyzer =
                    new DetectDbContextInitializationAsClassFieldAnalyzer(_efAnalyzer.DocumentSemanticModel);
                context.RuleViolations.AddRange(detectDbContextInitializationAsClassFieldAnalyzer.AnalyzeSyntaxNode(fieldDeclarationSyntax));

                var detectStaticDbContextFieldAnalyzer =
                    new DetectStaticDbContextFieldAnalyzer(_efAnalyzer.DocumentSemanticModel);
                context.RuleViolations.AddRange(detectStaticDbContextFieldAnalyzer.AnalyzeSyntaxNode(fieldDeclarationSyntax));
        }

            base.VisitClassDeclaration(node);
        }

    }
}
