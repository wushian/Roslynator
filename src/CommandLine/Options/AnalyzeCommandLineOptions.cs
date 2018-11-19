// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;

namespace Roslynator.CommandLine
{
    [Verb("analyze", HelpText = "Analyzes specified project or solution and reports diagnostics.")]
    public class AnalyzeCommandLineOptions : AbstractAnalyzeCommandLineOptions
    {
        [Option(longName: "execution-time")]
        public bool ExecutionTime { get; set; }

        [Option(longName: "ignore-compiler-diagnostics")]
        public bool IgnoreCompilerDiagnostics { get; set; }

        [Option(longName: "output")]
        public string Output { get; set; }

        [Option(longName: "report-not-configurable")]
        public bool ReportNotConfigurable { get; set; }

        [Option(longName: "report-suppressed-diagnostics")]
        public bool ReportSuppressedDiagnostics { get; set; }
    }
}
