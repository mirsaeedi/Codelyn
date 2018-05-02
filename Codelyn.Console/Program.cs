using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codelyn
{
    class Program
    {
        static  void Main(string[] args)
        {
            string solutionPath = @"C:\Users\Ehsan Mirsaeedi\Downloads\Compressed\MusicStore-dev\MusicStore-dev\MusicStore.sln";

            var analyzerManager = new AnalyzerManager(solutionPath);
            var contexts = analyzerManager.Analyze().Result;
            Save(contexts, @"C:\Users\Ehsan Mirsaeedi\Desktop\contoso.txt");

        }

        private static void Save(List<AnalyzeContext> contexts, string outputPath)
        {
            var violations = contexts.SelectMany(q => q.RuleViolations).ToArray();

            string output = JsonConvert.SerializeObject(violations);
            File.WriteAllText(outputPath, output);
        }
    }
}
