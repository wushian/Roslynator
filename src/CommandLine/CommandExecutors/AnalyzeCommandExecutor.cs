// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Analysis;
using static Roslynator.ConsoleHelpers;

namespace Roslynator.CommandLine
{
    internal class AnalyzeCommandExecutor : MSBuildWorkspaceCommandExecutor
    {
        public AnalyzeCommandExecutor(AnalyzeCommandLineOptions options, DiagnosticSeverity minimalSeverity)
        {
            Options = options;
            MinimalSeverity = minimalSeverity;
        }

        public AnalyzeCommandLineOptions Options { get; }

        public DiagnosticSeverity MinimalSeverity { get; }

        public override async Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
        {
            AssemblyResolver.Register();

            var codeAnalyzerOptions = new CodeAnalyzerOptions(
                ignoreAnalyzerReferences: Options.IgnoreAnalyzerReferences,
                ignoreCompilerDiagnostics: Options.IgnoreCompilerDiagnostics,
                reportFadeDiagnostics: Options.ReportFadeDiagnostics,
                reportSuppressedDiagnostics: Options.ReportSuppressedDiagnostics,
                executionTime: Options.ExecutionTime,
                minimalSeverity: MinimalSeverity,
                ignoredDiagnosticIds: Options.IgnoredDiagnostics,
                ignoredProjectNames: Options.IgnoredProjects,
                language: CommandLineHelpers.GetLanguageName(Options.Language),
                cultureName: Options.CultureName);

            CultureInfo culture = (Options.CultureName != null) ? CultureInfo.GetCultureInfo(Options.CultureName) : null;

            var codeAnalyzer = new CodeAnalyzer(analyzerAssemblies: Options.AnalyzerAssemblies, formatProvider: culture, options: codeAnalyzerOptions);

            if (projectOrSolution.IsProject)
            {
                Project project = projectOrSolution.AsProject();

                WriteLine($"Analyze project '{project.Name}'");

                await codeAnalyzer.AnalyzeProjectAsync(project, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                Solution solution = projectOrSolution.AsSolution();

                await codeAnalyzer.AnalyzeSolutionAsync(solution, cancellationToken).ConfigureAwait(false);
            }

            return new CommandResult(true);
        }

        protected override void OperationCanceled(OperationCanceledException ex)
        {
            WriteLine("Analysis was canceled.");
        }
    }
}
