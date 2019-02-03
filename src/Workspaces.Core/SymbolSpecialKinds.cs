// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator
{
    [Flags]
    internal enum SymbolSpecialKinds
    {
        None = 0,
        Class = 1,
        Delegate = 2,
        Enum = 4,
        Interface = 8,
        Struct = 16,
        Type = Class | Delegate | Enum | Interface | Struct,
        Event = 32,
        Field = 64,
        Const = 128,
        Method = 256,
        Property = 512,
        Indexer = 1024,
        Member = Event | Field | Const | Method | Property | Indexer,
        TypeOrMember = Type | Member
    }
}
