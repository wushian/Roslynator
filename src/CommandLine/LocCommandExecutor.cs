// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Metrics;
using static Roslynator.CodeFixes.ConsoleHelpers;

namespace Roslynator.CommandLine
{
    internal class LocCommandExecutor : MSBuildWorkspaceCommandExecutor
    {
        public LocCommandExecutor(LocCommandLineOptions options)
        {
            Options = options;
        }

        public LocCommandLineOptions Options { get; }

        public override async Task<CommandResult> ExecuteAsync(Solution solution, CancellationToken cancellationToken = default)
        {
            var codeMetricsOptions = new CodeMetricsOptions(
                includeGenerated: Options.IncludeGenerated,
                includeWhiteSpace: Options.IncludeWhiteSpace,
                includeComments: Options.IncludeComments,
                includePreprocessorDirectives: Options.IncludePreprocessorDirectives,
                ignoreBlockBoundary: Options.IgnoreBlockBoundary);

            WriteLine($"Count metrics for solution '{solution.FilePath}'", ConsoleColor.Cyan);

            var projectsMetrics = new List<(Project project, CodeMetrics metrics)>();

            ImmutableHashSet<string> ignoredProjectNames = (Options.IgnoredProjects.Any())
                ? ImmutableHashSet.CreateRange(Options.IgnoredProjects)
                : ImmutableHashSet<string>.Empty;

            foreach (Project project in solution.Projects)
            {
                if (ignoredProjectNames.Contains(project.Name))
                    continue;

                WriteLine($"  Count metrics for project '{project.Name}'");

                projectsMetrics.Add((project, await CodeMetricsCounter.CountLinesAsync(project, codeMetricsOptions, cancellationToken).ConfigureAwait(false)));
            }

            WriteLine($"Done counting metrics for solution '{solution.FilePath}'", ConsoleColor.Green);

            WriteLine();
            WriteLine("Lines of code by project:");

            int maxDigits = projectsMetrics.Max(f => f.metrics.CodeLineCount).ToString("n0").Length;

            int maxNameLength = projectsMetrics.Max(f => f.project.Name.Length);

            foreach ((Project project, CodeMetrics metrics) in projectsMetrics.OrderByDescending(f => f.metrics.CodeLineCount))
            {
                WriteLine($"{metrics.CodeLineCount.ToString("n0").PadLeft(maxDigits)} {project.Name.PadRight(maxNameLength)} {project.Language}");
            }

            WriteLine();
            WriteLine("Solution metrics:");

            int totalCodeLineCount = projectsMetrics.Sum(f => f.metrics.CodeLineCount);
            int totalBlockBoundaryLineCount = projectsMetrics.Sum(f => f.metrics.BlockBoundaryLineCount);
            int totalWhiteSpaceLineCount = projectsMetrics.Sum(f => f.metrics.WhiteSpaceLineCount);
            int totalCommentLineCount = projectsMetrics.Sum(f => f.metrics.CommentLineCount);
            int totalPreprocessorDirectiveLineCount = projectsMetrics.Sum(f => f.metrics.PreprocessorDirectiveLineCount);
            int totalLineCount = projectsMetrics.Sum(f => f.metrics.TotalLineCount);

            string totalCodeLines = totalCodeLineCount.ToString("n0");
            string totalBlockBoundaryLines = totalBlockBoundaryLineCount.ToString("n0");
            string totalWhiteSpaceLines = totalWhiteSpaceLineCount.ToString("n0");
            string totalCommentLines = totalCommentLineCount.ToString("n0");
            string totalPreprocessorDirectiveLines = totalPreprocessorDirectiveLineCount.ToString("n0");
            string totalLines = totalLineCount.ToString("n0");

            maxDigits = Math.Max(totalCodeLines.Length,
                Math.Max(totalBlockBoundaryLines.Length,
                    Math.Max(totalWhiteSpaceLines.Length,
                        Math.Max(totalCommentLines.Length,
                            Math.Max(totalPreprocessorDirectiveLines.Length, totalLines.Length)))));

            if (Options.IgnoreBlockBoundary
                || !Options.IncludeWhiteSpace
                || !Options.IncludeComments
                || !Options.IncludePreprocessorDirectives)
            {
                WriteLine($"{totalCodeLines.PadLeft(maxDigits)} {totalCodeLineCount / (double)totalLineCount,4:P0} lines of code");
            }
            else
            {
                WriteLine($"{totalCodeLines.PadLeft(maxDigits)} lines of code");
            }

            if (Options.IgnoreBlockBoundary)
                WriteLine($"{totalBlockBoundaryLines.PadLeft(maxDigits)} {totalBlockBoundaryLineCount / (double)totalLineCount,4:P0} block boundary lines");

            if (!Options.IncludeWhiteSpace)
                WriteLine($"{totalWhiteSpaceLines.PadLeft(maxDigits)} {totalWhiteSpaceLineCount / (double)totalLineCount,4:P0} white-space lines");

            if (!Options.IncludeComments)
                WriteLine($"{totalCommentLines.PadLeft(maxDigits)} {totalCommentLineCount / (double)totalLineCount,4:P0} comment lines");

            if (!Options.IncludePreprocessorDirectives)
                WriteLine($"{totalPreprocessorDirectiveLines.PadLeft(maxDigits)} {totalPreprocessorDirectiveLineCount / (double)totalLineCount,4:P0} preprocessor directive lines");

            WriteLine($"{totalLines.PadLeft(maxDigits)} {totalLineCount / (double)totalLineCount,4:P0} total lines");

            WriteLine();

            return new CommandResult(true);
        }

        protected override void OperationCanceled(OperationCanceledException ex)
        {
            WriteLine("Metrics counting was canceled.");
        }
    }
}
