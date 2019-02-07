// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;

namespace Roslynator.CommandLine
{
#if DEBUG
    [Verb("list-symbols", HelpText = "Lists symbols in the specified project or solution.")]
#endif
    public class ListSymbolsCommandLineOptions : MSBuildCommandLineOptions
    {
        [Option(longName: ParameterNames.ContainingNamespaceStyle)]
        public string ContainingNamespaceStyle { get; set; }

        [Option(longName: ParameterNames.Depth)]
        public string Depth { get; set; }

        [Option(longName: "no-precedence-for-system")]
        public bool NoPrecedenceForSystem { get; set; }

        [Option(longName: "output")]
        public string Output { get; set; }

        [Option(longName: ParameterNames.Visibility)]
        public string Visibility { get; set; }
    }
}
