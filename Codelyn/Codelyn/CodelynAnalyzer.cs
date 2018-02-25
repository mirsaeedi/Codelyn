using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Codelyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CodelynAnalyzer : DiagnosticAnalyzer
    {
        private DiagnosticDescriptor[] _rules;
        private AnalyzerEngine _engine;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if(_rules is null)
                {
                    InitAnalyzerEngine();
                }

                return ImmutableArray.Create(_rules);
            }
        }

        private void InitAnalyzerEngine()
        {
            _engine = new AnalyzerEngine();
            _engine.RegisterAnalyzers();
            _rules = _engine.RegisteredRules.ToArray();
        }
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(Analyze);
            context.RegisterSyntaxNodeAction<SyntaxKind>(_engine.AnalyzeSyntaxNode, SyntaxKind.CatchClause);
        }

        public void Analyze(CompilationStartAnalysisContext context)
        {
            _engine.AnalyzeSyntaxNode(context);
        }
    }
}
