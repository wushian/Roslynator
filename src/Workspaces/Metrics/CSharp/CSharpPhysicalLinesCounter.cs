// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Metrics.CSharp
{
    public class CSharpPhysicalLinesCounter : CSharpCodeMetricsCounter
    {
        public static CSharpPhysicalLinesCounter Instance { get; } = new CSharpPhysicalLinesCounter();

        protected override CodeMetrics CountLines(SyntaxNode node, SourceText sourceText, CodeMetricsOptions options, CancellationToken cancellationToken)
        {
            TextLineCollection lines = sourceText.Lines;

            var walker = new CSharpPhysicalLinesWalker(lines, options, cancellationToken);

            walker.Visit(node);

            int whiteSpaceLineCount = (options.IncludeWhiteSpace) ? 0 : CountWhiteSpaceLines(node, sourceText);

            return new CodeMetrics(
                totalLineCount: lines.Count,
                codeLineCount: lines.Count - whiteSpaceLineCount - walker.CommentLineCount - walker.PreprocessorDirectiveLineCount - walker.BlockBoundaryLineCount,
                whiteSpaceLineCount: whiteSpaceLineCount,
                commentLineCount: walker.CommentLineCount,
                preprocessorDirectiveLineCount: walker.PreprocessorDirectiveLineCount,
                blockBoundaryLineCount: walker.BlockBoundaryLineCount);
        }
    }
}
