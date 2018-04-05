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

            var methodInvocations = blockSyntax.DescendantNodes()
                .Where(q => q.Kind() == SyntaxKind.SimpleMemberAccessExpression)
                .ToArray();

            foreach (var methodInvocation in methodInvocations)
            {
                var invokedSymbol = _efAnalyzer.DocumentSemanticModel.GetSymbolInfo(methodInvocations[0]).Symbol;
                if(invokedSymbol.Name=="Add" && invokedSymbol.ContainingSymbol.Name == "DbSet")
                {

                }
                if (invokedSymbol.Name == "Remove" && invokedSymbol.ContainingSymbol.Name == "DbSet")
                {

                }
            }

            base.VisitForStatement(node);
        }

        public override void VisitWhileStatement(WhileStatementSyntax node)
        {
            base.VisitWhileStatement(node);
        }

        public override void VisitForEachStatement(ForEachStatementSyntax node)
        {
            
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            base.VisitMethodDeclaration(node);
        }
        
    }
}
