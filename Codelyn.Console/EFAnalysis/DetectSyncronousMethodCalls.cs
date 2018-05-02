using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codelyn.Console.EFAnalysis
{
    internal class DetectSyncronousMethodCalls : SyntaxNodeAnalyzer<InvocationExpressionSyntax>
    {
        private string[] _methodNames =
        {
            "ToArray","ToList","ToDictionary","SaveChanges","Single",
            "SingleOrDefault","Min","Max","All","Any","Average","Contains","Count",
            "First","FirstOrDefault","Sum","Load"
        };

        private SemanticModel _semanticModel { get; }
        public DetectSyncronousMethodCalls(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
        }

        internal override IEnumerable<RuleViolation> AnalyzeSyntaxNode(InvocationExpressionSyntax node)
        {
            var invokedSymbol = _semanticModel
                    .GetSymbolInfo(node).Symbol;

            if (invokedSymbol == null)
                yield break;

            if (_methodNames.Any(m=>m== invokedSymbol.Name))
            {
                dynamic expressionSyntax = 
                    node.Expression as MemberAccessExpressionSyntax;

                while (expressionSyntax != null)
                {

                    if (expressionSyntax is IdentifierNameSyntax)
                    {
                        var symbol = _semanticModel
                            .GetTypeInfo(expressionSyntax as IdentifierNameSyntax);

                        if (symbol.Type?.BaseType?.Name == "DbContext" ||
                            symbol.Type.Name == "IQueryable")
                        {
                            yield return new RuleViolation(RuleName);
                        }

                        break;
                    }

                    if(!(expressionSyntax.Expression is MemberAccessExpressionSyntax
                        || expressionSyntax.Expression is InvocationExpressionSyntax
                        || expressionSyntax.Expression is IdentifierNameSyntax))
                    {
                        yield break;
                    }

                    if (expressionSyntax.Expression is MemberAccessExpressionSyntax)
                    {
                        expressionSyntax =
                              expressionSyntax.Expression as MemberAccessExpressionSyntax;

                        var symbol = _semanticModel
                        .GetSymbolInfo(expressionSyntax as MemberAccessExpressionSyntax)
                        .Symbol;

                        if (symbol.ContainingType.BaseType.Name == "DbContext" ||
                            symbol.ContainingType.Name == "Queryable")
                        {
                            yield return new RuleViolation(RuleName);
                            break;
                        }

                    }
                    else if (expressionSyntax.Expression is InvocationExpressionSyntax)
                    {
                        expressionSyntax =
                              expressionSyntax.Expression as InvocationExpressionSyntax;

                        var symbol = _semanticModel
                        .GetSymbolInfo(expressionSyntax as InvocationExpressionSyntax)
                        .Symbol;

                        if (symbol.ContainingType.BaseType.Name == "DbContext" ||
                            symbol.ContainingType.Name == "Queryable")
                        {
                            yield return new RuleViolation(RuleName);
                            break;
                        }
                    }
                    else if (expressionSyntax.Expression is IdentifierNameSyntax)
                    {
                        expressionSyntax =
                              expressionSyntax.Expression as IdentifierNameSyntax;
                    }
                }
            }
        }
    }
}
