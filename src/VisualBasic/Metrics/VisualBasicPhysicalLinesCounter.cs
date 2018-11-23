// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Metrics;

namespace Roslynator.VisualBasic.Metrics
{
    public class VisualBasicPhysicalLinesCounter : VisualBasicCodeMetricsCounter
    {
        public static VisualBasicPhysicalLinesCounter Instance { get; } = new VisualBasicPhysicalLinesCounter();

        public override CodeMetrics CountLines(SyntaxNode node, SourceText sourceText, CodeMetricsOptions options, CancellationToken cancellationToken)
        {
            TextLineCollection lines = sourceText.Lines;

            var walker = new VisualBasicPhysicalLinesWalker(lines, options, cancellationToken);

            walker.Visit(node);

            int whitespaceLineCount = (options.IncludeWhitespace) ? 0 : CountWhitespaceLines(node, sourceText);

            return new CodeMetrics(
                totalLineCount: lines.Count,
                codeLineCount: lines.Count - whitespaceLineCount - walker.CommentLineCount - walker.PreprocessorDirectiveLineCount - walker.BlockBoundaryLineCount,
                whitespaceLineCount: whitespaceLineCount,
                commentLineCount: walker.CommentLineCount,
                preprocessorDirectiveLineCount: walker.PreprocessorDirectiveLineCount,
                blockBoundaryLineCount: walker.BlockBoundaryLineCount);
        }
    }
}
