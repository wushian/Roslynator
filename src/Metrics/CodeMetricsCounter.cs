// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Metrics
{
    public abstract class CodeMetricsCounter
    {
        public abstract bool IsComment(SyntaxTrivia trivia);

        public abstract bool IsEndOfLine(SyntaxTrivia trivia);

        protected abstract CodeMetrics CountLines(SyntaxNode node, SourceText sourceText, CodeMetricsOptions options, CancellationToken cancellationToken);

        public static ImmutableDictionary<ProjectId, CodeMetrics> CountPhysicalLinesInParallel(
            IEnumerable<Project> projects,
            CodeMetricsOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return CountLinesInParallel(projects, CodeMetricsCounters.GetPhysicalLinesCounter, options, cancellationToken);
        }

        public static ImmutableDictionary<ProjectId, CodeMetrics> CountLogicalLinesInParallel(
            IEnumerable<Project> projects,
            CodeMetricsOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return CountLinesInParallel(projects, CodeMetricsCounters.GetLogicalLinesCounter, options, cancellationToken);
        }

        private static ImmutableDictionary<ProjectId, CodeMetrics> CountLinesInParallel(
            IEnumerable<Project> projects,
            Func<string, CodeMetricsCounter> counterFactory,
            CodeMetricsOptions options = null,
            CancellationToken cancellationToken = default)
        {
            var metrics = new ConcurrentBag<(ProjectId projectId, CodeMetrics metrics)>();

            Parallel.ForEach(projects, project =>
            {
                CodeMetricsCounter counter = counterFactory(project.Language);

                CodeMetrics projectMetrics = (counter != null)
                    ? counter.CountLinesAsync(project, options, cancellationToken).Result
                    : CodeMetrics.NotAvailable;

                metrics.Add((project.Id, metrics: projectMetrics));
            });

            return metrics.ToImmutableDictionary(f => f.projectId, f => f.metrics);
        }

        public static Task<ImmutableDictionary<ProjectId, CodeMetrics>> CountPhysicalLinesAsync(
            IEnumerable<Project> projects,
            CodeMetricsOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return CountLinesAsync(projects, CodeMetricsCounters.GetPhysicalLinesCounter, options, cancellationToken);
        }

        public static Task<ImmutableDictionary<ProjectId, CodeMetrics>> CountLogicalLinesAsync(
            IEnumerable<Project> projects,
            CodeMetricsOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return CountLinesAsync(projects, CodeMetricsCounters.GetLogicalLinesCounter, options, cancellationToken);
        }

        private static async Task<ImmutableDictionary<ProjectId, CodeMetrics>> CountLinesAsync(
            IEnumerable<Project> projects,
            Func<string, CodeMetricsCounter> counterFactory,
            CodeMetricsOptions options = null,
            CancellationToken cancellationToken = default)
        {
            ImmutableDictionary<ProjectId, CodeMetrics>.Builder builder = ImmutableDictionary.CreateBuilder<ProjectId, CodeMetrics>();

            foreach (Project project in projects)
            {
                CodeMetricsCounter counter = counterFactory(project.Language);

                CodeMetrics projectMetrics = (counter != null)
                    ? await counter.CountLinesAsync(project, options, cancellationToken).ConfigureAwait(false)
                    : CodeMetrics.NotAvailable;

                builder.Add(project.Id, projectMetrics);
            }

            return builder.ToImmutableDictionary();
        }

        public async Task<CodeMetrics> CountLinesAsync(
            Project project,
            CodeMetricsOptions options = null,
            CancellationToken cancellationToken = default)
        {
            CodeMetrics metrics = default;

            foreach (Document document in project.Documents)
            {
                if (!document.SupportsSyntaxTree)
                    continue;

                CodeMetrics documentMetrics = await CountLinesAsync(document, options, cancellationToken).ConfigureAwait(false);

                metrics = metrics.Add(documentMetrics);
            }

            return metrics;
        }

        public async Task<CodeMetrics> CountLinesAsync(
            Document document,
            CodeMetricsOptions options = null,
            CancellationToken cancellationToken = default)
        {
            SyntaxTree tree = await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);

            if (tree == null)
                return default;

            if (!options.IncludeGenerated
                && GeneratedCodeUtility.IsGeneratedCode(tree, IsComment, cancellationToken))
            {
                return default;
            }

            SyntaxNode root = await tree.GetRootAsync(cancellationToken).ConfigureAwait(false);

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            return CountLines(root, sourceText, options, cancellationToken);
        }

        private protected int CountWhiteSpaceLines(SyntaxNode root, SourceText sourceText)
        {
            int whiteSpaceLineCount = 0;

            foreach (TextLine line in sourceText.Lines)
            {
                if (line.IsEmptyOrWhiteSpace())
                {
                    if (line.End == sourceText.Length
                        || IsEndOfLine(root.FindTrivia(line.End)))
                    {
                        whiteSpaceLineCount++;
                    }
                }
            }

            return whiteSpaceLineCount;
        }
    }
}
