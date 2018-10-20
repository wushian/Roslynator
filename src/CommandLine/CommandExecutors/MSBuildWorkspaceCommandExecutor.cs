// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using static Roslynator.ConsoleHelpers;

namespace Roslynator.CommandLine
{
    internal abstract class MSBuildWorkspaceCommandExecutor
    {
        public abstract Task<CommandResult> ExecuteAsync(Solution solution, CancellationToken cancellationToken = default);

        public async Task<CommandResult> ExecuteAsync(string solutionPath, string msbuildPath = null, IEnumerable<string> properties = null)
        {
            MSBuildWorkspace workspace = null;

            try
            {
                workspace = CreateMSBuildWorkspace(msbuildPath, properties);

                if (workspace == null)
                    return CommandResult.Fail;

                workspace.WorkspaceFailed += WorkspaceFailed;

                workspace.WorkspaceFailed += (o, e) => WriteLine(e.Diagnostic.Message, ConsoleColor.Yellow);

                WriteLine($"Load solution '{solutionPath}'", ConsoleColor.Cyan);

                try
                {
                    var cts = new CancellationTokenSource();
                    Console.CancelKeyPress += (sender, e) =>
                    {
                        e.Cancel = true;
                        cts.Cancel();
                    };

                    CancellationToken cancellationToken = cts.Token;

                    Solution solution;

                    try
                    {
                        solution = await workspace.OpenSolutionAsync(solutionPath, new ConsoleProgressReporter(), cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        if (ex is FileNotFoundException
                            || ex is InvalidOperationException)
                        {
                            WriteLine(ex.ToString(), ConsoleColor.Red);
                            return CommandResult.Fail;
                        }
                        else
                        {
                            throw;
                        }
                    }

                    WriteLine($"Done loading solution '{solutionPath}'", ConsoleColor.Green);

                    return await ExecuteAsync(solution, cancellationToken).ConfigureAwait(false);
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
            WriteLine("Operation was canceled.");
        }

        protected virtual void WorkspaceFailed(object sender, WorkspaceDiagnosticEventArgs e)
        {
            WriteLine(e.Diagnostic.Message, ConsoleColor.Yellow);
        }

        private static MSBuildWorkspace CreateMSBuildWorkspace(string msbuildPath, IEnumerable<string> properties)
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
                    WriteLine("MSBuild location not found. Use option '--msbuild-path' to specify MSBuild location", ConsoleColor.Red);
                    return null;
                }

                WriteLine($"MSBuild location is '{instance.MSBuildPath}'");

                MSBuildLocator.RegisterInstance(instance);
            }

            if (!CommandLineHelpers.TryParseMSBuildProperties(properties, out Dictionary<string, string> dicProperties))
                return null;

            return MSBuildWorkspace.Create(dicProperties);
        }

        private class ConsoleProgressReporter : IProgress<ProjectLoadProgress>
        {
            public void Report(ProjectLoadProgress value)
            {
                string text = Path.GetFileName(value.FilePath);

                if (value.TargetFramework != null)
                    text += $" ({value.TargetFramework})";

                WriteLine($"  {value.Operation,-9} {value.ElapsedTime:mm\\:ss\\.ff}  {text}");
            }
        }
    }
}
