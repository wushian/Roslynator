// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CodeMetrics
{
    internal static class WorkspaceCodeMetrics
    {
        public static ImmutableDictionary<ProjectId, CodeMetricsInfo> CountLinesInParallel(
            IEnumerable<Project> projects,
            Func<string, CodeMetricsCounter> counterFactory,
            CodeMetricsOptions options = null,
            CancellationToken cancellationToken = default)
        {
            var metrics = new ConcurrentBag<(ProjectId projectId, CodeMetricsInfo metrics)>();

            Parallel.ForEach(projects, project =>
            {
                CodeMetricsCounter counter = counterFactory(project.Language);

                CodeMetricsInfo projectMetrics = (counter != null)
                    ? CountLinesAsync(project, counter, options, cancellationToken).Result
                    : CodeMetricsInfo.NotAvailable;

                metrics.Add((project.Id, metrics: projectMetrics));
            });

            return metrics.ToImmutableDictionary(f => f.projectId, f => f.metrics);
        }

        public static async Task<ImmutableDictionary<ProjectId, CodeMetricsInfo>> CountLinesAsync(
            IEnumerable<Project> projects,
            Func<string, CodeMetricsCounter> counterFactory,
            CodeMetricsOptions options = null,
            CancellationToken cancellationToken = default)
        {
            ImmutableDictionary<ProjectId, CodeMetricsInfo>.Builder builder = ImmutableDictionary.CreateBuilder<ProjectId, CodeMetricsInfo>();

            foreach (Project project in projects)
            {
                CodeMetricsCounter counter = counterFactory(project.Language);

                CodeMetricsInfo projectMetrics = (counter != null)
                    ? await CountLinesAsync(project, counter, options, cancellationToken).ConfigureAwait(false)
                    : CodeMetricsInfo.NotAvailable;

                builder.Add(project.Id, projectMetrics);
            }

            return builder.ToImmutableDictionary();
        }

        public static async Task<CodeMetricsInfo> CountLinesAsync(
            Project project,
            CodeMetricsCounter counter,
            CodeMetricsOptions options = null,
            CancellationToken cancellationToken = default)
        {
            CodeMetricsInfo metrics = default;

            foreach (Document document in project.Documents)
            {
                if (!document.SupportsSyntaxTree)
                    continue;

                CodeMetricsInfo documentMetrics = await CountLinesAsync(document, counter, options, cancellationToken).ConfigureAwait(false);

                metrics = metrics.Add(documentMetrics);
            }

            return metrics;
        }

        public static async Task<CodeMetricsInfo> CountLinesAsync(
            Document document,
            CodeMetricsCounter counter,
            CodeMetricsOptions options = null,
            CancellationToken cancellationToken = default)
        {
            SyntaxTree tree = await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);

            if (tree == null)
                return default;

            if (!options.IncludeGeneratedCode
                && GeneratedCodeUtility.IsGeneratedCode(tree, counter.SyntaxFacts.IsComment, cancellationToken))
            {
                return default;
            }

            SyntaxNode root = await tree.GetRootAsync(cancellationToken).ConfigureAwait(false);

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            return counter.CountLines(root, sourceText, options, cancellationToken);
        }
    }
}
