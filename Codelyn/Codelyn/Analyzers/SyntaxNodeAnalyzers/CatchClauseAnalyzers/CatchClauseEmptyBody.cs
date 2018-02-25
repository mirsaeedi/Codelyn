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
        protected override string Category => "Clean Code";

        protected override LocalizableString Title => "Exception Swallowing - Empty Catch";

        protected override LocalizableString MessageFormat => "It's recommended to act appropriately to recover from occured exceptions instead of ignoring them";

        protected override LocalizableString Description => "It's recommended to act appropriately to recover from occured exceptions instead of ignoring them";

        internal override void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var catchClause = context.Node as CatchClauseSyntax;

            if (catchClause.Block.Statements.Count == 0)
                ReportDiagnostic(context, catchClause.CatchKeyword.GetLocation());
        }

        internal override void AnalyzeSyntaxNode(CompilationStartAnalysisContext context, SyntaxNode catchClauseSyntaxNode)
        {
           
        }
    }
}
