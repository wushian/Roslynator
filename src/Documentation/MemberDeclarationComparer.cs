// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    internal sealed class MemberDeclarationComparer : IComparer<ISymbol>
    {
        public static MemberDeclarationComparer Instance { get; } = new MemberDeclarationComparer();

        public int Compare(ISymbol x, ISymbol y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            int result = ((int)x.GetMemberDeclarationKind()).CompareTo((int)y.GetMemberDeclarationKind());

            if (result != 0)
                return result;

            result = string.Compare(x.Name, y.Name, StringComparison.Ordinal);

            if (result != 0)
                return result;

            return string.CompareOrdinal(
                x.ToDisplayString(SymbolDisplayFormats.SortDeclarationList),
                y.ToDisplayString(SymbolDisplayFormats.SortDeclarationList));
        }
    }
}
