// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Diagnostics;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal class AnalyzeUnusedCommandExecutor : MSBuildWorkspaceCommandExecutor
    {
        public AnalyzeUnusedCommandExecutor(
            AnalyzeUnusedCommandLineOptions options,
            Visibility visibility,
            UnusedSymbolKinds unusedSymbolKinds,
            string language) : base(language)
        {
            Options = options;
            Visibility = visibility;
            UnusedSymbolKinds = unusedSymbolKinds;
        }

        public AnalyzeUnusedCommandLineOptions Options { get; }

        public Visibility Visibility { get; }

        public UnusedSymbolKinds UnusedSymbolKinds { get; }

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

                foreach (IGrouping<UnusedSymbolKinds, UnusedSymbolInfo> grouping in allUnusedSymbols
                    .GroupBy(f => f.Kind)
                    .OrderBy(f => f.Key))
                {
                    WriteLine($"{grouping.Count()} {grouping.Key.ToString().ToLowerInvariant()} symbols", Verbosity.Normal);
                }
            }

            WriteLine(Verbosity.Minimal);
            WriteLine($"{allUnusedSymbols.Length} unused {((allUnusedSymbols.Length == 1) ? "symbol" : "symbols")} found", ConsoleColor.Green, Verbosity.Minimal);
            WriteLine(Verbosity.Minimal);

            return CommandResult.Success;
        }

        private async Task<ImmutableArray<UnusedSymbolInfo>> AnalyzeProject(Project project, CancellationToken cancellationToken)
        {
            WriteLine($"Analyze '{project.Name}'", ConsoleColor.Cyan, Verbosity.Minimal);

            Compilation compilation = await project.GetCompilationAsync(cancellationToken);

            ImmutableHashSet<ISymbol> ignoredSymbols = Options.IgnoredSymbols
                .Select(f => DocumentationCommentId.GetFirstSymbolForDeclarationId(f, compilation))
                .Where(f => f != null)
                .ToImmutableHashSet();

            return await UnusedSymbolFinder.FindUnusedSymbolsAsync(project, compilation, Predicate, ignoredSymbols, cancellationToken).ConfigureAwait(false);

            bool Predicate(ISymbol symbol)
            {
                return (UnusedSymbolKinds & UnusedSymbolFinder.GetUnusedSymbolKind(symbol)) != 0
                    && IsVisible(symbol);
            }
        }

        private bool IsVisible(ISymbol symbol)
        {
            switch (Visibility)
            {
                case Visibility.Public:
                    return true;
                case Visibility.Internal:
                    return !symbol.IsPubliclyVisible();
                case Visibility.Private:
                    return !symbol.IsPubliclyOrInternallyVisible();
                default:
                    throw new InvalidOperationException();
            }
        }

        protected override void OperationCanceled(OperationCanceledException ex)
        {
            WriteLine("Analysis was canceled.", Verbosity.Quiet);
        }
    }
}
