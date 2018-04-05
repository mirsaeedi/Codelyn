using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Codelyn
{
    internal abstract class SyntaxNodeAnalyzer
    {
        protected virtual string RuleName => GetType().Name;
        internal abstract IEnumerable<RuleViolation> AnalyzeSyntaxNode(CatchClauseSyntax catchClauseSyntax);
    }
}
