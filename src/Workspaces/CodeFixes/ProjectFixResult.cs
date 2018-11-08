// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CodeFixes
{
    public class ProjectFixResult
    {
        public static ProjectFixResult Skipped { get; } = new ProjectFixResult(ProjectFixKind.Skipped);

        public static ProjectFixResult NoAnalyzers { get; } = new ProjectFixResult(ProjectFixKind.NoAnalyzers);

        internal ProjectFixResult(
            ProjectFixKind kind,
            IEnumerable<DiagnosticDescriptor> fixedDiagnostics = default,
            IEnumerable<DiagnosticDescriptor> unfixedDiagnostics = default,
            IEnumerable<DiagnosticDescriptor> unfixableDiagnostics = default,
            IEnumerable<DiagnosticAnalyzer> analyzers = default,
            IEnumerable<CodeFixProvider> fixers = default,
            int formattedDocumentCount = 0)
        {
            Kind = kind;
            FixedDiagnostics = fixedDiagnostics?.ToImmutableArray() ?? ImmutableArray<DiagnosticDescriptor>.Empty;
            UnfixedDiagnostics = unfixedDiagnostics?.ToImmutableArray() ?? ImmutableArray<DiagnosticDescriptor>.Empty;
            UnfixableDiagnostics = unfixableDiagnostics?.ToImmutableArray() ?? ImmutableArray<DiagnosticDescriptor>.Empty;
            Analyzers = analyzers?.ToImmutableArray() ?? ImmutableArray<DiagnosticAnalyzer>.Empty;
            Fixers = fixers?.ToImmutableArray() ?? ImmutableArray<CodeFixProvider>.Empty;
            FormattedDocumentCount = formattedDocumentCount;
        }

        public ProjectFixKind Kind { get; }

        public ImmutableArray<DiagnosticDescriptor> FixedDiagnostics { get; }

        public ImmutableArray<DiagnosticDescriptor> UnfixedDiagnostics { get; }

        public ImmutableArray<DiagnosticDescriptor> UnfixableDiagnostics { get; }

        public ImmutableArray<DiagnosticAnalyzer> Analyzers { get; }

        public ImmutableArray<CodeFixProvider> Fixers { get; }

        public int FormattedDocumentCount { get; }
    }
}
