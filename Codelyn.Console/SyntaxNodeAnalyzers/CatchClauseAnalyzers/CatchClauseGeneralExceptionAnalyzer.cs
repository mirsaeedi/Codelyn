using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Codelyn
{
    internal class CatchClauseGeneralExceptionAnalyzer : SyntaxNodeAnalyzer<CatchClauseSyntax>
    {
        internal override IEnumerable<RuleViolation> AnalyzeSyntaxNode(CatchClauseSyntax catchClauseSyntax)
        {
            var siblingCatchClauses = catchClauseSyntax.Parent.ChildNodes()
                .OfType<CatchClauseSyntax>();

            var lastCatchClause = catchClauseSyntax.Parent.ChildNodes()
                .OfType<CatchClauseSyntax>()
                .Last();

            foreach (var siblingCatchClause in siblingCatchClauses)
            {
                if (catchClauseSyntax.Declaration?.Type.ToString() == "Exception"
                    && catchClauseSyntax != lastCatchClause)
                {
                    yield return new RuleViolation
                        (
                        RuleName
                        );
                }
            }

            if (catchClauseSyntax.Declaration?.Type.ToString() == "Exception")
            {
                yield return new RuleViolation
                    (
                    RuleName
                    );
            }
        }
    }
}
