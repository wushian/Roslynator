// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CodeFixes;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal class FixCommandExecutor : MSBuildWorkspaceCommandExecutor
    {
        private static ImmutableHashSet<string> _roslynatorAnalyzerAssemblies;

        public FixCommandExecutor(
            FixCommandLineOptions options,
            DiagnosticSeverity minimalSeverity,
            ImmutableDictionary<string, string> diagnosticFixMap,
            ImmutableDictionary<string, string> diagnosticFixerMap)
        {
            Options = options;
            MinimalSeverity = minimalSeverity;
            DiagnosticFixMap = diagnosticFixMap;
            DiagnosticFixerMap = diagnosticFixerMap;
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

        public ImmutableDictionary<string, string> DiagnosticFixMap { get; }

        public ImmutableDictionary<string, string> DiagnosticFixerMap { get; }

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
                projectNames: Options.Projects,
                ignoredProjectNames: Options.IgnoredProjects,
                diagnosticFixMap: DiagnosticFixMap,
                diagnosticFixerMap: DiagnosticFixerMap,
                fileBanner: Options.FileBanner,
                language: Options.Language,
                batchSize: Options.BatchSize,
                format: Options.Format);

            IEnumerable<string> analyzerAssemblies = Options.AnalyzerAssemblies;

            if (Options.UseRoslynatorAnalyzers)
                analyzerAssemblies = analyzerAssemblies.Concat(RoslynatorAnalyzerAssemblies);

            CultureInfo culture = (Options.CultureName != null) ? CultureInfo.GetCultureInfo(Options.CultureName) : null;

            return await FixAsync(projectOrSolution, analyzerAssemblies, codeFixerOptions, culture, cancellationToken);
        }

        internal static async Task<CommandResult> FixAsync(
            ProjectOrSolution projectOrSolution,
            IEnumerable<string> analyzerAssemblies,
            CodeFixerOptions codeFixerOptions,
            IFormatProvider formatProvider = null,
            CancellationToken cancellationToken = default)
        {
            if (projectOrSolution.IsProject)
            {
                Project project = projectOrSolution.AsProject();

                Solution solution = project.Solution;

                var codeFixer = new CodeFixer(solution, analyzerAssemblies: analyzerAssemblies, formatProvider: formatProvider, options: codeFixerOptions);

                WriteLine($"Fix project '{project.Name}'", ConsoleColor.Cyan, Verbosity.Minimal);

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
            WriteLine("Fixing was canceled.", Verbosity.Quiet);
        }
    }
}
