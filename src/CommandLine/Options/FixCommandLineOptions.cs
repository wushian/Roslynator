// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;

namespace Roslynator.CommandLine
{
    [Verb("fix")]
    public class FixCommandLineOptions : MSBuildCommandLineOptions
    {
        [Option(shortName: 'a', longName: "analyzer-assemblies")]
        public IEnumerable<string> AnalyzerAssemblies { get; set; }

        [Option(longName: "batch-size", Default = -1)]
        public int BatchSize { get; set; }

        [Option(longName: "format")]
        public bool Format { get; set; }

        [Option(longName: "ignore-analyzer-references")]
        public bool IgnoreAnalyzerReferences { get; set; }

        [Option(longName: "ignore-compiler-errors")]
        public bool IgnoreCompilerErrors { get; set; }

        [Option(longName: "ignored-compiler-diagnostics")]
        public IEnumerable<string> IgnoredCompilerDiagnostics { get; set; }

        [Option(longName: "ignored-diagnostics")]
        public IEnumerable<string> IgnoredDiagnostics { get; set; }

        [Option(longName: "minimal-severity")]
        public string MinimalSeverity { get; set; }

        [Option(longName: "supported-diagnostics")]
        public IEnumerable<string> SupportedDiagnostics { get; set; }

        [Option(longName: "use-roslynator-analyzers")]
        public bool UseRoslynatorAnalyzers { get; set; }
    }
}
