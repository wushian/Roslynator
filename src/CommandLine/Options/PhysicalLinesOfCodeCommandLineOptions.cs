// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;

namespace Roslynator.CommandLine
{
    [Verb("loc", HelpText = "Counts physical lines of code in the specified project or solution.")]
    public class PhysicalLinesOfCodeCommandLineOptions : AbstractLinesOfCodeCommandLineOptions
    {
        [Option(longName: "ignore-block-boundary")]
        public bool IgnoreBlockBoundary { get; set; }

        [Option(longName: "include-comments")]
        public bool IncludeComments { get; set; }

        [Option(longName: "include-preprocessor-directives")]
        public bool IncludePreprocessorDirectives { get; set; }

        [Option(longName: "include-whitespace")]
        public bool IncludeWhitespace { get; set; }
    }
}
