// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Roslynator.Diagnostics;

#if DEBUG
namespace Roslynator.CommandLine
{
    //TODO: Remove, Output
    //TODO: IgnoreGeneratedCode
    [Verb("analyze-unused", HelpText = "Finds unused symbols in the specified project or solution.")]
    public class AnalyzeUnusedCommandLineOptions : MSBuildCommandLineOptions
    {
        [Option(longName: "ignored-symbols")]
        public IEnumerable<string> IgnoredSymbols { get; set; }

        [Option(longName: "scope")]
        public IEnumerable<string> Scope { get; set; }

        //TODO: MaxVisibility
        [Option(longName: "visibility")]
        public string Visibility { get; set; }

        internal bool TryGetScope(UnusedSymbolKinds defaultValue, out UnusedSymbolKinds value)
        {
            if (Scope.Any())
                return ParseHelpers.TryParseUnusedSymbolKinds(Scope, out value);

            value = defaultValue;
            return true;
        }

        internal bool TryGetVisibility(Visibility defaultValue, out Visibility value)
        {
            if (Visibility != null)
                return ParseHelpers.TryParseVisibility(Visibility, out value);

            value = defaultValue;
            return true;
        }
    }
}
#endif
