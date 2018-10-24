// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Metrics;
using static Roslynator.ConsoleHelpers;

namespace Roslynator.CommandLine
{
    internal class LogicalLinesOfCodeCommandExecutor : AbstractLinesOfCodeCommandExecutor
    {
        public LogicalLinesOfCodeCommandExecutor(LogicalLinesOfCodeCommandLineOptions options)
        {
            Options = options;
        }

        public LogicalLinesOfCodeCommandLineOptions Options { get; }

        public override async Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
        {
            var codeMetricsOptions = new CodeMetricsOptions(includeGenerated: Options.IncludeGenerated);

            if (projectOrSolution.IsProject)
            {
                Project project = projectOrSolution.AsProject();

                WriteLine($"Count logical lines for '{project.FilePath}'", ConsoleColor.Cyan);

                CodeMetricsCounter counter = CodeMetricsCounters.GetLogicalLinesCounter(project.Language);

                if (counter != null)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    CodeMetrics metrics = await counter.CountLinesAsync(project, codeMetricsOptions, cancellationToken);

                    stopwatch.Stop();

                    WriteLine();

                    WriteMetrics(
                        metrics.CodeLineCount,
                        metrics.WhiteSpaceLineCount,
                        metrics.CommentLineCount,
                        metrics.PreprocessorDirectiveLineCount,
                        metrics.TotalLineCount);

                    WriteLine();
                    WriteLine($"Done counting logical lines for '{project.FilePath}' {stopwatch.Elapsed:mm\\:ss\\.ff}", ConsoleColor.Green);
                }
                else
                {
                    WriteLine($"Cannot count logical lines for language '{project.Language}'", ConsoleColor.Yellow);
                }
            }
            else
            {
                Solution solution = projectOrSolution.AsSolution();

                WriteLine($"Count logical lines for solution '{solution.FilePath}'", ConsoleColor.Cyan);

                IEnumerable<Project> projects = FilterProjects(solution, Options.IgnoredProjects, Options.Language);

                Stopwatch stopwatch = Stopwatch.StartNew();

                ImmutableDictionary<ProjectId, CodeMetrics> projectsMetrics = CodeMetricsCounter.CountLogicalLinesInParallel(projects, codeMetricsOptions, cancellationToken);

                stopwatch.Stop();

                WriteLine();
                WriteLine("Logical lines of code by project:");

                WriteLinesOfCode(solution, projectsMetrics);

                WriteLine();
                WriteLine("Summary:");

                WriteMetrics(
                    projectsMetrics.Sum(f => f.Value.CodeLineCount),
                    projectsMetrics.Sum(f => f.Value.WhiteSpaceLineCount),
                    projectsMetrics.Sum(f => f.Value.CommentLineCount),
                    projectsMetrics.Sum(f => f.Value.PreprocessorDirectiveLineCount),
                    projectsMetrics.Sum(f => f.Value.TotalLineCount));

                WriteLine();
                WriteLine($"Done counting logical lines for solution '{solution.FilePath}' {stopwatch.Elapsed:mm\\:ss\\.ff}", ConsoleColor.Green);
            }

            return new CommandResult(true);
        }

        private static void WriteMetrics(
            int totalCodeLineCount,
            int totalWhiteSpaceLineCount,
            int totalCommentLineCount,
            int totalPreprocessorDirectiveLineCount,
            int totalLineCount)
        {
            string totalCodeLines = totalCodeLineCount.ToString("n0");
            string totalWhiteSpaceLines = totalWhiteSpaceLineCount.ToString("n0");
            string totalCommentLines = totalCommentLineCount.ToString("n0");
            string totalPreprocessorDirectiveLines = totalPreprocessorDirectiveLineCount.ToString("n0");
            string totalLines = totalLineCount.ToString("n0");

            int maxDigits = Math.Max(totalCodeLines.Length,
                Math.Max(totalWhiteSpaceLines.Length,
                    Math.Max(totalCommentLines.Length,
                        Math.Max(totalPreprocessorDirectiveLines.Length, totalLines.Length))));

            WriteLine($"{totalCodeLines.PadLeft(maxDigits)} {totalCodeLineCount / (double)totalLineCount,4:P0} logical lines of code");
            WriteLine($"{totalWhiteSpaceLines.PadLeft(maxDigits)} {totalWhiteSpaceLineCount / (double)totalLineCount,4:P0} white-space lines");
            WriteLine($"{totalCommentLines.PadLeft(maxDigits)} {totalCommentLineCount / (double)totalLineCount,4:P0} comment lines");
            WriteLine($"{totalPreprocessorDirectiveLines.PadLeft(maxDigits)} {totalPreprocessorDirectiveLineCount / (double)totalLineCount,4:P0} preprocessor directive lines");
            WriteLine($"{totalLines.PadLeft(maxDigits)} {totalLineCount / (double)totalLineCount,4:P0} total lines");
        }

        protected override void OperationCanceled(OperationCanceledException ex)
        {
            WriteLine("Logical lines counting was canceled.");
        }
    }
}
