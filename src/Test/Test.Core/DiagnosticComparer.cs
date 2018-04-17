// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Roslynator.Test
{
    public abstract class DiagnosticComparer : IComparer<Diagnostic>, IEqualityComparer<Diagnostic>
    {
        public static DiagnosticComparer IdOrdinal { get; } = new DiagnosticIdComparer();

        public static DiagnosticComparer SpanStart { get; } = new DiagnosticSpanStartComparer();

        public abstract int Compare(Diagnostic x, Diagnostic y);

        public abstract bool Equals(Diagnostic x, Diagnostic y);

        public abstract int GetHashCode(Diagnostic obj);

        private class DiagnosticIdComparer : DiagnosticComparer
        {
            public override int Compare(Diagnostic x, Diagnostic y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                return string.Compare(x.Id, y.Id, StringComparison.Ordinal);
            }

            public override bool Equals(Diagnostic x, Diagnostic y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x == null)
                    return false;

                if (y == null)
                    return false;

                return string.Equals(x.Id, y.Id, StringComparison.Ordinal);
            }

            public override int GetHashCode(Diagnostic obj)
            {
                return obj?.Id.GetHashCode() ?? 0;
            }
        }

        private class DiagnosticSpanStartComparer : DiagnosticComparer
        {
            public override int Compare(Diagnostic x, Diagnostic y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                return Comparer<int>.Default.Compare(x.Location.SourceSpan.Start, y.Location.SourceSpan.Start);
            }

            public override bool Equals(Diagnostic x, Diagnostic y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x == null)
                    return false;

                if (y == null)
                    return false;

                return x.Location.SourceSpan.Start == y.Location.SourceSpan.Start;
            }

            public override int GetHashCode(Diagnostic obj)
            {
                return obj?.Location.SourceSpan.Start.GetHashCode() ?? 0;
            }
        }
    }
}
