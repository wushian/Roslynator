// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Metrics.VisualBasic
{
    public class VisualBasicLogicalLinesCounter : VisualBasicCodeMetricsCounter
    {
        public static VisualBasicLogicalLinesCounter Instance { get; } = new VisualBasicLogicalLinesCounter();

        protected override CodeMetrics CountLines(SyntaxNode node, SourceText sourceText, CodeMetricsOptions options, CancellationToken cancellationToken)
        {
            TextLineCollection lines = sourceText.Lines;

            var walker = new VisualBasicLogicalLinesWalker(lines, options, cancellationToken);

            walker.Visit(node);

            return new CodeMetrics(
                totalLineCount: lines.Count,
                codeLineCount: walker.LogicalLineCount,
                whiteSpaceLineCount: CountWhiteSpaceLines(node, sourceText, options),
                commentLineCount: walker.CommentLineCount,
                preprocessorDirectiveLineCount: walker.PreprocessorDirectiveLineCount,
                blockBoundaryLineCount: 0);
        }
    }
}
