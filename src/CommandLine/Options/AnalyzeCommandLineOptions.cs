// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;

namespace Roslynator.CommandLine
{
    [Verb("analyze")]
    public class AnalyzeCommandLineOptions : MSBuildCommandLineOptions
    {
        [Option(shortName: 'a', longName: "analyzer-assemblies")]
        public IEnumerable<string> AnalyzerAssemblies { get; set; }

        [Option(longName: "culture")]
        public string CultureName { get; set; }

        [Option(longName: "execution-time")]
        public bool ExecutionTime { get; set; }

        [Option(longName: "ignore-analyzer-references")]
        public bool IgnoreAnalyzerReferences { get; set; }

        [Option(longName: "ignore-compiler-diagnostics")]
        public bool IgnoreCompilerDiagnostics { get; set; }

        [Option(longName: "ignored-diagnostics")]
        public IEnumerable<string> IgnoredDiagnostics { get; set; }

        [Option(longName: "minimal-severity")]
        public string MinimalSeverity { get; set; }

        [Option(longName: "report-fade-diagnostics")]
        public bool ReportFadeDiagnostics { get; set; }

        [Option(longName: "report-suppressed-diagnostics")]
        public bool ReportSuppressedDiagnostics { get; set; }

        [Option(longName: "supported-diagnostics")]
        public IEnumerable<string> SupportedDiagnostics { get; set; }
    }
}
