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
            string solutionPath = @"E:\Developing\WebApplication3\WebApplication3.sln";

            var analyzerManager = new AnalyzerManager(solutionPath);
            var tryCatchStatements = analyzerManager.Analyze().Result;

        }

        private static void Save(List<AnalyzeContext> tryCatchStatements, string outputPath)
        {
            var tryCatchStatementModels = tryCatchStatements.Select(c => new {
                ClassName = c.Class.OriginalDefinition.Name,
                MethodName = c.Declaration.OriginalDefinition.Name,
                FileName = c.Document.Name,
                ProjectName = c.Project.Name,
                TryStatement = new
                {
                    c.TryStatement.CatchClauses,
                    c.TryStatement.FinallyClause,
                    c.TryStatement.HasFinally
                }

            });

            string output = JsonConvert.SerializeObject(tryCatchStatementModels);
            File.WriteAllText(outputPath, output);
        }
    }
}
