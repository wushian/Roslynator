// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;

namespace Roslynator.CommandLine
{
    [Verb("format")]
    public class FormatCommandLineOptions
    {
        [Value(index: 0, Required = true)]
        public string SolutionPath { get; set; }

        [Option(longName: "ignored-projects")]
        public IEnumerable<string> IgnoredProjects { get; set; }

        [Option(longName: "msbuild-path")]
        public string MSBuildPath { get; set; }

        [Option(shortName: 'p', longName: "properties")]
        public IEnumerable<string> Properties { get; set; }
    }
}
