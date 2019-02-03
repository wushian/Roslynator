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
        public SymbolDefinitionComparer(bool systemNamespaceFirst = false)
        {
            SystemNamespaceFirst = systemNamespaceFirst;
        }

        public static SymbolDefinitionComparer Instance { get; } = new SymbolDefinitionComparer(systemNamespaceFirst: true);

        public bool SystemNamespaceFirst { get; }

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
                                return CompareNamespace(namespaceSymbol, (INamespaceSymbol)y);
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
                                return CompareNamedType(namedTypeSymbol, (INamedTypeSymbol)y);
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

        private int CompareNamespace(INamespaceSymbol x, INamespaceSymbol y)
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

            if (SystemNamespaceFirst)
            {
                INamespaceSymbol rootNamespace1 = GetContainingNamespace(x, count1);
                INamespaceSymbol rootNamespace2 = GetContainingNamespace(y, count2);

                if (rootNamespace1.Name == "System")
                {
                    if (rootNamespace2.Name != "System")
                        return -1;
                }
                else if (rootNamespace2.Name == "System")
                {
                    return 1;
                }
            }

            while (true)
            {
                if (count1 < 0)
                    return (count2 < 0) ? 0 : -1;

                if (count2 < 0)
                    return 1;

                INamespaceSymbol containingNamespace1 = GetContainingNamespace(x, count1);
                INamespaceSymbol containingNamespace2 = GetContainingNamespace(y, count2);

                int diff = string.CompareOrdinal(containingNamespace1.Name, containingNamespace2.Name);

                if (diff != 0)
                    return diff;

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

            INamespaceSymbol GetContainingNamespace(INamespaceSymbol namespaceSymbol, int count)
            {
                while (count > 0)
                {
                    namespaceSymbol = namespaceSymbol.ContainingNamespace;
                    count--;
                }

                return namespaceSymbol;
            }
        }

        public int CompareNamedType(INamedTypeSymbol x, INamedTypeSymbol y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            int diff = CompareNamespace(x.ContainingNamespace, y.ContainingNamespace);

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

        private int CompareMemberSymbol(ISymbol x, ISymbol y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            int diff = CompareNamespace(x.ContainingNamespace, y.ContainingNamespace);

            if (diff != 0)
                return diff;

            diff = CompareContainingTypes(x.ContainingType, y.ContainingType);

            if (diff != 0)
                return diff;

            MemberDeclarationKind kind1 = GetMemberDeclarationKind(x);
            MemberDeclarationKind kind2 = GetMemberDeclarationKind(y);

            diff = ((int)kind1).CompareTo((int)kind2);

            if (diff != 0)
                return diff;

            diff = string.CompareOrdinal(x.Name, y.Name);

            if (diff != 0)
                return diff;

            switch (kind1)
            {
                case MemberDeclarationKind.Constructor:
                case MemberDeclarationKind.Method:
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

            MemberDeclarationKind GetMemberDeclarationKind(ISymbol symbol)
            {
                switch (symbol.Kind)
                {
                    case SymbolKind.Event:
                        {
                            return MemberDeclarationKind.Event;
                        }
                    case SymbolKind.Field:
                        {
                            var fieldSymbol = (IFieldSymbol)symbol;

                            if (fieldSymbol.IsConst)
                                return MemberDeclarationKind.Const;

                            return MemberDeclarationKind.Field;
                        }
                    case SymbolKind.Method:
                        {
                            var methodSymbol = (IMethodSymbol)symbol;

                            switch (methodSymbol.MethodKind)
                            {
                                case MethodKind.Constructor:
                                    return MemberDeclarationKind.Constructor;
                                case MethodKind.Conversion:
                                    return MemberDeclarationKind.ConversionOperator;
                                case MethodKind.UserDefinedOperator:
                                    return MemberDeclarationKind.Operator;
                                case MethodKind.Ordinary:
                                    return MemberDeclarationKind.Method;
                            }

                            break;
                        }
                    case SymbolKind.Property:
                        {
                            var propertySymbol = (IPropertySymbol)symbol;

                            if (propertySymbol.IsIndexer)
                                return MemberDeclarationKind.Indexer;

                            return MemberDeclarationKind.Property;
                        }
                }

                Debug.Fail(symbol.ToDisplayString(SymbolDisplayFormats.Test));

                return MemberDeclarationKind.None;
            }
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
                if (count1 < 0)
                    return (count2 < 0) ? 0 : -1;

                if (count2 < 0)
                    return 1;

                INamedTypeSymbol containingType1 = GetContainingType(x, count1);
                INamedTypeSymbol containingType2 = GetContainingType(y, count2);

                int diff = string.CompareOrdinal(containingType1.Name, containingType2.Name);

                if (diff != 0)
                    return diff;

                diff = containingType1.TypeParameters.Length.CompareTo(containingType2.TypeParameters.Length);

                if (diff != 0)
                    return diff;

                count1--;
                count2--;
            }

            int CountContainingTypes(INamedTypeSymbol containingType)
            {
                int count = 0;

                while (true)
                {
                    containingType = containingType.ContainingType;

                    if (containingType == null)
                        break;

                    count++;
                }

                return count;
            }

            INamedTypeSymbol GetContainingType(INamedTypeSymbol containingType, int count)
            {
                while (count > 0)
                {
                    containingType = containingType.ContainingType;
                    count--;
                }

                return containingType;
            }
        }

        private int CompareSymbolAndNamespaceSymbol(ISymbol symbol, INamespaceSymbol namespaceSymbol)
        {
            int diff = CompareNamespace(symbol.ContainingNamespace, namespaceSymbol);

            if (diff != 0)
                return diff;

            return 1;
        }

        private int CompareSymbolAndNamedTypeSymbol(ISymbol symbol, INamedTypeSymbol namedTypeSymbol)
        {
            int diff = CompareNamespace(symbol.ContainingNamespace, namedTypeSymbol.ContainingNamespace);

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

            //TODO: compare parameter type
            return 0;
        }
    }
}
