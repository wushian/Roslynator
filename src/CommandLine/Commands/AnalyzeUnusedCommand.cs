// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.FindSymbols;
using Roslynator.Host.Mef;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal class AnalyzeUnusedCommand : MSBuildWorkspaceCommand
    {
        public AnalyzeUnusedCommand(
            AnalyzeUnusedCommandLineOptions options,
            Visibility visibility,
            UnusedSymbolKinds unusedSymbolKinds,
            ImmutableArray<MetadataName> ignoredAttributes,
            string language) : base(language)
        {
            Options = options;
            Visibility = visibility;
            UnusedSymbolKinds = unusedSymbolKinds;
            IgnoredAttributes = ignoredAttributes;
        }

        public AnalyzeUnusedCommandLineOptions Options { get; }

        public Visibility Visibility { get; }

        public UnusedSymbolKinds UnusedSymbolKinds { get; }

        public ImmutableArray<MetadataName> IgnoredAttributes { get; }

        public override async Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
        {
            AssemblyResolver.Register();

            ImmutableArray<UnusedSymbolInfo> allUnusedSymbols;

            if (projectOrSolution.IsProject)
            {
                Project project = projectOrSolution.AsProject();

                allUnusedSymbols = await AnalyzeProject(project, cancellationToken);
            }
            else
            {
                Solution solution = projectOrSolution.AsSolution();

                ImmutableArray<UnusedSymbolInfo>.Builder unusedSymbols = null;

                foreach (Project project in FilterProjects(solution, Options, s => s
                    .GetProjectDependencyGraph()
                    .GetTopologicallySortedProjects(cancellationToken)
                    .ToImmutableArray()))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    ImmutableArray<UnusedSymbolInfo> unusedSymbols2 = await AnalyzeProject(project, cancellationToken);

                    if (unusedSymbols2.Any())
                    {
                        (unusedSymbols ?? (unusedSymbols = ImmutableArray.CreateBuilder<UnusedSymbolInfo>())).AddRange(unusedSymbols2);
                    }
                }

                allUnusedSymbols = unusedSymbols?.ToImmutableArray() ?? ImmutableArray<UnusedSymbolInfo>.Empty;
            }

            if (allUnusedSymbols.Any())
            {
                WriteLine(Verbosity.Normal);

                Dictionary<UnusedSymbolKind, int> countByKind = allUnusedSymbols
                    .GroupBy(f => UnusedSymbolFinder.GetUnusedSymbolKind(f.Symbol))
                    .OrderByDescending(f => f.Count())
                    .ThenBy(f => f.Key)
                    .ToDictionary(f => f.Key, f => f.Count());

                int maxCountLength = countByKind.Sum(f => f.Value.ToString().Length);

                foreach (KeyValuePair<UnusedSymbolKind, int> kvp in countByKind)
                {
                    WriteLine($"{kvp.Value.ToString().PadLeft(maxCountLength)} {kvp.Key.ToString().ToLowerInvariant()} symbols", Verbosity.Normal);
                }
            }

            WriteLine(Verbosity.Minimal);
            WriteLine($"{allUnusedSymbols.Length} unused {((allUnusedSymbols.Length == 1) ? "symbol" : "symbols")} found", ConsoleColor.Green, Verbosity.Minimal);
            WriteLine(Verbosity.Minimal);

            return CommandResult.Success;
        }

        private async Task<ImmutableArray<UnusedSymbolInfo>> AnalyzeProject(Project project, CancellationToken cancellationToken)
        {
            WriteLine($"Analyze '{project.Name}'", Verbosity.Minimal);

            Compilation compilation = await project.GetCompilationAsync(cancellationToken);

            INamedTypeSymbol generatedCodeAttribute = compilation.GetTypeByMetadataName("System.CodeDom.Compiler.GeneratedCodeAttribute");

            ImmutableHashSet<ISymbol> ignoredSymbols = Options.IgnoredSymbols
                .Select(f => DocumentationCommentId.GetFirstSymbolForDeclarationId(f, compilation))
                .Where(f => f != null)
                .ToImmutableHashSet();

            return await UnusedSymbolFinder.FindUnusedSymbolsAsync(project, compilation, Predicate, cancellationToken: cancellationToken).ConfigureAwait(false);

            bool Predicate(ISymbol symbol)
            {
                if (ignoredSymbols.Contains(symbol))
                {
                    if (ShouldWrite(Verbosity.Diagnostic))
                        WriteLine($"Skip symbol '{symbol}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                    return false;
                }

                UnusedSymbolKinds unusedSymbolKind = GetUnusedSymbolKinds(symbol);

                if ((UnusedSymbolKinds & unusedSymbolKind) == 0)
                {
                    if (ShouldWrite(Verbosity.Diagnostic))
                        WriteLine($"Skip {unusedSymbolKind.ToString().ToLowerInvariant()} symbol '{symbol}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                    return false;
                }

                Visibility visibility = symbol.GetVisibility();

                Debug.Assert(visibility != Visibility.NotApplicable, $"{visibility} {symbol}");

                if (visibility > Visibility)
                {
                    if (ShouldWrite(Verbosity.Diagnostic))
                        WriteLine($"Skip {GetVisibilityText(visibility)} visible symbol '{symbol}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                    return false;
                }

                foreach (MetadataName ignoredAttribute in IgnoredAttributes)
                {
                    if (symbol.HasAttribute(ignoredAttribute))
                    {
                        if (ShouldWrite(Verbosity.Diagnostic))
                            WriteLine($"Skip symbol '{symbol}' with attribute '{ignoredAttribute}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                        return false;
                    }
                }

                if (!Options.IncludeGeneratedCode
                       && GeneratedCodeUtility.IsGeneratedCode(symbol, generatedCodeAttribute, MefWorkspaceServices.Default.GetService<ISyntaxFactsService>(project.Language).IsComment, cancellationToken))
                {
                    if (ShouldWrite(Verbosity.Diagnostic))
                        WriteLine($"Skip generated symbol '{symbol}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                    return false;
                }

                return true;
            }
        }

        protected override void OperationCanceled(OperationCanceledException ex)
        {
            WriteLine("Analysis was canceled.", Verbosity.Quiet);
        }

        private static UnusedSymbolKinds GetUnusedSymbolKinds(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                    {
                        var namedType = (INamedTypeSymbol)symbol;

                        switch (namedType.TypeKind)
                        {
                            case TypeKind.Class:
                                return UnusedSymbolKinds.Class;
                            case TypeKind.Delegate:
                                return UnusedSymbolKinds.Delegate;
                            case TypeKind.Enum:
                                return UnusedSymbolKinds.Enum;
                            case TypeKind.Interface:
                                return UnusedSymbolKinds.Interface;
                            case TypeKind.Struct:
                                return UnusedSymbolKinds.Struct;
                        }

                        Debug.Fail(namedType.TypeKind.ToString());
                        return UnusedSymbolKinds.None;
                    }
                case SymbolKind.Event:
                    {
                        return UnusedSymbolKinds.Event;
                    }
                case SymbolKind.Field:
                    {
                        return UnusedSymbolKinds.Field;
                    }
                case SymbolKind.Method:
                    {
                        return UnusedSymbolKinds.Method;
                    }
                case SymbolKind.Property:
                    {
                        return UnusedSymbolKinds.Property;
                    }
            }

            Debug.Fail(symbol.Kind.ToString());
            return UnusedSymbolKinds.None;
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
