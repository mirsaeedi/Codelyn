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
    internal class DetectDbContextUsageInsideMvcControllerAnalyzer : SyntaxNodeAnalyzer<MethodDeclarationSyntax>
    {
        private SemanticModel _semanticModel { get; }
        public DetectDbContextUsageInsideMvcControllerAnalyzer(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
        }

        internal override IEnumerable<RuleViolation> AnalyzeSyntaxNode(MethodDeclarationSyntax node)
        {
            var methodSymbol = _semanticModel.GetDeclaredSymbol(node);

            if (methodSymbol.ContainingType.BaseType == null) // for interfaces
                yield break;

            if (methodSymbol.ContainingType?.BaseType.Name != "Controller" && 
                !methodSymbol.ContainingType.Name.EndsWith("Controller"))
                yield break;

            var methodInvocations = node.DescendantNodes()
                .Where(n => n.Kind() == SyntaxKind.IdentifierName)
                .OfType<IdentifierNameSyntax>()
                .ToArray();

            foreach (var methodInvocation in methodInvocations)
            {
                var s = _semanticModel
                            .GetTypeInfo(methodInvocation);

                if (s.Type?.Name == "DbSet" ||
                    s.Type?.Name == "Queryable" ||
                    s.Type?.BaseType?.Name == "DbContext")
                {
                    yield return new RuleViolation(RuleName);
                }
            }
        }
    }
}
