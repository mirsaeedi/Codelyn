using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codelyn
{
    internal abstract class SyntaxNodeAnalyzer:Analyzer
    {
        internal abstract void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context);

        internal abstract void AnalyzeSyntaxNode(CompilationStartAnalysisContext context, SyntaxNode catchClauseSyntaxNode);
    }
}
