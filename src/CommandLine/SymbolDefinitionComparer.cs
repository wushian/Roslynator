// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator.CommandLine
{
    internal class SymbolDefinitionComparer : IComparer<ISymbol>
    {
        public static SymbolDefinitionComparer Instance { get; } = new SymbolDefinitionComparer();

        public int Compare(ISymbol x, ISymbol y)
        {
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
                                return CompareNamespaceSymbol(namespaceSymbol, (INamespaceSymbol)y);
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
                                return CompareNamedTypeSymbol(namedTypeSymbol, (INamedTypeSymbol)y);
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
                                return CompareMemberSymbol(x, y);
                        }

                        break;
                    }
            }

            throw new InvalidOperationException();
        }

        private static int CompareNamespaceSymbol(INamespaceSymbol x, INamespaceSymbol y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            if (x.IsGlobalNamespace)
            {
                return (y.IsGlobalNamespace) ? 0 : 1;
            }
            else if (y.IsGlobalNamespace)
            {
                return -1;
            }

            int count1 = CountContainingNamespaces(x);
            int count2 = CountContainingNamespaces(y);

            while (true)
            {
                INamespaceSymbol namespaceSymbol1 = GetNamespaceSymbol(x, count1);
                INamespaceSymbol namespaceSymbol2 = GetNamespaceSymbol(y, count2);

                int diff = CompareName(namespaceSymbol1, namespaceSymbol2);

                if (diff != 0)
                    return diff;

                if (count1 == 0)
                    return (count2 == 0) ? 0 : -1;

                if (count2 == 0)
                    return 1;

                count1--;
                count2--;
            }

            int CountContainingNamespaces(INamespaceSymbol namespaceSymbol)
            {
                int count = 0;

                while (true)
                {
                    namespaceSymbol = namespaceSymbol.ContainingNamespace;

                    if (namespaceSymbol.IsGlobalNamespace)
                        break;

                    count++;
                }

                return count;
            }

            INamespaceSymbol GetNamespaceSymbol(INamespaceSymbol namespaceSymbol, int count)
            {
                while (count > 0)
                {
                    namespaceSymbol = namespaceSymbol.ContainingNamespace;
                    count--;
                }

                return namespaceSymbol;
            }
        }

        public static int CompareNamedTypeSymbol(INamedTypeSymbol x, INamedTypeSymbol y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            int diff = CompareNamespaceSymbol(x.ContainingNamespace, y.ContainingNamespace);

            if (diff != 0)
                return diff;

            diff = GetRank(x).CompareTo(GetRank(y));

            if (diff != 0)
                return diff;

            return CompareContainingTypes(x, y);

            int GetRank(INamedTypeSymbol symbol)
            {
                switch (symbol.TypeKind)
                {
                    case TypeKind.Class:
                        return 1;
                    case TypeKind.Struct:
                        return 2;
                    case TypeKind.Interface:
                        return 3;
                    case TypeKind.Enum:
                        return 4;
                    case TypeKind.Delegate:
                        return 5;
                }

                Debug.Fail(symbol.ToDisplayString(SymbolDisplayFormats.Test));

                return 0;
            }
        }

        private static int CompareMemberSymbol(ISymbol x, ISymbol y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            int diff = CompareNamespaceSymbol(x.ContainingNamespace, y.ContainingNamespace);

            if (diff != 0)
                return diff;

            diff = CompareContainingTypes(x.ContainingType, y.ContainingType);

            if (diff != 0)
                return diff;

            MemberDeclarationKind kind1 = x.GetMemberDeclarationKind();
            MemberDeclarationKind kind2 = y.GetMemberDeclarationKind();

            diff = ((int)kind1).CompareTo((int)kind2);

            if (diff != 0)
                return diff;

            diff = CompareName(x, y);

            if (diff != 0)
                return diff;

            switch (kind1)
            {
                case MemberDeclarationKind.Constructor:
                case MemberDeclarationKind.OrdinaryMethod:
                    {
                        var methodSymbol1 = (IMethodSymbol)x;
                        var methodSymbol2 = (IMethodSymbol)y;

                        diff = methodSymbol1.TypeParameters.Length.CompareTo(methodSymbol2.TypeParameters.Length);

                        if (diff != 0)
                            return diff;

                        return CompareParameters(methodSymbol1.Parameters, methodSymbol2.Parameters);
                    }
                case MemberDeclarationKind.Indexer:
                case MemberDeclarationKind.Property:
                    {
                        var propertySymbol1 = (IPropertySymbol)x;
                        var propertySymbol2 = (IPropertySymbol)y;

                        return CompareParameters(propertySymbol1.Parameters, propertySymbol2.Parameters);
                    }
            }

            return 0;
        }

        private static int CompareContainingTypes(INamedTypeSymbol x, INamedTypeSymbol y)
        {
            if (x == null)
                return (y == null) ? 0 : -1;

            if (y == null)
                return 1;

            int count1 = CountContainingTypes(x);
            int count2 = CountContainingTypes(y);

            while (true)
            {
                INamedTypeSymbol containingType1 = GetContainingType(x, count1);
                INamedTypeSymbol containingType2 = GetContainingType(y, count2);

                int diff = CompareName(containingType1, containingType2);

                if (diff != 0)
                    return diff;

                diff = containingType1.TypeParameters.Length.CompareTo(containingType2.TypeParameters.Length);

                if (diff != 0)
                    return diff;

                if (count1 == 0)
                    return (count2 == 0) ? 0 : -1;

                if (count2 == 0)
                    return 1;

                count1--;
                count2--;
            }

            int CountContainingTypes(INamedTypeSymbol namedType)
            {
                int count = 0;

                while (true)
                {
                    namedType = namedType.ContainingType;

                    if (namedType == null)
                        break;

                    count++;
                }

                return count;
            }

            INamedTypeSymbol GetContainingType(INamedTypeSymbol namedType, int count)
            {
                while (count > 0)
                {
                    namedType = namedType.ContainingType;
                    count--;
                }

                return namedType;
            }
        }

        private static int CompareSymbolAndNamespaceSymbol(ISymbol symbol, INamespaceSymbol namespaceSymbol)
        {
            int diff = CompareNamespaceSymbol(symbol.ContainingNamespace, namespaceSymbol);

            if (diff != 0)
                return diff;

            return 1;
        }

        private static int CompareSymbolAndNamedTypeSymbol(ISymbol symbol, INamedTypeSymbol namedTypeSymbol)
        {
            int diff = CompareNamespaceSymbol(symbol.ContainingNamespace, namedTypeSymbol.ContainingNamespace);

            if (diff != 0)
                return diff;

            diff = CompareContainingTypes(symbol.ContainingType, namedTypeSymbol);

            if (diff != 0)
                return diff;

            return 1;
        }

        private static int CompareParameters(ImmutableArray<IParameterSymbol> parameters1, ImmutableArray<IParameterSymbol> parameters2)
        {
            int length = parameters1.Length;

            int diff = length.CompareTo(parameters2.Length);

            if (diff != 0)
                return diff;

            for (int i = 0; i < length; i++)
            {
                diff = CompareParameter(parameters1[i], parameters2[i]);

                if (diff != 0)
                    return diff;
            }

            return 0;
        }

        private static int CompareParameter(IParameterSymbol x, IParameterSymbol y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            int diff = ((int)x.RefKind).CompareTo((int)y.RefKind);

            if (diff != 0)
                return diff;

            return CompareName(x, y);
        }

        private static int CompareName(ISymbol symbol1, ISymbol symbol2)
        {
            return string.Compare(symbol1.Name, symbol2.Name, StringComparison.Ordinal);
        }
    }
}
