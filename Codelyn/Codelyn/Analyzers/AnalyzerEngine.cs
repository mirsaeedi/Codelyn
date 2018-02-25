using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Codelyn
{
    internal class AnalyzerEngine
    {
        private List<Analyzer> _registeredAnalyzers =new List<Analyzer>();
        private List<DiagnosticDescriptor> _registeredRules = new List<DiagnosticDescriptor>();
        public ReadOnlyCollection<DiagnosticDescriptor> RegisteredRules =>
            new ReadOnlyCollection<DiagnosticDescriptor>(_registeredRules);
        internal void RegisterAnalyzers()
        {
            RegisterAnalyzer<CatchClauseComments>();
            RegisterAnalyzer<CatchClauseEmptyBody>();
            RegisterAnalyzer<CatchClauseGeneralExceptionAnalyzer>();
            RegisterAnalyzer<CatchClauseLosingStackTraceThrows>();
            RegisterAnalyzer<CatchClauseNoDeclaration>();
        }

        private void RegisterAnalyzer<TAnalyzer>()
            where TAnalyzer:Analyzer
        {
            var analyzer = (Analyzer)Activator.CreateInstance<TAnalyzer>();
            _registeredAnalyzers.Add(analyzer);
            _registeredRules.Add(analyzer.Rule);
        }

        public void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            foreach (var analyzer in _registeredAnalyzers)
            {
                ((SyntaxNodeAnalyzer)analyzer).AnalyzeSyntaxNode(context);
            }
        }

        internal void AnalyzeSyntaxNode(CompilationStartAnalysisContext context)
        {
            var trees = context.Compilation.SyntaxTrees;

            var catchClauseSyntaxNodes = trees
                .Select(t => t.GetRoot())
                .SelectMany(r => r.DescendantNodes().Where(n => n.Kind() == SyntaxKind.CatchClause))
                .ToArray();

            foreach (var catchClauseSyntaxNode in catchClauseSyntaxNodes)
            {
                foreach (var analyzer in _registeredAnalyzers)
                {
                    ((SyntaxNodeAnalyzer)analyzer).AnalyzeSyntaxNode(context, catchClauseSyntaxNode);
                }
            }

            
        }
    }
}
