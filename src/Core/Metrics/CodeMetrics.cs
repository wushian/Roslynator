// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Metrics
{
    public readonly struct CodeMetrics : IEquatable<CodeMetrics>
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

        public override bool Equals(object obj)
        {
            return obj is CodeMetrics other && Equals(other);
        }

        public bool Equals(CodeMetrics other)
        {
            return TotalLineCount == other.TotalLineCount
                && CodeLineCount == other.CodeLineCount
                && WhitespaceLineCount == other.WhitespaceLineCount
                && CommentLineCount == other.CommentLineCount
                && PreprocessorDirectiveLineCount == other.PreprocessorDirectiveLineCount
                && BlockBoundaryLineCount == other.BlockBoundaryLineCount;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(TotalLineCount,
                Hash.Combine(CodeLineCount,
                Hash.Combine(WhitespaceLineCount,
                Hash.Combine(CommentLineCount,
                Hash.Combine(PreprocessorDirectiveLineCount, Hash.Create(BlockBoundaryLineCount))))));
        }

        public static bool operator ==(in CodeMetrics metrics1, in CodeMetrics metrics2)
        {
            return metrics1.Equals(metrics2);
        }

        public static bool operator !=(in CodeMetrics metrics1, in CodeMetrics metrics2)
        {
            return !(metrics1 == metrics2);
        }
    }
}
