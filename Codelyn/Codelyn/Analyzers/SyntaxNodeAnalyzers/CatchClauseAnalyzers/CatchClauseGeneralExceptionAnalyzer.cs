using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Codelyn
{
    internal class CatchClauseGeneralExceptionAnalyzer : SyntaxNodeAnalyzer
    {
        protected override string Category => "Clean Code";

        protected override LocalizableString Title => "General Catch";

        protected override LocalizableString MessageFormat => "It's recommended to use specific catches instead of using Exception to catch all errors";

        protected override LocalizableString Description => "It's recommended to use specific catches instead of using Exception to catch all errors";

        internal override void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var catchClause = context.Node as CatchClauseSyntax;

            var siblingCatchClauses = catchClause.Parent.ChildNodes()
                .OfType<CatchClauseSyntax>();

            var lastCatchClause = catchClause.Parent.ChildNodes()
                .OfType<CatchClauseSyntax>()
                .Last();

            foreach (var siblingCatchClause in siblingCatchClauses)
            {
                if (catchClause.Declaration?.Type.ToString() == "Exception"
                    && catchClause != lastCatchClause)
                    ReportDiagnostic(context, catchClause.CatchKeyword.GetLocation());
            }

            if (catchClause.Declaration?.Type.ToString() == "Exception")
                ReportDiagnostic(context, catchClause.CatchKeyword.GetLocation());

        }

        internal override void AnalyzeSyntaxNode(CompilationStartAnalysisContext context, SyntaxNode catchClauseSyntaxNode)
        {
           
        }
    }
}
