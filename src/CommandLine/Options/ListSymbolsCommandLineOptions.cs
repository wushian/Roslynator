// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;

namespace Roslynator.CommandLine
{
    //TODO: Output, XmlOutput, AddAssemblyAttributes
#if DEBUG
    [Verb("list-symbols", HelpText = "Lists symbols in the specified project or solution.")]
#endif
    public class ListSymbolsCommandLineOptions : MSBuildCommandLineOptions
    {
        [Option(longName: "depth")]
        public string Depth { get; set; }

        [Option(longName: "visibility")]
        public string Visibility { get; set; }
    }
}
