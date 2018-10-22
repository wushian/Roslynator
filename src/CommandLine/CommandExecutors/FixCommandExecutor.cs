// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CodeFixes;
using static Roslynator.ConsoleHelpers;

namespace Roslynator.CommandLine
{
    internal class FixCommandExecutor : MSBuildWorkspaceCommandExecutor
    {
        public FixCommandExecutor(FixCommandLineOptions options, DiagnosticSeverity minimalSeverity)
        {
            Options = options;
            MinimalSeverity = minimalSeverity;
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
                ignoredDiagnosticIds: Options.IgnoredDiagnostics,
                ignoredCompilerDiagnosticIds: Options.IgnoredCompilerDiagnostics,
                ignoredProjectNames: Options.IgnoredProjects,
                language: Options.Language,
                batchSize: Options.BatchSize,
                format: Options.Format);

            if (projectOrSolution.IsProject)
            {
                Project project = projectOrSolution.AsProject();

                Solution solution = project.Solution;

                var codeFixer = new CodeFixer(solution, analyzerAssemblies: Options.AnalyzerAssemblies, options: codeFixerOptions);

                WriteLine($"Fix project '{project.Name}'", ConsoleColor.Cyan);

                await codeFixer.FixProjectAsync(project, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                Solution solution = projectOrSolution.AsSolution();

                var codeFixer = new CodeFixer(solution, analyzerAssemblies: Options.AnalyzerAssemblies, options: codeFixerOptions);

                await codeFixer.FixSolutionAsync(cancellationToken).ConfigureAwait(false);
            }

            return new CommandResult(true);
        }

        protected override void OperationCanceled(OperationCanceledException ex)
        {
            WriteLine("Fixing was canceled.");
        }
    }
}
