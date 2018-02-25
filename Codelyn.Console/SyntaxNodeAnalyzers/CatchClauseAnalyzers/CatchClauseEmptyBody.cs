using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Codelyn
{
    internal class CatchClauseEmptyBody : SyntaxNodeAnalyzer
    {
        internal override IEnumerable<RuleViolation> AnalyzeSyntaxNode(CatchClauseSyntax catchClauseSyntax)
        {
            if (catchClauseSyntax.Block.Statements.Count == 0)
            {
                yield return new RuleViolation
                    (
                    RuleName
                    );
            }
        }
    }
}
