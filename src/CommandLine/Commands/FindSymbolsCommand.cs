// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Roslynator.FindSymbols;
using Roslynator.Text;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal class FindSymbolsCommand : MSBuildWorkspaceCommand
    {
        public FindSymbolsCommand(
            FindSymbolsCommandLineOptions options,
            ImmutableArray<Visibility> visibilities,
            SymbolSpecialKinds symbolKinds,
            ImmutableArray<MetadataName> ignoredAttributes,
            string language) : base(language)
        {
            Options = options;
            Visibilities = visibilities;
            SymbolKinds = symbolKinds;
            IgnoredAttributes = ignoredAttributes;
        }

        public FindSymbolsCommandLineOptions Options { get; }

        public ImmutableArray<Visibility> Visibilities { get; }

        public SymbolSpecialKinds SymbolKinds { get; }

        public ImmutableArray<MetadataName> IgnoredAttributes { get; }

        public override async Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
        {
            AssemblyResolver.Register();

            ImmutableArray<ISymbol> allSymbols;

            if (projectOrSolution.IsProject)
            {
                Project project = projectOrSolution.AsProject();

                allSymbols = await AnalyzeProject(project, cancellationToken);
            }
            else
            {
                Solution solution = projectOrSolution.AsSolution();

                ImmutableArray<ISymbol>.Builder symbols = null;

                foreach (Project project in FilterProjects(solution, Options, s => s
                    .GetProjectDependencyGraph()
                    .GetTopologicallySortedProjects(cancellationToken)
                    .ToImmutableArray()))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    ImmutableArray<ISymbol> symbols2 = await AnalyzeProject(project, cancellationToken);

                    if (symbols2.Any())
                    {
                        (symbols ?? (symbols = ImmutableArray.CreateBuilder<ISymbol>())).AddRange(symbols2);
                    }

                    WriteLine($"  {symbols.Count} {((symbols.Count == 1) ? "symbol" : "symbols")} found", Verbosity.Normal);
                }

                allSymbols = symbols?.ToImmutableArray() ?? ImmutableArray<ISymbol>.Empty;
            }

            if (allSymbols.Any())
            {
                WriteLine(Verbosity.Normal);

                foreach (ISymbol symbol in allSymbols.OrderBy(f => f, SymbolDefinitionComparer.Instance))
                {
                    WriteLine(symbol.ToDisplayString(), Verbosity.Normal);
                }

                WriteLine(Verbosity.Normal);

                Dictionary<SymbolSpecialKind, int> countByKind = allSymbols
                    .GroupBy(f => f.GetSpecialKind())
                    .OrderByDescending(f => f.Count())
                    .ThenBy(f => f.Key)
                    .ToDictionary(f => f.Key, f => f.Count());

                int maxCountLength = countByKind.Max(f => f.Value.ToString().Length);

                foreach (KeyValuePair<SymbolSpecialKind, int> kvp in countByKind)
                {
                    WriteLine($"{kvp.Value.ToString().PadLeft(maxCountLength)} {kvp.Key.ToString().ToLowerInvariant()} symbols", Verbosity.Normal);
                }
            }

            WriteLine(Verbosity.Minimal);
            WriteLine($"{allSymbols.Length} {((allSymbols.Length == 1) ? "symbol" : "symbols")} found", ConsoleColor.Green, Verbosity.Minimal);
            WriteLine(Verbosity.Minimal);

            return CommandResult.Success;
        }

        private async Task<ImmutableArray<ISymbol>> AnalyzeProject(Project project, CancellationToken cancellationToken)
        {
            WriteLine($"Analyze '{project.Name}'", Verbosity.Minimal);

            Compilation compilation = await project.GetCompilationAsync(cancellationToken);

            //TODO: IgnoredSymbols
            ImmutableHashSet<ISymbol> ignoredSymbols = Options.IgnoredSymbols
                .Select(f => DocumentationCommentId.GetFirstSymbolForDeclarationId(f, compilation))
                .Where(f => f != null)
                .ToImmutableHashSet();

            var options = new SymbolFinderOptions(
                symbolKinds: SymbolKinds,
                visibilities: Visibilities,
                ignoredAttributes: IgnoredAttributes,
                includeGeneratedCode: Options.IncludeGeneratedCode,
                unusedOnly: Options.UnusedOnly);

            var progress = new FindSymbolsProgress() { UnusedOnly = options.UnusedOnly };

            return await SymbolFinder.FindSymbolsAsync(project, options, progress, cancellationToken).ConfigureAwait(false);
        }

        protected override void OperationCanceled(OperationCanceledException ex)
        {
            WriteLine("Analysis was canceled.", Verbosity.Quiet);
        }

        private class FindSymbolsProgress : IFindSymbolsProgress
        {
            public bool UnusedOnly { get; set; }

            public void OnSymbolFound(ISymbol symbol)
            {
                //TODO: 
                //if (UnusedOnly)
                //{
                //    WriteLine($"  Unused {symbol.GetSpecialKind().ToString()} {symbol.ToDisplayString()}", Verbosity.Normal);
                //}
                //else
                //{
                //    WriteLine($"  {symbol.ToDisplayString()}", Verbosity.Normal);
                //}

                //WriteLine($"    Location: {FormatLocation(symbol.Locations[0])}", ConsoleColor.DarkGray, Verbosity.Detailed);
                //WriteLine($"    Id:       {symbol.GetDocumentationCommentId()}", ConsoleColor.DarkGray, Verbosity.Diagnostic);
            }

            private static string FormatLocation(Location location)
            {
                StringBuilder sb = StringBuilderCache.GetInstance();

                switch (location.Kind)
                {
                    case LocationKind.SourceFile:
                    case LocationKind.XmlFile:
                    case LocationKind.ExternalFile:
                        {
                            FileLinePositionSpan span = location.GetMappedLineSpan();

                            if (span.IsValid)
                            {
                                sb.Append(span.Path);

                                LinePosition linePosition = span.Span.Start;

                                sb.Append('(');
                                sb.Append(linePosition.Line + 1);
                                sb.Append(',');
                                sb.Append(linePosition.Character + 1);
                                sb.Append("): ");
                            }

                            break;
                        }
                }

                return StringBuilderCache.GetStringAndFree(sb);
            }
        }
    }
}
