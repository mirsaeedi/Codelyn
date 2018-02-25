using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Codelyn
{
    internal class CatchClauseNoDeclaration : SyntaxNodeAnalyzer
    {
        internal override IEnumerable<RuleViolation> AnalyzeSyntaxNode(CatchClauseSyntax catchClauseSyntax)
        {
            if (catchClauseSyntax.Declaration is null)
            {
                yield return (new RuleViolation
                    (
                    RuleName
                    ));

            }

            yield break;
        }
    }
}
