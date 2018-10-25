// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CodeFixes;
using static Roslynator.ConsoleHelpers;

namespace Roslynator.CommandLine
{
    internal class FixCommandExecutor : MSBuildWorkspaceCommandExecutor
    {
        private static ImmutableHashSet<string> _roslynatorAnalyzerAssemblies;

        public FixCommandExecutor(FixCommandLineOptions options, DiagnosticSeverity minimalSeverity)
        {
            Options = options;
            MinimalSeverity = minimalSeverity;
        }

        public static ImmutableHashSet<string> RoslynatorAnalyzerAssemblies
        {
            get
            {
                return _roslynatorAnalyzerAssemblies ?? (_roslynatorAnalyzerAssemblies = ImmutableHashSet.CreateRange(new string[]
                {
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Roslynator.CSharp.Analyzers.dll"),
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Roslynator.CSharp.Analyzers.CodeFixes.dll"),
                }));
            }
        }

        public FixCommandLineOptions Options { get; }

        public DiagnosticSeverity MinimalSeverity { get; }

        public override async Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
        {
            AssemblyResolver.Register();

            var codeFixerOptions = new CodeFixerOptions(
                minimalSeverity: MinimalSeverity,
                ignoreCompilerErrors: Options.IgnoreCompilerErrors,
                ignoreAnalyzerReferences: Options.IgnoreAnalyzerReferences,
                supportedDiagnosticIds: Options.SupportedDiagnostics,
                ignoredDiagnosticIds: Options.IgnoredDiagnostics,
                ignoredCompilerDiagnosticIds: Options.IgnoredCompilerDiagnostics,
                ignoredProjectNames: Options.IgnoredProjects,
                language: Options.Language,
                batchSize: Options.BatchSize,
                format: Options.Format);

            IEnumerable<string> analyzerAssemblies = Options.AnalyzerAssemblies;

            if (Options.UseRoslynatorAnalyzers)
                analyzerAssemblies = analyzerAssemblies.Concat(RoslynatorAnalyzerAssemblies);

            return await FixAsync(projectOrSolution, analyzerAssemblies, codeFixerOptions, cancellationToken);
        }

        internal static async Task<CommandResult> FixAsync(
            ProjectOrSolution projectOrSolution,
            IEnumerable<string> analyzerAssemblies,
            CodeFixerOptions codeFixerOptions,
            CancellationToken cancellationToken = default)
        {
            if (projectOrSolution.IsProject)
            {
                Project project = projectOrSolution.AsProject();

                Solution solution = project.Solution;

                var codeFixer = new CodeFixer(solution, analyzerAssemblies: analyzerAssemblies, options: codeFixerOptions);

                WriteLine($"Fix project '{project.Name}'", ConsoleColor.Cyan);

                await codeFixer.FixProjectAsync(project, cancellationToken);
            }
            else
            {
                Solution solution = projectOrSolution.AsSolution();

                var codeFixer = new CodeFixer(solution, analyzerAssemblies: analyzerAssemblies, options: codeFixerOptions);

                await codeFixer.FixSolutionAsync(cancellationToken);
            }

            return new CommandResult(true);
        }

        protected override void OperationCanceled(OperationCanceledException ex)
        {
            WriteLine("Fixing was canceled.");
        }
    }
}
