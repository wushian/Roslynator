// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;

namespace Roslynator.CommandLine
{
    //TODO: list-references > list-assemblies
#if DEBUG
    [Verb("list-references", HelpText = "Lists assembly references from the specified project or solution.")]
#endif
    public class ListReferencesCommandLineOptions : MSBuildCommandLineOptions
    {
        [Option(longName: "display",
            HelpText = "Defines how the assembly reference is displayed. Allowed values are path (default), file-name, file-name-without-extension or assembly-name.")]
        public string Display { get; set; }
    }
}
