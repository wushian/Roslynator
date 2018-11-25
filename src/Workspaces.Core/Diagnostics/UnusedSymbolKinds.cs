// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Diagnostics
{
    //TODO: zrušit Flags
    [Flags]
    internal enum UnusedSymbolKinds
    {
        None = 0,
        Class = 1,
        Delegate = 2,
        Enum = 4,
        Event = 8,
        Field = 16,
        Interface = 32,
        Method = 64,
        Property = 128,
        Struct = 256,
        Type = Class | Delegate | Enum | Interface | Struct,
        Member = Event | Field | Method | Property,
        TypeOrMember = Type | Member    }
}
