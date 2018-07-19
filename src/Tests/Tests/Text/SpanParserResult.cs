// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;

namespace Roslynator.Tests.Text
{
    public readonly struct SpanParserResult : IEquatable<SpanParserResult>
    {
        public SpanParserResult(string source, ImmutableArray<LinePositionSpanInfo> spans)
        {
            Source = source;
            Spans = spans;
        }

        public string Source { get; }

        public ImmutableArray<LinePositionSpanInfo> Spans { get; }

        public override bool Equals(object obj)
        {
            return obj is SpanParserResult other
                && Equals(other);
        }

        public bool Equals(SpanParserResult other)
        {
            return Source == other.Source
                   && Spans.Equals(other.Spans);
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Spans.GetHashCode(), Hash.Create(Source));
        }

        public static bool operator ==(in SpanParserResult analysis1, in SpanParserResult analysis2)
        {
            return analysis1.Equals(analysis2);
        }

        public static bool operator !=(in SpanParserResult analysis1, in SpanParserResult analysis2)
        {
            return !(analysis1 == analysis2);
        }
    }
}
