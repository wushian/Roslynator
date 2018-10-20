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

        public override async Task<CommandResult> ExecuteAsync(Solution solution, CancellationToken cancellationToken = default)
        {
            Workspace workspace = solution.Workspace;

            WriteLine($"Format solution '{solution.FilePath}'", ConsoleColor.Cyan);

            ImmutableHashSet<string> ignoredProjectNames = (Options.IgnoredProjects.Any())
                ? ImmutableHashSet.CreateRange(Options.IgnoredProjects)
                : ImmutableHashSet<string>.Empty;

            foreach (ProjectId projectId in solution.ProjectIds)
            {
                Project project = workspace.CurrentSolution.GetProject(projectId);

                if (ignoredProjectNames.Contains(project.Name)
                    || (Options.Language != null && Options.Language != project.Language))
                {
                    WriteLine($"  Skip   '{project.Name}'", ConsoleColor.DarkGray);
                    continue;
                }

                WriteLine($"  Format '{project.Name}'");

                Project newProject = await CodeFormatter.FormatAsync(project, cancellationToken).ConfigureAwait(false);

                bool success = workspace.TryApplyChanges(newProject.Solution);

                Debug.Assert(success, "Cannot apply changes to a solution.");
            }

            WriteLine($"Done formatting solution '{solution.FilePath}'", ConsoleColor.Green);

            return new CommandResult(true);
        }

        protected override void OperationCanceled(OperationCanceledException ex)
        {
            WriteLine("Formatting was canceled.");
        }
    }
}
