using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codelyn.Console.EFAnalysis
{
    internal class DetectDbContextCreationInsideLoopAnalyzer : SyntaxNodeAnalyzer<ObjectCreationExpressionSyntax>
    {
        private SemanticModel _semanticModel { get; }
        public DetectDbContextCreationInsideLoopAnalyzer(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
        }

        internal override IEnumerable<RuleViolation> AnalyzeSyntaxNode(ObjectCreationExpressionSyntax node)
        {
            var invokedSymbol = _semanticModel
                .GetTypeInfo(node);

            if (invokedSymbol.Type?.BaseType?.Name == "DbContext")
            {
                yield return new RuleViolation(RuleName);
            }
            
        }
    }
}
