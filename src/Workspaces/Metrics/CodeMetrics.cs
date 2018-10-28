// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Metrics
{
    public readonly struct CodeMetrics
    {
        internal static CodeMetrics NotAvailable { get; } = new CodeMetrics(-1, 0, 0, 0, 0, 0);

        internal CodeMetrics(
            int totalLineCount,
            int codeLineCount,
            int whitespaceLineCount,
            int commentLineCount,
            int preprocessorDirectiveLineCount,
            int blockBoundaryLineCount)
        {
            TotalLineCount = totalLineCount;
            CodeLineCount = codeLineCount;
            WhitespaceLineCount = whitespaceLineCount;
            CommentLineCount = commentLineCount;
            PreprocessorDirectiveLineCount = preprocessorDirectiveLineCount;
            BlockBoundaryLineCount = blockBoundaryLineCount;
        }

        public int TotalLineCount { get; }

        public int CodeLineCount { get; }

        public int WhitespaceLineCount { get; }

        public int CommentLineCount { get; }

        public int PreprocessorDirectiveLineCount { get; }

        public int BlockBoundaryLineCount { get; }

        internal CodeMetrics Add(in CodeMetrics codeMetrics)
        {
            return new CodeMetrics(
                totalLineCount: TotalLineCount + codeMetrics.TotalLineCount,
                codeLineCount: CodeLineCount + codeMetrics.CodeLineCount,
                whitespaceLineCount: WhitespaceLineCount + codeMetrics.WhitespaceLineCount,
                commentLineCount: CommentLineCount + codeMetrics.CommentLineCount,
                preprocessorDirectiveLineCount: PreprocessorDirectiveLineCount + codeMetrics.PreprocessorDirectiveLineCount,
                blockBoundaryLineCount: BlockBoundaryLineCount + codeMetrics.BlockBoundaryLineCount);
        }
    }
}
