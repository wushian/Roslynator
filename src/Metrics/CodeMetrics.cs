// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Metrics
{
    public readonly struct CodeMetrics
    {
        public CodeMetrics(
            int totalLineCount,
            int codeLineCount,
            int whiteSpaceLineCount,
            int commentLineCount,
            int preprocessorDirectiveLineCount,
            int blockBoundaryLineCount)
        {
            TotalLineCount = totalLineCount;
            CodeLineCount = codeLineCount;
            WhiteSpaceLineCount = whiteSpaceLineCount;
            CommentLineCount = commentLineCount;
            PreprocessorDirectiveLineCount = preprocessorDirectiveLineCount;
            BlockBoundaryLineCount = blockBoundaryLineCount;
        }

        public int TotalLineCount { get; }

        public int CodeLineCount { get; }

        public int WhiteSpaceLineCount { get; }

        public int CommentLineCount { get; }

        public int PreprocessorDirectiveLineCount { get; }

        public int BlockBoundaryLineCount { get; }

        internal CodeMetrics Add(in CodeMetrics codeMetrics)
        {
            return new CodeMetrics(
                totalLineCount: TotalLineCount + codeMetrics.TotalLineCount,
                codeLineCount: CodeLineCount + codeMetrics.CodeLineCount,
                whiteSpaceLineCount: WhiteSpaceLineCount + codeMetrics.WhiteSpaceLineCount,
                commentLineCount: CommentLineCount + codeMetrics.CommentLineCount,
                preprocessorDirectiveLineCount: PreprocessorDirectiveLineCount + codeMetrics.PreprocessorDirectiveLineCount,
                blockBoundaryLineCount: BlockBoundaryLineCount + codeMetrics.BlockBoundaryLineCount);
        }
    }
}
