// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;

namespace Roslynator.CommandLine
{
    [Verb("loc")]
    public class AbstractLinesOfCodeCommandLineOptions : MSBuildCommandLineOptions
    {
        [Option(longName: "include-comments")]
        public bool IncludeComments { get; set; }

        [Option(longName: "include-generated")]
        public bool IncludeGenerated { get; set; }

        [Option(longName: "include-preprocessor-directives")]
        public bool IncludePreprocessorDirectives { get; set; }

        [Option(longName: "include-whitespace")]
        public bool IncludeWhiteSpace { get; set; }
    }
}
