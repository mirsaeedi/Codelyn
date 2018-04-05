using Codelyn.Console;

namespace Codelyn
{
    internal class ModelContext
    {
        public ModelContext()
        {
        }

        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string FileName { get; set; }
        public string ProjectName { get; set; }
        public TryStatement TryStatement { get; set; }
    }
}