// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal abstract class MSBuildWorkspaceCommandExecutor
    {
        public abstract Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default);

        public async Task<CommandResult> ExecuteAsync(string path, string msbuildPath = null, IEnumerable<string> properties = null)
        {
            MSBuildWorkspace workspace = null;

            try
            {
                workspace = CreateMSBuildWorkspace(msbuildPath, properties);

                if (workspace == null)
                    return CommandResult.Fail;

                workspace.WorkspaceFailed += WorkspaceFailed;

                bool isSolution = string.Equals(Path.GetExtension(path), ".sln", StringComparison.OrdinalIgnoreCase);

                WriteLine($"Load {((isSolution) ? "solution" : "project")} '{path}'", ConsoleColor.Cyan, Verbosity.Minimal);

                try
                {
                    var cts = new CancellationTokenSource();
                    Console.CancelKeyPress += (sender, e) =>
                    {
                        e.Cancel = true;
                        cts.Cancel();
                    };

                    CancellationToken cancellationToken = cts.Token;

                    ProjectOrSolution projectOrSolution;

                    try
                    {
                        if (isSolution)
                        {
                            projectOrSolution = await workspace.OpenSolutionAsync(path, ConsoleProgressReporter.Instance, cancellationToken);
                        }
                        else
                        {
                            projectOrSolution = await workspace.OpenProjectAsync(path, ConsoleProgressReporter.Instance, cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex is FileNotFoundException
                            || ex is InvalidOperationException)
                        {
                            WriteLine(ex.ToString(), ConsoleColor.Red, Verbosity.Minimal);
                            return CommandResult.Fail;
                        }
                        else
                        {
                            throw;
                        }
                    }

                    WriteLine($"Done loading {((isSolution) ? "solution" : "project")} '{path}'", ConsoleColor.Green, Verbosity.Minimal);

                    return await ExecuteAsync(projectOrSolution, cancellationToken);
                }
                catch (OperationCanceledException ex)
                {
                    OperationCanceled(ex);
                }
            }
            finally
            {
                workspace?.Dispose();
            }

            return CommandResult.Fail;
        }

        protected virtual void OperationCanceled(OperationCanceledException ex)
        {
            WriteLine("Operation was canceled.", Verbosity.Quiet);
        }

        protected virtual void WorkspaceFailed(object sender, WorkspaceDiagnosticEventArgs e)
        {
            WriteLine(e.Diagnostic.Message, ConsoleColor.Yellow, Verbosity.Normal);
        }

        private static MSBuildWorkspace CreateMSBuildWorkspace(string msbuildPath, IEnumerable<string> rawProperties)
        {
            if (msbuildPath != null)
            {
                MSBuildLocator.RegisterMSBuildPath(msbuildPath);
            }
            else
            {
                VisualStudioInstance instance = MSBuildLocator.QueryVisualStudioInstances()
                    .OrderBy(f => f.Version)
                    .LastOrDefault();

                if (instance == null)
                {
                    WriteLine("MSBuild location not found. Use option '--msbuild-path' to specify MSBuild location", ConsoleColor.Red, Verbosity.Quiet);
                    return null;
                }

                WriteLine($"MSBuild location is '{instance.MSBuildPath}'", Verbosity.Detailed);

                MSBuildLocator.RegisterInstance(instance);
            }

            if (!CommandLineHelpers.TryParseMSBuildProperties(rawProperties, out Dictionary<string, string> properties))
                return null;

            if (properties == null)
                properties = new Dictionary<string, string>();

            // https://github.com/Microsoft/MSBuildLocator/issues/16
            if (!properties.ContainsKey("AlwaysCompileMarkupFilesInSeparateDomain"))
                properties["AlwaysCompileMarkupFilesInSeparateDomain"] = bool.FalseString;

            return MSBuildWorkspace.Create(properties);
        }

        private protected static IEnumerable<Project> FilterProjects(
            Solution solution,
            MSBuildCommandLineOptions options)
        {
            ImmutableHashSet<string> projectNames = (options.Projects != null)
                ? options.Projects.ToImmutableHashSet()
                : ImmutableHashSet<string>.Empty;

            ImmutableHashSet<string> ignoredProjectNames = (options.IgnoredProjects != null)
                ? options.IgnoredProjects.ToImmutableHashSet()
                : ImmutableHashSet<string>.Empty;

            Workspace workspace = solution.Workspace;

            foreach (ProjectId projectId in solution.ProjectIds)
            {
                Project project = workspace.CurrentSolution.GetProject(projectId);

                if (SyntaxFactsService.IsSupportedLanguage(project.Language)
                    && (options.Language == null || options.Language == project.Language)
                    && ((projectNames.Count > 0) ? projectNames.Contains(project.Name) : !ignoredProjectNames.Contains(project.Name)))
                {
                    yield return project;
                }
                else
                {
                    WriteLine($"  Skip '{project.Name}'", ConsoleColor.DarkGray, Verbosity.Normal);
                }
            }
        }

        private class ConsoleProgressReporter : IProgress<ProjectLoadProgress>
        {
            public static ConsoleProgressReporter Instance { get; } = new ConsoleProgressReporter();

            public void Report(ProjectLoadProgress value)
            {
                string text = Path.GetFileName(value.FilePath);

                if (value.TargetFramework != null)
                    text += $" ({value.TargetFramework})";

                WriteLine($"  {value.Operation,-9} {value.ElapsedTime:mm\\:ss\\.ff}  {text}", Verbosity.Detailed);
            }
        }
    }
}
