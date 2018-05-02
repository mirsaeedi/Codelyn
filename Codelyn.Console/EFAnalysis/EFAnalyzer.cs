using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codelyn.Console.TryCatchFinallyAnalysis
{
    public class EFAnalyzer
    {
        public List<RuleViolation> RuleViolations { get; set; } = new List<RuleViolation>();
        public List<AnalyzeContext> AnalyzedContexts { get; set; } = new List<AnalyzeContext>();

        public Project Project { get; }
        public Document Document { get; }
        public SemanticModel DocumentSemanticModel { get; }
        public EFAnalyzer(Project project,
            Document document)
        {
            Project = project;
            Document = document;
            DocumentSemanticModel = document.GetSemanticModelAsync().Result;
        }
        public AnalyzeContext Analyze()
        {
            return null;
        }
    }
}
