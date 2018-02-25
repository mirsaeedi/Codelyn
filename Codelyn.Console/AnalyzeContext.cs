using Codelyn.Console;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Codelyn
{
    public class AnalyzeContext
    {
 
        public AnalyzeContext(Project project,
            Document document)
        {
            Project = project;
            Document = document;
            DocumentSemanticModel = document.GetSemanticModelAsync().Result;
        }


        public SyntaxNode TryStatementSyntaxNode { get; set; }
        public Project Project { get; }
        public Document Document { get; }
        public ISymbol Declaration { get; set; }
        public ISymbol Class { get; set; }
        public SemanticModel DocumentSemanticModel { get; internal set; }
        internal TryStatement TryStatement { get; set; }

        public List<RuleViolation> RuleViolations = new List<RuleViolation>();
    }
}