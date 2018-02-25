using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codelyn
{
    internal abstract class Analyzer
    {
        protected string DiagnosticId => "Codelyn";
        protected abstract string Category { get; }
        protected abstract LocalizableString Title { get; }
        protected abstract LocalizableString MessageFormat { get; }
        protected abstract LocalizableString Description { get; }
        internal DiagnosticDescriptor Rule => new DiagnosticDescriptor
            (DiagnosticId, 
            Title, 
            MessageFormat, 
            Category, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, 
            description: Description);

        protected void ReportDiagnostic(dynamic context,Location location, params object[] messageArgs)
        {
            var diagnostic = Diagnostic.Create(Rule, location, messageArgs);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
