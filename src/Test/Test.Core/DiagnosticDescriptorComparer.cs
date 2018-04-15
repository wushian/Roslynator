// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Roslynator.Test
{
    public abstract class DiagnosticDescriptorComparer : IComparer<DiagnosticDescriptor>, IEqualityComparer<DiagnosticDescriptor>
    {
        public static DiagnosticDescriptorComparer IdOrdinal { get; } = new DiagnosticDescriptorIdComparer();

        public abstract int Compare(DiagnosticDescriptor x, DiagnosticDescriptor y);

        public abstract bool Equals(DiagnosticDescriptor x, DiagnosticDescriptor y);

        public abstract int GetHashCode(DiagnosticDescriptor obj);

        private class DiagnosticDescriptorIdComparer : DiagnosticDescriptorComparer
        {
            public override int Compare(DiagnosticDescriptor x, DiagnosticDescriptor y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                return string.Compare(x.Id, y.Id, StringComparison.Ordinal);
            }

            public override bool Equals(DiagnosticDescriptor x, DiagnosticDescriptor y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x == null)
                    return false;

                if (y == null)
                    return false;

                return string.Equals(x.Id, y.Id, StringComparison.Ordinal);
            }

            public override int GetHashCode(DiagnosticDescriptor obj)
            {
                return obj?.Id.GetHashCode() ?? 0;
            }
        }
    }
}
