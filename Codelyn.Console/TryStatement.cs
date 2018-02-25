using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codelyn.Console
{
    class TryStatement
    {
        public bool HasFinally => FinallyClause != null;
        public CatchClause[] CatchClauses { get; internal set; }
        public FinallyClause FinallyClause { get; internal set; }
    }
}
