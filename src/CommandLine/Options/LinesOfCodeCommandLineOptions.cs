// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;

namespace Roslynator.CommandLine
{
    [Verb("loc")]
    public class LinesOfCodeCommandLineOptions : AbstractLinesOfCodeCommandLineOptions
    {
        [Option(longName: "ignore-block-boundary")]
        public bool IgnoreBlockBoundary { get; set; }
    }
}
