// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Roslynator.FindSymbols;

namespace Roslynator.Documentation
{
    internal static class SymbolDefinitionWriterHelpers
    {
        public static string GetAccessorName(IMethodSymbol accessorSymbol)
        {
            switch (accessorSymbol.MethodKind)
            {
                case MethodKind.EventAdd:
                    return "add";
                case MethodKind.EventRemove:
                    return "remove";
                case MethodKind.PropertyGet:
                    return "get";
                case MethodKind.PropertySet:
                    return "set";
                default:
                    return null;
            }
        }

        public static bool HasAttributes(ISymbol symbol, SymbolFilterOptions filter)
        {
            if (IsMatch(symbol))
                return true;

            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                    {
                        var typeSymbol = (INamedTypeSymbol)symbol;

                        if (typeSymbol.TypeKind == TypeKind.Delegate
                            && HasAttributes(typeSymbol.DelegateInvokeMethod?.Parameters ?? ImmutableArray<IParameterSymbol>.Empty))
                        {
                            return true;
                        }

                        break;
                    }
                case SymbolKind.Event:
                    {
                        var eventSymbol = (IEventSymbol)symbol;

                        if (IsMatch(eventSymbol.AddMethod))
                            return true;

                        if (IsMatch(eventSymbol.RemoveMethod))
                            return true;

                        break;
                    }
                case SymbolKind.Method:
                    {
                        var methodSymbol = (IMethodSymbol)symbol;

                        if (HasAttributes(methodSymbol.Parameters))
                            return true;

                        break;
                    }
                case SymbolKind.Property:
                    {
                        var propertySymbol = (IPropertySymbol)symbol;

                        if (HasAttributes(propertySymbol.Parameters))
                            return true;

                        if (IsMatch(propertySymbol.GetMethod))
                            return true;

                        if (IsMatch(propertySymbol.SetMethod))
                            return true;

                        break;
                    }
            }

            return false;

            bool HasAttributes(ImmutableArray<IParameterSymbol> parameters)
            {
                return parameters.Any(f => IsMatch(f));
            }

            bool IsMatch(ISymbol s)
            {
                if (s != null)
                {
                    foreach (AttributeData attribute in s.GetAttributes())
                    {
                        if (filter.IsMatch(s, attribute))
                            return true;
                    }
                }

                return false;
            }
        }

        public static ImmutableArray<IParameterSymbol> GetParameters(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                    return GetParameters((INamedTypeSymbol)symbol);
                case SymbolKind.Method:
                    return ((IMethodSymbol)symbol).Parameters;
                case SymbolKind.Property:
                    return ((IPropertySymbol)symbol).Parameters;
            }

            return ImmutableArray<IParameterSymbol>.Empty;
        }

        public static ImmutableArray<IParameterSymbol> GetParameters(INamedTypeSymbol typeSymbol)
        {
            if (typeSymbol.TypeKind == TypeKind.Delegate)
            {
                IMethodSymbol delegateInvokeMethod = typeSymbol.DelegateInvokeMethod;

                if (delegateInvokeMethod != null)
                    return delegateInvokeMethod.Parameters;
            }

            return ImmutableArray<IParameterSymbol>.Empty;
        }

        public static (IMethodSymbol accessor1, IMethodSymbol accessor2) GetAccessors(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Event:
                    {
                        var eventSymbol = (IEventSymbol)symbol;

                        return (eventSymbol.AddMethod, eventSymbol.RemoveMethod);
                    }
                case SymbolKind.Property:
                    {
                        var propertySymbol = (IPropertySymbol)symbol;

                        return (propertySymbol.GetMethod, propertySymbol.SetMethod);
                    }
            }

            return default;
        }
    }
}
