// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Metrics
{
    public abstract class CodeMetricsCounter
    {
        internal abstract SyntaxFactsService SyntaxFacts { get; }

        protected abstract CodeMetrics CountLines(SyntaxNode node, SourceText sourceText, CodeMetricsOptions options, CancellationToken cancellationToken);

        public static CodeMetricsCounter GetPhysicalLinesCounter(string language)
        {
            switch (language)
            {
                case LanguageNames.CSharp:
                    return CSharp.CSharpPhysicalLinesCounter.Instance;
                case LanguageNames.VisualBasic:
                    return VisualBasic.VisualBasicPhysicalLinesCounter.Instance;
            }

            Debug.Assert(language == LanguageNames.FSharp, language);

            return null;
        }

        public static CodeMetricsCounter GetLogicalLinesCounter(string language)
        {
            switch (language)
            {
                case LanguageNames.CSharp:
                    return CSharp.CSharpLogicalLinesCounter.Instance;
                case LanguageNames.VisualBasic:
                    return VisualBasic.VisualBasicLogicalLinesCounter.Instance;
            }

            Debug.Assert(language == LanguageNames.FSharp, language);

            return null;
        }

        public static ImmutableDictionary<ProjectId, CodeMetrics> CountPhysicalLinesInParallel(
            IEnumerable<Project> projects,
            CodeMetricsOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return CountLinesInParallel(projects, GetPhysicalLinesCounter, options, cancellationToken);
        }

        public static ImmutableDictionary<ProjectId, CodeMetrics> CountLogicalLinesInParallel(
            IEnumerable<Project> projects,
            CodeMetricsOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return CountLinesInParallel(projects, GetLogicalLinesCounter, options, cancellationToken);
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
            return CountLinesAsync(projects, GetPhysicalLinesCounter, options, cancellationToken);
        }

        public static Task<ImmutableDictionary<ProjectId, CodeMetrics>> CountLogicalLinesAsync(
            IEnumerable<Project> projects,
            CodeMetricsOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return CountLinesAsync(projects, GetLogicalLinesCounter, options, cancellationToken);
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

            if (!options.IncludeGeneratedCode
                && GeneratedCodeUtility.IsGeneratedCode(tree, SyntaxFacts.IsComment, cancellationToken))
            {
                return default;
            }

            SyntaxNode root = await tree.GetRootAsync(cancellationToken).ConfigureAwait(false);

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            return CountLines(root, sourceText, options, cancellationToken);
        }

        private protected int CountWhitespaceLines(SyntaxNode root, SourceText sourceText)
        {
            int whitespaceLineCount = 0;

            foreach (TextLine line in sourceText.Lines)
            {
                if (line.IsEmptyOrWhiteSpace())
                {
                    if (line.End == sourceText.Length
                        || SyntaxFacts.IsEndOfLineTrivia(root.FindTrivia(line.End)))
                    {
                        whitespaceLineCount++;
                    }
                }
            }

            return whitespaceLineCount;
        }
    }
}
