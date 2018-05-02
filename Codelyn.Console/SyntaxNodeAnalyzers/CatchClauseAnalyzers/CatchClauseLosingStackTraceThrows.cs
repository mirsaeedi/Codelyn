using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Codelyn
{
    internal class CatchClauseLosingStackTraceThrows : SyntaxNodeAnalyzer<CatchClauseSyntax>
    {
        internal override IEnumerable<RuleViolation> AnalyzeSyntaxNode(CatchClauseSyntax catchClauseSyntax)
        {
            var throwStatements = catchClauseSyntax.Block.Statements.OfType<ThrowStatementSyntax>();

            foreach (var throwStatement in throwStatements)
            {
                if (throwStatement.Expression != null)
                {
                    yield return new RuleViolation
                        (
                        RuleName
                        );
                }
            }

            yield break;
        }
    }
}
