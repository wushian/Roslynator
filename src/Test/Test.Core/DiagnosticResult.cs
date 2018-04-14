// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator.Test
{
    public readonly struct DiagnosticResult
    {
        public DiagnosticResult(DiagnosticDescriptor descriptor, FileLinePositionSpan span)
            : this(descriptor, ImmutableArray.Create(span))
        {
        }

        public DiagnosticResult(DiagnosticDescriptor descriptor, ImmutableArray<FileLinePositionSpan> spans)
        {
            Descriptor = descriptor;
            Spans = spans;
        }

        public DiagnosticDescriptor Descriptor { get; }

        public ImmutableArray<FileLinePositionSpan> Spans { get; }
    }
}
