// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CommandLine
{
    internal readonly struct CommandResult
    {
        public static CommandResult Fail { get; } = new CommandResult(success: false);

        public CommandResult(bool success)
        {
            Success = success;
        }

        public bool Success { get; }
    }
}
