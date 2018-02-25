using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codelyn.Console
{
    class CatchClause
    {
        public int StatementsCount { get; internal set; }
        public bool ContainsLog { get; internal set; }
        public string[] Comments { get; internal set; }
        public string ExceptionType { get; internal set; }
        public bool HasIdentifier { get; internal set; }
        public IEnumerable<RuleViolation> RuleViolations { get; internal set; }
    }
}
