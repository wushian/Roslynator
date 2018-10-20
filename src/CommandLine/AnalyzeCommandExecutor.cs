// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CodeFixes;
using static Roslynator.CodeFixes.ConsoleHelpers;

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

        public override async Task<CommandResult> ExecuteAsync(Solution solution, CancellationToken cancellationToken = default)
        {
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

            var codeAnalyzer = new CodeAnalyzer(solution, analyzerAssemblies: Options.AnalyzerAssemblies, options: codeAnalyzerOptions);

            await codeAnalyzer.AnalyzeAsync(cancellationToken).ConfigureAwait(false);

            return new CommandResult(true);
        }

        protected override void OperationCanceled(OperationCanceledException ex)
        {
            WriteLine("Analysis was canceled.");
        }
    }
}
