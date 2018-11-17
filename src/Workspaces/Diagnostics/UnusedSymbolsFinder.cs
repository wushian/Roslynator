// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Roslynator.CSharp;

namespace Roslynator.Diagnostics
{
    internal static class UnusedSymbolsFinder
    {
        public static async Task<ImmutableArray<ISymbol>> FindUnusedSymbolsAsync(
            Project project,
            Func<ISymbol, bool> predicate = null,
            CancellationToken cancellationToken = default)
        {
            ImmutableArray<ISymbol>.Builder unusedSymbols = null;

            Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            var namespaceOrTypeSymbols = new Stack<INamespaceOrTypeSymbol>();

            namespaceOrTypeSymbols.Push(compilation.Assembly.GlobalNamespace);

            while (namespaceOrTypeSymbols.Count > 0)
            {
                INamespaceOrTypeSymbol namespaceOrTypeSymbol = namespaceOrTypeSymbols.Pop();

                foreach (ISymbol symbol in namespaceOrTypeSymbol.GetMembers())
                {
                    bool isUnused = false;

                    if ((predicate == null || predicate(symbol))
                        && !symbol.IsOverride
                        && IsAnalyzable(symbol))
                    {
                        isUnused = await IsUnusedSymbolAsync(symbol, project, cancellationToken).ConfigureAwait(false);

                        if (isUnused)
                        {
                            (unusedSymbols ?? (unusedSymbols = ImmutableArray.CreateBuilder<ISymbol>())).Add(symbol);
                        }
                    }

                    if (!isUnused
                        && symbol is INamespaceOrTypeSymbol namespaceOrTypeSymbol2)
                    {
                        namespaceOrTypeSymbols.Push(namespaceOrTypeSymbol2);
                    }
                }
            }

            return unusedSymbols?.ToImmutableArray() ?? ImmutableArray<ISymbol>.Empty;

            bool IsAnalyzable(ISymbol symbol)
            {
                switch (symbol.Kind)
                {
                    case SymbolKind.Namespace:
                        {
                            return false;
                        }
                    case SymbolKind.NamedType:
                        {
                            var namedType = (INamedTypeSymbol)symbol;

                            return namedType.TypeKind != TypeKind.Class
                                || !namedType.IsStatic;
                        }
                    case SymbolKind.Event:
                        {
                            var eventSymbol = (IEventSymbol)symbol;

                            return eventSymbol.ExplicitInterfaceImplementations.IsDefaultOrEmpty
                                && !eventSymbol.ImplementsInterfaceMember(allInterfaces: true);
                        }
                    case SymbolKind.Field:
                        {
                            var fieldSymbol = (IFieldSymbol)symbol;

                            return !fieldSymbol.ImplementsInterfaceMember(allInterfaces: true);
                        }
                    case SymbolKind.Property:
                        {
                            var propertySymbol = (IPropertySymbol)symbol;

                            if (propertySymbol.IsIndexer
                                || propertySymbol.Name != "DebuggerDisplay")
                            {
                                if (propertySymbol.ExplicitInterfaceImplementations.IsDefaultOrEmpty
                                    && !propertySymbol.ImplementsInterfaceMember(allInterfaces: true))
                                {
                                    return true;
                                }
                            }

                            return false;
                        }
                    case SymbolKind.Method:
                        {
                            var methodSymbol = (IMethodSymbol)symbol;

                            switch (methodSymbol.MethodKind)
                            {
                                case MethodKind.Ordinary:
                                    {
                                        if (methodSymbol.Parameters.Any()
                                            || methodSymbol.TypeParameters.Any()
                                            || methodSymbol.Name != "GetDebuggerDisplay")
                                        {
                                            if (methodSymbol.ExplicitInterfaceImplementations.IsDefaultOrEmpty
                                                && !methodSymbol.ImplementsInterfaceMember(allInterfaces: true))
                                            {
                                                return true;
                                            }
                                        }

                                        return false;
                                    }
                                case MethodKind.Constructor:
                                    {
                                        return true;
                                    }
                            }

                            return false;
                        }
                    default:
                        {
                            Debug.Fail(symbol.Kind.ToString());
                            return false;
                        }
                }
            }
        }

        private static async Task<bool> IsUnusedSymbolAsync(
            ISymbol symbol,
            Project project,
            CancellationToken cancellationToken)
        {
            ImmutableArray<SyntaxReference> syntaxReferences = symbol.DeclaringSyntaxReferences;

            if (!syntaxReferences.Any())
                return false;

            IEnumerable<ReferencedSymbol> referencedSymbols = await SymbolFinder.FindReferencesAsync(symbol, project.Solution, cancellationToken).ConfigureAwait(false);

            foreach (ReferencedSymbol referencedSymbol in referencedSymbols)
            {
                foreach (ReferenceLocation referenceLocation in referencedSymbol.Locations)
                {
                    if (referenceLocation.IsImplicit)
                        continue;

                    if (referenceLocation.IsCandidateLocation)
                        return false;

                    Location location = referenceLocation.Location;

                    if (!location.IsInSource)
                        continue;

                    foreach (SyntaxReference syntaxReference in syntaxReferences)
                    {
                        if (syntaxReference.SyntaxTree != location.SourceTree
                            || !syntaxReference.Span.Contains(location.SourceSpan))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
