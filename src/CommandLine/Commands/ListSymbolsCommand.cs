// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal class ListSymbolsCommand : MSBuildWorkspaceCommand
    {
        public ListSymbolsCommand(
            ListSymbolsCommandLineOptions options,
            DefinitionListDepth depth,
            Visibility visibility,
            SymbolDisplayContainingNamespaceStyle containingNamespaceStyle,
            string language) : base(language)
        {
            Options = options;
            Depth = depth;
            Visibility = visibility;
            ContainingNamespaceStyle = containingNamespaceStyle;
        }

        public ListSymbolsCommandLineOptions Options { get; }

        public DefinitionListDepth Depth { get; }

        public Visibility Visibility { get; }

        public SymbolDisplayContainingNamespaceStyle ContainingNamespaceStyle { get; }

        public override async Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
        {
            AssemblyResolver.Register();

            var options = new DefinitionListOptions(
                visibility: Visibility,
                depth: Depth,
                containingNamespaceStyle: ContainingNamespaceStyle);

            var assemblies = new List<IAssemblySymbol>();

            if (projectOrSolution.IsProject)
            {
                Project project = projectOrSolution.AsProject();

                WriteLine($"Compile '{project.Name}'", Verbosity.Minimal);

                Compilation compilation = await project.GetCompilationAsync(cancellationToken);

                assemblies.Add(compilation.Assembly);
            }
            else
            {
                Solution solution = projectOrSolution.AsSolution();

                WriteLine($"Compile solution '{solution.FilePath}'", Verbosity.Minimal);

                Stopwatch stopwatch = Stopwatch.StartNew();

                foreach (Project project in FilterProjects(solution, Options, s => s
                    .GetProjectDependencyGraph()
                    .GetTopologicallySortedProjects(cancellationToken)
                    .ToImmutableArray()))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    WriteLine($"  Compile '{project.Name}'", Verbosity.Minimal);

                    Compilation compilation = await project.GetCompilationAsync(cancellationToken);

                    assemblies.Add(compilation.Assembly);
                }

                stopwatch.Stop();

                WriteLine($"Done compiling solution '{solution.FilePath}' in {stopwatch.Elapsed:mm\\:ss\\.ff}", Verbosity.Minimal);
            }

            string text = null;

            using (var writer = new StringWriter())
            {
                var builder = new DefinitionListWriter(
                    writer,
                    options: options,
                    comparer: SymbolDefinitionComparer.GetInstance(systemNamespaceFirst: !Options.NoPrecedenceForSystem));

                builder.Write(assemblies);

                text = builder.ToString();
            }

            WriteLine(Verbosity.Minimal);
            WriteLine(text, Verbosity.Minimal);

            if (Options.Output != null)
                File.WriteAllText(Options.Output, text, Encoding.UTF8);

            return CommandResult.Success;
        }
    }
}
