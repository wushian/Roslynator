// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;

namespace Roslynator.Tests
{
    public readonly struct TestSourceTextAnalysis
    {
        public TestSourceTextAnalysis(string originalSource, string source, ImmutableArray<LinePositionSpanInfo> spans)
        {
            OriginalSource = originalSource;
            Source = source;
            Spans = spans;
        }

        public string OriginalSource { get; }

        public string Source { get; }

        public ImmutableArray<LinePositionSpanInfo> Spans { get; }
    }
}
