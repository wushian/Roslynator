// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator
{
    [Flags]
    internal enum DiagnosticDisplayParts
    {
        None = 0,
        Severity = 1,
        Id = 2,
        Message = 4,
        Path = 8,
        Location = 16,
        All = Severity | Id | Message | Path | Location
    }
}
