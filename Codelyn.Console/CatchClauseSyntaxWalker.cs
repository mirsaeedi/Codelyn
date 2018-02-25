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
        private AnalyzeContext _context;
        private Action<TryStatementSyntax, AnalyzeContext> _action;

        public TryStatementSyntaxWalker(AnalyzeContext context, Action<TryStatementSyntax,AnalyzeContext> action)
        {
            _context = context;
            _action = action;
        }

        public override void VisitTryStatement(TryStatementSyntax node)
        {
            _action(node,_context);
            base.VisitTryStatement(node);
        }



    }
}
