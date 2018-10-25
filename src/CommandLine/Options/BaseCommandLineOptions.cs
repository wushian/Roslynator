// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;

namespace Roslynator.CommandLine
{
    public abstract class BaseCommandLineOptions
    {
        [Option(longName: "log-file")]
        public string LogFile { get; set; }

        [Option(longName: "log-file-verbosity")]
        public string LogFileVerbosity { get; set; }

        [Option(shortName: 'v', longName: "verbosity")]
        public string Verbosity { get; set; }
    }
}
