// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Roslynator
{
    public static class CodeFixProviderDiagnostic
    {
        public static bool CanFixAny(this CodeFixProvider codeFixProvider, ImmutableArray<DiagnosticDescriptor> diagnosticDecriptors)
        {
            return codeFixProvider.FixableDiagnosticIds.Intersect(diagnosticDecriptors.Select(f => f.Id)).Any();
        }
    }
}
