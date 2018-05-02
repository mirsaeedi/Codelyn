using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codelyn.Console.EFAnalysis
{
    internal class DetectDbContextCreationWithoutUsingAnalyzer : SyntaxNodeAnalyzer<ObjectCreationExpressionSyntax>
    {
        private SemanticModel _semanticModel { get; }
        public DetectDbContextCreationWithoutUsingAnalyzer(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
        }

        internal override IEnumerable<RuleViolation> AnalyzeSyntaxNode(ObjectCreationExpressionSyntax node)
        {
            var invokedSymbol = _semanticModel
                             .GetTypeInfo(node);

            if (invokedSymbol.Type?.BaseType?.Name == "DbContext")
            {
                if (node
                    ?.Parent
                    ?.Parent
                    ?.Parent
                    ?.Parent.Kind() != SyntaxKind.UsingStatement)
                {
                    yield return new RuleViolation(RuleName);
                }
            }

        }
    }
}
