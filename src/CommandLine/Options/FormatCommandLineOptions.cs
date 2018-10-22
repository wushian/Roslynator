// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;

namespace Roslynator.CommandLine
{
    [Verb("format")]
    public class FormatCommandLineOptions : AbstractCommandLineOptions
    {
        [Option(longName: "include-generated")]
        public bool IncludeGenerated { get; set; }
    }
}
