// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;

namespace Roslynator.CommandLine
{
    [Verb("fix", HelpText = "Fixes diagnostics in the specified project or solution.")]
    public class FixCommandLineOptions : AbstractAnalyzeCommandLineOptions
    {
        //TODO: Batch
        [Option(longName: "batch-size", Default = -1)]
        public int BatchSize { get; set; }

        //TODO: DiagnosticsFixableOneByOne, FixOneByOneDiagnostics
        [Option(longName: "fixable-one-by-one-diagnostics")]
        public IEnumerable<string> FixableOneByOneDiagnostics { get; set; }

        [Option(longName: "diagnostic-fixer-map")]
        public IEnumerable<string> DiagnosticFixerMap { get; set; }

        [Option(longName: "diagnostic-fix-map")]
        public IEnumerable<string> DiagnosticFixMap { get; set; }

        [Option(longName: "file-banner")]
        public string FileBanner { get; set; }

        [Option(longName: "format")]
        public bool Format { get; set; }

        [Option(longName: "ignore-compiler-errors")]
        public bool IgnoreCompilerErrors { get; set; }

        [Option(longName: "ignored-compiler-diagnostics")]
        public IEnumerable<string> IgnoredCompilerDiagnostics { get; set; }

        [Option(longName: "max-iterations", Default = -1)]
        public int MaxIterations { get; set; }

        //TODO: UseCodeFixes, UseEmbeddedCodeFixes
        [Option(longName: "use-roslynator-code-fixes")]
        public bool UseRoslynatorCodeFixes { get; set; }
    }
}
