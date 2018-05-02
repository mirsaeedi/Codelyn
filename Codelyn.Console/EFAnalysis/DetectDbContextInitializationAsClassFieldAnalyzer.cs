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
    internal class DetectDbContextInitializationAsClassFieldAnalyzer : SyntaxNodeAnalyzer<FieldDeclarationSyntax>
    {
        private SemanticModel _semanticModel { get; }
        public DetectDbContextInitializationAsClassFieldAnalyzer(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
        }

        internal override IEnumerable<RuleViolation> AnalyzeSyntaxNode(FieldDeclarationSyntax node)
        {
            var objectCreation = node.DescendantNodes()
                    .SingleOrDefault(q => q.Kind() == SyntaxKind.ObjectCreationExpression);

            if (objectCreation == null)
                yield break;

            var symbol = _semanticModel
                         .GetSymbolInfo(objectCreation)
                         .Symbol as IMethodSymbol;

            if (symbol!=null && symbol.ReceiverType.BaseType.Name == "DbContext")
            {
                yield return new RuleViolation(RuleName);
            }

        }
    }
}
