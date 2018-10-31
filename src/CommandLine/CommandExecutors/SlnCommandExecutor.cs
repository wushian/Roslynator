// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal class SlnCommandExecutor : MSBuildWorkspaceCommandExecutor
    {
        public SlnCommandExecutor(SlnCommandLineOptions options, string language) : base(language)
        {
            Options = options;
        }

        public SlnCommandLineOptions Options { get; }

        public override Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new CommandResult(success: true));
        }

        protected override async Task<ProjectOrSolution> OpenProjectOrSolutionAsync(string path, MSBuildWorkspace workspace, IProgress<ProjectLoadProgress> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!string.Equals(Path.GetExtension(path), ".sln", StringComparison.OrdinalIgnoreCase))
            {
                WriteLine("Specified file is not a solution file.", ConsoleColor.Red, Verbosity.Minimal);
                return default;
            }

            var consoleProgress = new ConsoleProgressReporter(shouldSaveProgress: true);

            ProjectOrSolution projectOrSolution = await base.OpenProjectOrSolutionAsync(path, workspace, consoleProgress, cancellationToken);

            if (projectOrSolution == default)
            {
                return projectOrSolution;
            }

            Solution solution = projectOrSolution.AsSolution();

            string solutionDirectory = Path.GetDirectoryName(solution.FilePath);

            Dictionary<string, ImmutableArray<Project>> projectsByPath = solution.Projects
                .GroupBy(f => f.FilePath)
                .ToDictionary(f => f.Key, f => f.ToImmutableArray());

            IReadOnlyList<ProjectId> projectIds = solution.ProjectIds;

            Dictionary<string, List<string>> projects = consoleProgress.Projects;

            int nameMaxLength = projects.Max(f => Path.GetFileNameWithoutExtension(f.Key).Length);

            int targetFrameworksMaxLength = projects.Max(f =>
            {
                List<string> frameworks = f.Value;

                return (frameworks != null) ? $"({string.Join(", ", frameworks)})".Length : 0;
            });

            bool anyHasTargetFrameworks = projects.Any(f => f.Value != null);

            WriteLine();
            WriteLine($"{projects.Count} {((projects.Count == 1) ? "project" : "projects")} found in solution '{Path.GetFileNameWithoutExtension(solution.FilePath)}' [{solution.FilePath}]", ConsoleColor.Green, Verbosity.Minimal);

            foreach (KeyValuePair<string, List<string>> kvp in projects
                .OrderBy(f => Path.GetFileName(f.Key)))
            {
                string projectPath = kvp.Key;
                List<string> targetFrameworks = kvp.Value;

                Project project = projectsByPath[projectPath][0];

                string projectName = Path.GetFileNameWithoutExtension(projectPath);

                Write($"  {projectName.PadRight(nameMaxLength)}  {project.Language}", Verbosity.Normal);

                if (anyHasTargetFrameworks)
                    Write("  ", Verbosity.Normal);

                if (targetFrameworks != null)
                {
                    string targetFrameworksText = $"({string.Join(", ", targetFrameworks.OrderBy(f => f))})";
                    Write(targetFrameworksText.PadRight(targetFrameworksMaxLength), Verbosity.Normal);
                }
                else
                {
                    Write(new string(' ', targetFrameworksMaxLength), Verbosity.Normal);
                }

                WriteLine($"  {PathUtilities.TrimStart(projectPath, solutionDirectory)}", Verbosity.Normal);
            }

            WriteLine();

            return projectOrSolution;
        }
    }
}
