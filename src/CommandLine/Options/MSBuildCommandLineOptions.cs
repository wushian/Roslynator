// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using CommandLine;

namespace Roslynator.CommandLine
{
    //TODO: Files, IgnoredFiles
    public abstract class MSBuildCommandLineOptions : BaseCommandLineOptions
    {
        [Value(index: 0, Required = true, HelpText = "The project or solution file.", MetaValue = "<PROJECT|SOLUTION>")]
        public string Path { get; set; }

        [Option(longName: "ignored-projects")]
        public IEnumerable<string> IgnoredProjects { get; set; }

        [Option(longName: "language")]
        public string Language { get; set; }

        [Option(longName: "msbuild-path")]
        public string MSBuildPath { get; set; }

        [Option(longName: "projects")]
        public IEnumerable<string> Projects { get; set; }

        [Option(shortName: 'p', longName: "properties")]
        public IEnumerable<string> Properties { get; set; }

        internal ImmutableHashSet<string> GetProjectNames()
        {
            return (Projects != null)
                ? Projects.ToImmutableHashSet()
                : ImmutableHashSet<string>.Empty;
        }

        internal ImmutableHashSet<string> GetIgnoredProjectNames()
        {
            return (IgnoredProjects != null)
                ? IgnoredProjects.ToImmutableHashSet()
                : ImmutableHashSet<string>.Empty;
        }

        internal bool TryGetLanguage(out string value)
        {
            if (Language != null)
                return ParseHelpers.TryParseLanguage(Language, out value);

            value = null;
            return true;
        }
    }
}
