// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Host.Mef;
using static Roslynator.Logger;

namespace Roslynator.FindSymbols
{
    internal static class SymbolFinder
    {
        internal static async Task<ImmutableArray<ISymbol>> FindSymbolsAsync(
            Project project,
            SymbolFinderOptions options = null,
            IFindSymbolsProgress progress = null,
            CancellationToken cancellationToken = default)
        {
            Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            INamedTypeSymbol generatedCodeAttribute = compilation.GetTypeByMetadataName("System.CodeDom.Compiler.GeneratedCodeAttribute");

            ImmutableArray<ISymbol>.Builder symbols = null;

            var namespaceOrTypeSymbols = new Stack<INamespaceOrTypeSymbol>();

            namespaceOrTypeSymbols.Push(compilation.Assembly.GlobalNamespace);

            while (namespaceOrTypeSymbols.Count > 0)
            {
                INamespaceOrTypeSymbol namespaceOrTypeSymbol = namespaceOrTypeSymbols.Pop();

                foreach (ISymbol symbol in namespaceOrTypeSymbol.GetMembers())
                {
                    if (symbol.IsImplicitlyDeclared)
                        continue;

                    SymbolKind kind = symbol.Kind;

                    if (kind == SymbolKind.Namespace)
                    {
                        namespaceOrTypeSymbols.Push((INamespaceSymbol)symbol);
                        continue;
                    }

                    bool isUnused = false;

                    if (!options.UnusedOnly
                        || UnusedSymbolUtility.CanBeUnusedSymbol(symbol))
                    {
                        if (IsMatch(symbol))
                        {
                            if (options.UnusedOnly)
                            {
                                isUnused = await UnusedSymbolUtility.IsUnusedSymbolAsync(symbol, project.Solution, cancellationToken).ConfigureAwait(false);
                            }

                            if (!options.UnusedOnly
                                || isUnused)
                            {
                                progress?.OnSymbolFound(symbol);

                                (symbols ?? (symbols = ImmutableArray.CreateBuilder<ISymbol>())).Add(symbol);
                            }
                        }
                    }

                    if (!isUnused
                        && kind == SymbolKind.NamedType)
                    {
                        namespaceOrTypeSymbols.Push((INamedTypeSymbol)symbol);
                    }
                }
            }

            return symbols?.ToImmutableArray() ?? ImmutableArray<ISymbol>.Empty;

            bool IsMatch(ISymbol symbol)
            {
                SymbolSpecialKinds symbolKind = GetSymbolKinds(symbol);

                if ((options.SymbolKinds & symbolKind) == 0)
                {
                    if (ShouldWrite(Verbosity.Diagnostic))
                        WriteLine($"Skip {symbolKind.ToString().ToLowerInvariant()} symbol '{symbol}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                    return false;
                }

                Visibility visibility = symbol.GetVisibility();

                Debug.Assert(visibility != Visibility.NotApplicable, $"{visibility} {symbol}");

                if (!options.IsVisible(symbol))
                {
                    if (ShouldWrite(Verbosity.Diagnostic))
                        WriteLine($"Skip {GetVisibilityText(visibility)} visible symbol '{symbol}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                    return false;
                }

                foreach (MetadataName ignoredAttribute in options.IgnoredAttributes)
                {
                    if (symbol.HasAttribute(ignoredAttribute))
                    {
                        if (ShouldWrite(Verbosity.Diagnostic))
                            WriteLine($"Skip symbol '{symbol}' with attribute '{ignoredAttribute}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                        return false;
                    }
                }

                if (!options.IncludeGeneratedCode
                       && GeneratedCodeUtility.IsGeneratedCode(symbol, generatedCodeAttribute, MefWorkspaceServices.Default.GetService<ISyntaxFactsService>(compilation.Language).IsComment, cancellationToken))
                {
                    if (ShouldWrite(Verbosity.Diagnostic))
                        WriteLine($"Skip generated symbol '{symbol}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                    return false;
                }

                return true;
            }
        }

        private static SymbolSpecialKinds GetSymbolKinds(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                    {
                        var namedType = (INamedTypeSymbol)symbol;

                        switch (namedType.TypeKind)
                        {
                            case TypeKind.Class:
                                return SymbolSpecialKinds.Class;
                            case TypeKind.Delegate:
                                return SymbolSpecialKinds.Delegate;
                            case TypeKind.Enum:
                                return SymbolSpecialKinds.Enum;
                            case TypeKind.Interface:
                                return SymbolSpecialKinds.Interface;
                            case TypeKind.Struct:
                                return SymbolSpecialKinds.Struct;
                        }

                        Debug.Fail(namedType.TypeKind.ToString());
                        return SymbolSpecialKinds.None;
                    }
                case SymbolKind.Event:
                    {
                        return SymbolSpecialKinds.Event;
                    }
                case SymbolKind.Field:
                    {
                        return SymbolSpecialKinds.Field;
                    }
                case SymbolKind.Method:
                    {
                        return SymbolSpecialKinds.Method;
                    }
                case SymbolKind.Property:
                    {
                        return SymbolSpecialKinds.Property;
                    }
            }

            Debug.Fail(symbol.Kind.ToString());
            return SymbolSpecialKinds.None;
        }

        private static string GetVisibilityText(Visibility visibility)
        {
            switch (visibility)
            {
                case Visibility.Private:
                    return "privately";
                case Visibility.Internal:
                    return "internally";
                case Visibility.Public:
                    return "public";
            }

            Debug.Fail(visibility.ToString());

            return visibility.ToString();
        }
    }
}
