// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Metrics;
using static Roslynator.ConsoleHelpers;

namespace Roslynator.CommandLine
{
    internal class LogicalLinesOfCodeCommandExecutor : MSBuildWorkspaceCommandExecutor
    {
        public LogicalLinesOfCodeCommandExecutor(LogicalLinesOfCodeCommandLineOptions options)
        {
            Options = options;
        }

        public LogicalLinesOfCodeCommandLineOptions Options { get; }

        public override async Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
        {
            var codeMetricsOptions = new CodeMetricsOptions(
                includeGenerated: Options.IncludeGenerated,
                includeWhiteSpace: Options.IncludeWhiteSpace,
                includeComments: Options.IncludeComments,
                includePreprocessorDirectives: Options.IncludePreprocessorDirectives);

            if (projectOrSolution.IsProject)
            {
                Project project = projectOrSolution.AsProject();

                WriteLine($"Count logical lines for '{project.FilePath}'", ConsoleColor.Cyan);

                CodeMetricsCounter counter = CodeMetricsCounter.GetLogicalLinesCounter(project.Language);

                if (counter != null)
                {
                    CodeMetrics metrics = await counter.CountLinesAsync(project, codeMetricsOptions, cancellationToken).ConfigureAwait(false);

                    WriteLine($"Done counting logical lines for '{project.FilePath}'", ConsoleColor.Green);

                    WriteLine();

                    WriteMetrics(
                        metrics.CodeLineCount,
                        metrics.BlockBoundaryLineCount,
                        metrics.WhiteSpaceLineCount,
                        metrics.CommentLineCount,
                        metrics.PreprocessorDirectiveLineCount,
                        metrics.TotalLineCount);

                    WriteLine();
                }
            }
            else
            {
                Solution solution = projectOrSolution.AsSolution();

                WriteLine($"Count logical lines for solution '{solution.FilePath}'", ConsoleColor.Cyan);

                var projectsMetrics = new ConcurrentBag<(Project project, CodeMetrics metrics)>();

                Stopwatch stopwatch = Stopwatch.StartNew();

                Parallel.ForEach(FilterProjects(solution, Options.IgnoredProjects, Options.Language), project =>
                {
                    WriteLine($"  Analyze '{project.Name}'");

                    CodeMetricsCounter counter = CodeMetricsCounter.GetLogicalLinesCounter(project.Language);

                    if (counter != null)
                    {
                        projectsMetrics.Add((project, counter.CountLinesAsync(project, codeMetricsOptions, cancellationToken).Result));
                    }
                });

                WriteLine($"Done counting logical lines for solution '{solution.FilePath}' {stopwatch.Elapsed:mm\\:ss\\.ff}", ConsoleColor.Green);

                WriteLine();
                WriteLine("Logical lines of code by project:");

                int maxDigits = projectsMetrics.Max(f => f.metrics.CodeLineCount).ToString("n0").Length;

                int maxNameLength = projectsMetrics.Max(f => f.project.Name.Length);

                foreach ((Project project, CodeMetrics metrics) in projectsMetrics.OrderByDescending(f => f.metrics.CodeLineCount))
                {
                    WriteLine($"{metrics.CodeLineCount.ToString("n0").PadLeft(maxDigits)} {project.Name.PadRight(maxNameLength)} {project.Language}");
                }

                WriteLine();
                WriteLine("Summary:");

                WriteMetrics(
                    projectsMetrics.Sum(f => f.metrics.CodeLineCount),
                    projectsMetrics.Sum(f => f.metrics.BlockBoundaryLineCount),
                    projectsMetrics.Sum(f => f.metrics.WhiteSpaceLineCount),
                    projectsMetrics.Sum(f => f.metrics.CommentLineCount),
                    projectsMetrics.Sum(f => f.metrics.PreprocessorDirectiveLineCount),
                    projectsMetrics.Sum(f => f.metrics.TotalLineCount));

                WriteLine();
            }

            return new CommandResult(true);
        }

        private void WriteMetrics(int totalCodeLineCount, int totalBlockBoundaryLineCount, int totalWhiteSpaceLineCount, int totalCommentLineCount, int totalPreprocessorDirectiveLineCount, int totalLineCount)
        {
            string totalCodeLines = totalCodeLineCount.ToString("n0");
            string totalBlockBoundaryLines = totalBlockBoundaryLineCount.ToString("n0");
            string totalWhiteSpaceLines = totalWhiteSpaceLineCount.ToString("n0");
            string totalCommentLines = totalCommentLineCount.ToString("n0");
            string totalPreprocessorDirectiveLines = totalPreprocessorDirectiveLineCount.ToString("n0");
            string totalLines = totalLineCount.ToString("n0");

            int maxDigits = Math.Max(totalCodeLines.Length,
                Math.Max(totalBlockBoundaryLines.Length,
                    Math.Max(totalWhiteSpaceLines.Length,
                        Math.Max(totalCommentLines.Length,
                            Math.Max(totalPreprocessorDirectiveLines.Length, totalLines.Length)))));

            if (!Options.IncludeWhiteSpace
                || !Options.IncludeComments
                || !Options.IncludePreprocessorDirectives)
            {
                WriteLine($"{totalCodeLines.PadLeft(maxDigits)} {totalCodeLineCount / (double)totalLineCount,4:P0} logical lines of code");
            }
            else
            {
                WriteLine($"{totalCodeLines.PadLeft(maxDigits)} logical lines of code");
            }

            if (!Options.IncludeWhiteSpace)
                WriteLine($"{totalWhiteSpaceLines.PadLeft(maxDigits)} {totalWhiteSpaceLineCount / (double)totalLineCount,4:P0} white-space lines");

            if (!Options.IncludeComments)
                WriteLine($"{totalCommentLines.PadLeft(maxDigits)} {totalCommentLineCount / (double)totalLineCount,4:P0} comment lines");

            if (!Options.IncludePreprocessorDirectives)
                WriteLine($"{totalPreprocessorDirectiveLines.PadLeft(maxDigits)} {totalPreprocessorDirectiveLineCount / (double)totalLineCount,4:P0} preprocessor directive lines");

            WriteLine($"{totalLines.PadLeft(maxDigits)} {totalLineCount / (double)totalLineCount,4:P0} total lines");
        }

        protected override void OperationCanceled(OperationCanceledException ex)
        {
            WriteLine("Logical lines counting was canceled.");
        }
    }
}
