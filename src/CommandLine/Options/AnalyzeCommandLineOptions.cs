// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;

namespace Roslynator.CommandLine
{
    [Verb("analyze", HelpText = "Analyzes specified project or solution and reports diagnostics.")]
    public class AnalyzeCommandLineOptions : AbstractAnalyzeCommandLineOptions
    {
        //TODO: MeasureExecutionTime, Telemetry
        [Option(longName: "execution-time")]
        public bool ExecutionTime { get; set; }

        //TODO: skip, omit
        [Option(longName: "ignore-compiler-diagnostics")]
        public bool IgnoreCompilerDiagnostics { get; set; }

        [Option(longName: "report-fade-diagnostics")]
        public bool ReportFadeDiagnostics { get; set; }

        [Option(longName: "report-suppressed-diagnostics")]
        public bool ReportSuppressedDiagnostics { get; set; }

        //TODO: rename XmlFileLog
        [Option(longName: "xml-file-log")]
        public string XmlFileLog { get; set; }
    }
}
