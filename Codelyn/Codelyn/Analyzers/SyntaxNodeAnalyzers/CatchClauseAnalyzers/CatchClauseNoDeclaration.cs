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
        protected override string Category => "Clean Code";

        protected override LocalizableString Title => "Catch Without Specifying Exception Type";

        protected override LocalizableString MessageFormat => "It's recommended to specify the Exception type you want to catch";

        protected override LocalizableString Description => "It's recommended to specify the Exception type you want to catch";

        internal override void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var catchClause = context.Node as CatchClauseSyntax;

            if (catchClause.Declaration is null)
            {
                ReportDiagnostic(context,catchClause.CatchKeyword.GetLocation());
            }

        }

        internal override void AnalyzeSyntaxNode(CompilationStartAnalysisContext context, SyntaxNode catchClauseSyntaxNode)
        {
          
        }
    }
}
