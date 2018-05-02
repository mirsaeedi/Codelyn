using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codelyn.Console.EFAnalysis
{
    internal class DetectAddRemoveInsideLoopAnalyzer: SyntaxNodeAnalyzer<InvocationExpressionSyntax>
    {
        private SemanticModel _semanticModel { get; }
        public DetectAddRemoveInsideLoopAnalyzer(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
        }

        internal override IEnumerable<RuleViolation> AnalyzeSyntaxNode(InvocationExpressionSyntax node)
        {
            var invokedSymbol = _semanticModel
                .GetSymbolInfo(node).Symbol;

            if (invokedSymbol == null)
                yield break;

            if (invokedSymbol.Name == "Add" && invokedSymbol.ContainingSymbol.Name == "DbSet")
            {
                yield return new RuleViolation(RuleName);
            }
            if (invokedSymbol.Name == "Remove" && invokedSymbol.ContainingSymbol.Name == "DbSet")
            {
                yield return new RuleViolation(RuleName);
            }
        }
    }
}
