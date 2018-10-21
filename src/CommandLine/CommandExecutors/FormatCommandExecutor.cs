// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting;
using static Roslynator.ConsoleHelpers;

namespace Roslynator.CommandLine
{
    internal class FormatCommandExecutor : MSBuildWorkspaceCommandExecutor
    {
        public FormatCommandExecutor(FormatCommandLineOptions options)
        {
            Options = options;
        }

        public FormatCommandLineOptions Options { get; }

        public override async Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
        {
            Workspace workspace = projectOrSolution.Workspace;

            if (projectOrSolution.IsProject)
            {
                Project project = projectOrSolution.AsProject();

                WriteLine($"  Format '{project.Name}'");

                Project newProject = await CodeFormatter.FormatProjectAsync(project, cancellationToken).ConfigureAwait(false);

                bool success = workspace.TryApplyChanges(newProject.Solution);

                Debug.Assert(success, "Cannot apply changes to a solution.");
            }
            else
            {
                Solution solution = projectOrSolution.AsSolution();

                WriteLine($"Format solution '{projectOrSolution.FilePath}'", ConsoleColor.Cyan);

                foreach (Project project in FilterProjects(solution, Options.IgnoredProjects, Options.Language))
                {
                    WriteLine($"  Format '{project.Name}'");

                    Project newProject = await CodeFormatter.FormatProjectAsync(project, cancellationToken).ConfigureAwait(false);

                    bool success = workspace.TryApplyChanges(newProject.Solution);

                    Debug.Assert(success, "Cannot apply changes to a solution.");
                }

                WriteLine($"Done formatting solution '{solution.FilePath}'", ConsoleColor.Green);
            }

            return new CommandResult(true);
        }

        protected override void OperationCanceled(OperationCanceledException ex)
        {
            WriteLine("Formatting was canceled.");
        }
    }
}
