// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;

namespace Roslynator.CommandLine
{
    //TODO: NormalizeLineEndings
    [Verb("format")]
    public class FormatCommandLineOptions : MSBuildCommandLineOptions
    {
        [Option(longName: "include-generated")]
        public bool IncludeGenerated { get; set; }

        [Option(longName: "remove-redundant-empty-line")]
        public bool RemoveRedundantEmptyLine { get; set; }
    }
}
