// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;

namespace Roslynator.CommandLine
{
    [Verb("analyze-assembly")]
    public class AnalyzeAssemblyCommandLineOptions
    {
        [Value(index: 0, Required = true)]
        public string Path { get; set; }

        [Option(longName: "language")]
        public string Language { get; set; }

        [Option(longName: "msbuild-path")]
        public string MSBuildPath { get; set; }

        [Option(longName: "no-analyzers")]
        public bool NoAnalyzers { get; set; }

        [Option(longName: "no-fixers")]
        public bool NoFixers { get; set; }

        [Option(shortName: 'p', longName: "properties")]
        public IEnumerable<string> Properties { get; set; }
    }
}
