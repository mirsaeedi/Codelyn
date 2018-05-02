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
    internal class DetectStaticDbContextFieldAnalyzer : SyntaxNodeAnalyzer<FieldDeclarationSyntax>
    {
        private SemanticModel _semanticModel { get; }
        public DetectStaticDbContextFieldAnalyzer(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
        }

        internal override IEnumerable<RuleViolation> AnalyzeSyntaxNode(FieldDeclarationSyntax node)
        {
            var identifierNameSyntax = node.DescendantNodes()
                .SingleOrDefault(q => q.Kind() == SyntaxKind.VariableDeclarator);

            var symbol = _semanticModel
                         .GetDeclaredSymbol(identifierNameSyntax) as IFieldSymbol;

            if (symbol.IsStatic && symbol.Type?.BaseType?.Name == "DbContext")
            {
                yield return new RuleViolation(RuleName);
            }
        }
    }
}
