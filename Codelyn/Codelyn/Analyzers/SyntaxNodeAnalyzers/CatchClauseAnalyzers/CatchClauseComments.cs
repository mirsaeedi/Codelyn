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
    internal class CatchClauseComments : SyntaxNodeAnalyzer
    {
        protected override string Category => "Clean Code";

        protected override LocalizableString Title => "Any Undone Task?";

        protected override LocalizableString MessageFormat => "The comment shows that the catch has not implemented completely.";

        protected override LocalizableString Description => "The comment shows that the catch has not implemented completely.";

        internal override void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            CheckSingleLineComments(context);

            CheckMultiLineComments(context);
        }

        internal override void AnalyzeSyntaxNode(CompilationStartAnalysisContext context, SyntaxNode syntaxNode)
        {
            var catchClause = syntaxNode as CatchClauseSyntax;

            var singleLineComments = catchClause.Block
                .DescendantTrivia()
                .Where(t => t.Kind() == SyntaxKind.SingleLineCommentTrivia)
                .ToArray();

            foreach (var comment in singleLineComments)
            {
                if (comment.ToString().ToLower().Contains("todo")
                    || comment.ToString().ToLower().Contains("to do")
                    || comment.ToString().ToLower().Contains("fixme")
                    || comment.ToString().ToLower().Contains("fix me"))
                    ReportDiagnostic(context, comment.GetLocation());
            }
        }

        private void CheckMultiLineComments(SyntaxNodeAnalysisContext context)
        {
            var catchClause = context.Node as CatchClauseSyntax;

            var multiLineComments = catchClause.Block
                .DescendantTrivia()
                .Where(t => t.Kind() == SyntaxKind.MultiLineCommentTrivia)
                .ToArray();

            foreach (var comment in multiLineComments)
            {
                if (comment.ToString().ToLower().Contains("todo")
                    || comment.ToString().ToLower().Contains("to do")
                    || comment.ToString().ToLower().Contains("fixme")
                    || comment.ToString().ToLower().Contains("fix me"))
                    ReportDiagnostic(context,comment.GetLocation());
            }
        }

        private void CheckSingleLineComments(SyntaxNodeAnalysisContext context)
        {
            var catchClause = context.Node as CatchClauseSyntax;

            var singleLineComments = catchClause.Block
                .DescendantTrivia()
                .Where(t => t.Kind() == SyntaxKind.SingleLineCommentTrivia)
                .ToArray();

            foreach (var comment in singleLineComments)
            {
                if (comment.ToString().ToLower().Contains("todo")
                    || comment.ToString().ToLower().Contains("to do")
                    || comment.ToString().ToLower().Contains("fixme")
                    || comment.ToString().ToLower().Contains("fix me"))
                    ReportDiagnostic(context,comment.GetLocation());
            }
        }
    }
}
