// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CommandLine;

namespace Roslynator.CommandLine
{
#if DEBUG
    [Verb("find-symbols", HelpText = "Finds symbols in the specified project or solution.")]
#endif
    public class FindSymbolsCommandLineOptions : MSBuildCommandLineOptions
    {
        [Option(longName: "ignored-attributes")]
        public IEnumerable<string> IgnoredAttributes { get; set; }

        [Option(longName: "ignore-obsolete")]
        public bool IgnoreObsolete { get; set; }

        [Option(longName: "ignored-symbols")]
        public IEnumerable<string> IgnoredSymbols { get; set; }

        [Option(longName: "include-generated-code")]
        public bool IncludeGeneratedCode { get; set; }

        [Option(longName: "symbol-kinds")]
        public IEnumerable<string> SymbolKinds { get; set; }

        [Option(longName: "unused-only")]
        public bool UnusedOnly { get; set; }

        [Option(longName: "visibility")]
        public IEnumerable<string> Visibility { get; set; }

        internal bool TryGetSymbolKinds(SymbolSpecialKinds defaultValue, out SymbolSpecialKinds value)
        {
            if (!SymbolKinds.Any())
            {
                value = defaultValue;
                return true;
            }

            return ParseHelpers.TryParseSymbolKinds(SymbolKinds, out value);
        }

        internal bool TryGetVisibility(ImmutableArray<Visibility> defaultValue, out ImmutableArray<Visibility> value)
        {
            if (!Visibility.Any())
            {
                value = defaultValue;
                return true;
            }

            ImmutableArray<Visibility>.Builder builder = ImmutableArray.CreateBuilder<Visibility>();

            foreach (string visibilityText in Visibility)
            {
                if (!ParseHelpers.TryParseVisibility(visibilityText, out Visibility visibility))
                {
                    value = default;
                    return false;
                }

                builder.Add(visibility);
            }

            value = builder.ToImmutableArray();
            return true;
        }
    }
}
