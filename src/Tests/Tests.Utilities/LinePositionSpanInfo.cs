// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Tests
{
    public readonly struct LinePositionSpanInfo
    {
        public LinePositionSpanInfo(LinePositionInfo start, LinePositionInfo end)
        {
            Start = start;
            End = end;
        }

        public LinePositionInfo Start { get; }

        public LinePositionInfo End { get; }

        public TextSpan Span
        {
            get { return TextSpan.FromBounds(Start.Index, End.Index); }
        }

        public LinePositionSpan LineSpan
        {
            get { return new LinePositionSpan(Start.LinePosition, End.LinePosition); }
        }
    }
}
