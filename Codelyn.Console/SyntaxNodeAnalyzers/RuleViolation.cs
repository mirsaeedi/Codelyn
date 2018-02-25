using Microsoft.CodeAnalysis;

namespace Codelyn
{
    public class RuleViolation
    {
        public RuleViolation(string ruleName)
        {
            RuleName = ruleName;
        }

        public string RuleName { get; set; }
        public Location Location { get; }
    }
}