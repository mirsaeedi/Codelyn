using Codelyn.Console.TryCatchFinallyAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codelyn.Console
{
    public class TryStatementSyntaxWalker : CSharpSyntaxWalker
    {
        private TryAnalyzer _tryAnalyzer;

        public TryStatementSyntaxWalker(TryAnalyzer tryAnalyzer)
        {
            _tryAnalyzer = tryAnalyzer;
        }

        public override void VisitTryStatement(TryStatementSyntax node)
        {
            _tryAnalyzer.AnalyzeTryStatements(node);
            base.VisitTryStatement(node);
        }
    }
}
