// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CodeFixes;
using static Roslynator.CodeFixes.ConsoleHelpers;

namespace Roslynator.CommandLine
{
    internal class FixCommandExecutor : MSBuildWorkspaceCommandExecutor
    {
        public FixCommandExecutor(FixCommandLineOptions options)
        {
            Options = options;
        }

        public FixCommandLineOptions Options { get; }

        public override async Task<CommandResult> ExecuteAsync(Solution solution, CancellationToken cancellationToken = default)
        {
            var codeFixerOptions = new CodeFixerOptions(
                ignoreCompilerErrors: Options.IgnoreCompilerErrors,
                ignoreAnalyzerReferences: Options.IgnoreAnalyzerReferences,
                ignoredDiagnosticIds: Options.IgnoredDiagnostics,
                ignoredCompilerDiagnosticIds: Options.IgnoredCompilerDiagnostics,
                ignoredProjectNames: Options.IgnoredProjects,
                batchSize: Options.BatchSize);

            var codeFixer = new CodeFixer(solution.Workspace, analyzerAssemblies: Options.AnalyzerAssemblies, options: codeFixerOptions);

            await codeFixer.FixAsync(cancellationToken).ConfigureAwait(false);

            return new CommandResult(true);
        }

        protected override void OperationCanceled(OperationCanceledException ex)
        {
            WriteLine("Fixing was canceled.");
        }
    }
}
