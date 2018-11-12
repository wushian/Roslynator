// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;
using Microsoft.CodeAnalysis;

namespace Roslynator.CommandLine
{
    public abstract class AbstractAnalyzeCommandLineOptions : MSBuildCommandLineOptions
    {
        [Option(shortName: 'a', longName: "analyzer-assemblies")]
        public IEnumerable<string> AnalyzerAssemblies { get; set; }

        [Option(longName: "culture")]
        public string Culture { get; set; }

        [Option(longName: "ignore-analyzer-references")]
        public bool IgnoreAnalyzerReferences { get; set; }

        [Option(longName: "ignored-diagnostics")]
        public IEnumerable<string> IgnoredDiagnostics { get; set; }

        [Option(longName: "minimal-severity")]
        public string MinimalSeverity { get; set; }

        [Option(longName: "supported-diagnostics")]
        public IEnumerable<string> SupportedDiagnostics { get; set; }

        [Option(longName: "use-roslynator-analyzers")]
        public bool UseRoslynatorAnalyzers { get; set; }

        internal bool TryGetMinimalSeverity(DiagnosticSeverity defaultValue, out DiagnosticSeverity value)
        {
            if (MinimalSeverity != null)
                return ParseHelpers.TryParseDiagnosticSeverity(MinimalSeverity, out value);

            value = defaultValue;
            return true;
        }
    }
}
