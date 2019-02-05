// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal class SymbolDefinitionComparer : IComparer<ISymbol>
    {
        public bool SystemNamespaceFirst { get; }

        public static SymbolDefinitionComparer Instance { get; } = new SymbolDefinitionComparer();

        public int Compare(ISymbol x, ISymbol y)
        {
            Debug.Assert(x.IsDefinition, $"symbol is not definition: {x.ToDisplayString()}");
            Debug.Assert(y.IsDefinition, $"symbol is not definition: {y.ToDisplayString()}");

            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            switch (x.Kind)
            {
                case SymbolKind.Namespace:
                    {
                        var namespaceSymbol = (INamespaceSymbol)x;

                        switch (y.Kind)
                        {
                            case SymbolKind.Namespace:
                                return NamespaceSymbolDefinitionComparer.GetInstance(SystemNamespaceFirst).Compare(namespaceSymbol, (INamespaceSymbol)y);
                            case SymbolKind.NamedType:
                            case SymbolKind.Event:
                            case SymbolKind.Field:
                            case SymbolKind.Method:
                            case SymbolKind.Property:
                                return -CompareSymbolAndNamespaceSymbol(y, namespaceSymbol);
                        }

                        break;
                    }
                case SymbolKind.NamedType:
                    {
                        var namedTypeSymbol = (INamedTypeSymbol)x;

                        switch (y.Kind)
                        {
                            case SymbolKind.Namespace:
                                return CompareSymbolAndNamespaceSymbol(namedTypeSymbol, (INamespaceSymbol)y);
                            case SymbolKind.NamedType:
                                return NamedTypeSymbolDefinitionComparer.GetInstance(SystemNamespaceFirst).Compare(namedTypeSymbol, (INamedTypeSymbol)y);
                            case SymbolKind.Event:
                            case SymbolKind.Field:
                            case SymbolKind.Method:
                            case SymbolKind.Property:
                                return -CompareSymbolAndNamedTypeSymbol(y, namedTypeSymbol);
                        }

                        break;
                    }
                case SymbolKind.Event:
                case SymbolKind.Field:
                case SymbolKind.Method:
                case SymbolKind.Property:
                    {
                        switch (y.Kind)
                        {
                            case SymbolKind.Namespace:
                                return CompareSymbolAndNamespaceSymbol(x, (INamespaceSymbol)y);
                            case SymbolKind.NamedType:
                                return CompareSymbolAndNamedTypeSymbol(x, (INamedTypeSymbol)y);
                            case SymbolKind.Event:
                            case SymbolKind.Field:
                            case SymbolKind.Method:
                            case SymbolKind.Property:
                                return MemberSymbolDefinitionComparer.GetInstance(SystemNamespaceFirst).Compare(x, y);
                        }

                        break;
                    }
            }

            throw new InvalidOperationException();
        }

        private int CompareSymbolAndNamespaceSymbol(ISymbol symbol, INamespaceSymbol namespaceSymbol)
        {
            int diff = NamespaceSymbolDefinitionComparer.GetInstance(SystemNamespaceFirst).Compare(symbol.ContainingNamespace, namespaceSymbol);

            if (diff != 0)
                return diff;

            return 1;
        }

        private int CompareSymbolAndNamedTypeSymbol(ISymbol symbol, INamedTypeSymbol namedTypeSymbol)
        {
            int diff = NamedTypeSymbolDefinitionComparer.GetInstance(SystemNamespaceFirst).Compare(symbol.ContainingType, namedTypeSymbol);

            if (diff != 0)
                return diff;

            return 1;
        }

        public static int CompareName(ISymbol symbol1, ISymbol symbol2)
        {
            return string.Compare(symbol1.Name, symbol2.Name, StringComparison.Ordinal);
        }
    }
}
