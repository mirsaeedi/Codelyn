using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Codelyn
{
    internal class CatchClauseLosingStackTraceThrows : SyntaxNodeAnalyzer
    {
        protected override string Category => "Clean Code";

        protected override LocalizableString Title => "Losing Stacktrace";

        protected override LocalizableString MessageFormat => "You are losing the stacktrace by throwing this exception. You can preserve the stacktrace using throw alone without any exception";

        protected override LocalizableString Description => "You are losing the stacktrace by throwing this exception. You can preserve the stacktrace using throw alone without any exception";

        internal override void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var catchClause = context.Node as CatchClauseSyntax;

            var throwStatements = catchClause.Block.Statements.OfType<ThrowStatementSyntax>();

            foreach (var throwStatement in throwStatements)
            {
                if (throwStatement.Expression != null)
                    ReportDiagnostic(context, throwStatement.Expression.GetLocation());
            }
        }

        internal override void AnalyzeSyntaxNode(CompilationStartAnalysisContext context, SyntaxNode catchClauseSyntaxNode)
        {
           
        }
    }
}
