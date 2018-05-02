using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Codelyn
{
    internal class CatchClauseComments : SyntaxNodeAnalyzer<CatchClauseSyntax>
    {
        internal override IEnumerable<RuleViolation> AnalyzeSyntaxNode(CatchClauseSyntax catchClauseSyntax)
        {
            return CheckSingleLineComments(catchClauseSyntax)
                .Concat(CheckMultiLineComments(catchClauseSyntax));
        }

        private IEnumerable<RuleViolation> CheckMultiLineComments(CatchClauseSyntax catchClauseSyntax)
        {
            var multiLineComments = catchClauseSyntax.Block
                .DescendantTrivia()
                .Where(t => t.Kind() == SyntaxKind.MultiLineCommentTrivia)
                .ToArray();

            foreach (var comment in multiLineComments)
            {
                if (comment.ToString().ToLower().Contains("todo")
                    || comment.ToString().ToLower().Contains("to do")
                    || comment.ToString().ToLower().Contains("fixme")
                    || comment.ToString().ToLower().Contains("fix me"))
                {
                    yield return new RuleViolation
                        (
                        RuleName
                        );
                }
                    
            }
        }

        private IEnumerable<RuleViolation> CheckSingleLineComments(CatchClauseSyntax catchClauseSyntax)
        {
            var singleLineComments = catchClauseSyntax.Block
                .DescendantTrivia()
                .Where(t => t.Kind() == SyntaxKind.SingleLineCommentTrivia)
                .ToArray();

            foreach (var comment in singleLineComments)
            {
                if (comment.ToString().ToLower().Contains("todo")
                    || comment.ToString().ToLower().Contains("to do")
                    || comment.ToString().ToLower().Contains("fixme")
                    || comment.ToString().ToLower().Contains("fix me"))
                {
                    yield return new RuleViolation
                    (
                    RuleName
                    );
                }
            }
        }
    }
}
