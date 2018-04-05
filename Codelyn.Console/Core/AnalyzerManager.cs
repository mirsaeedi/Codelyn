using Codelyn.Console;
using Codelyn.Console.TryCatchFinallyAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codelyn
{
    internal class AnalyzerManager
    {
        private string _solutionPath;
        public List<RuleViolation> RuleViolations { get; set; } = new List<RuleViolation>();
        public List<AnalyzeContext> AnalyzedContexts { get; set; } = new List<AnalyzeContext>();

        public AnalyzerManager(string solutionPath)
        {
            _solutionPath = solutionPath;
        }

        internal async Task<List<AnalyzeContext>> Analyze()
        {
            var msWorkspace = MSBuildWorkspace.Create();

            var solution = await msWorkspace.OpenSolutionAsync(_solutionPath);

            foreach (var project in solution.Projects)
            {
                await AnalyzeProject(project);
            }

            return AnalyzedContexts;
        }

        private async Task AnalyzeProject(Project project)
        {
            foreach (var document in project.Documents)
            {
                await AnalyzeDocument(project, document);
            }
        }

        private async Task AnalyzeDocument(Project project, Document document)
        {
            var tree = await document.GetSyntaxTreeAsync();

            if (tree.GetRoot().Language != "C#")
                return;

            var efAnalyzer = new EFAnalyzer(project, document);
            var walker = new EFSyntaxWalker(efAnalyzer);
            walker.Visit((CompilationUnitSyntax)tree.GetRoot());
        }
    }
}