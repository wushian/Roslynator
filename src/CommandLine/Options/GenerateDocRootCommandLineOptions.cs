// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;

namespace Roslynator.CommandLine
{
    [Verb("generate-doc-root", HelpText = "Generates root documentation file from specified assemblies.")]
    public class GenerateDocRootCommandLineOptions : AbstractGenerateDocCommandLineOptions
    {
        [Option(longName: "omit-containing-namespace")]
        public bool OmitContainingNamespace { get; set; }

        [Option(longName: "parts")]
        public IEnumerable<string> Parts { get; set; }

        [Option(longName: "root-directory-url")]
        public string RootDirectoryUrl { get; set; }
    }
}
