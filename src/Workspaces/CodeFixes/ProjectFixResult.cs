// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CodeFixes
{
    public class ProjectFixResult
    {
        public static ProjectFixResult Skipped { get; } = new ProjectFixResult(
            ImmutableArray<DiagnosticDescriptor>.Empty,
            ImmutableArray<DiagnosticDescriptor>.Empty,
            ImmutableArray<DiagnosticAnalyzer>.Empty,
            ImmutableArray<CodeFixProvider>.Empty,
            ProjectFixKind.Skipped);

        public static ProjectFixResult NoAnalyzers { get; } = new ProjectFixResult(
            ImmutableArray<DiagnosticDescriptor>.Empty,
            ImmutableArray<DiagnosticDescriptor>.Empty,
            ImmutableArray<DiagnosticAnalyzer>.Empty,
            ImmutableArray<CodeFixProvider>.Empty,
            ProjectFixKind.NoAnalyzers);

        public ProjectFixResult(
            ImmutableArray<DiagnosticDescriptor> fixedDiagnostics,
            ImmutableArray<DiagnosticDescriptor> unfixedDiagnostics,
            ImmutableArray<DiagnosticAnalyzer> analyzers,
            ImmutableArray<CodeFixProvider> fixers,
            ProjectFixKind kind)
        {
            FixedDiagnostics = fixedDiagnostics;
            UnfixedDiagnostics = unfixedDiagnostics;
            Analyzers = analyzers;
            Fixers = fixers;
            Kind = kind;
        }

        public ImmutableArray<DiagnosticDescriptor> FixedDiagnostics { get; }

        public ImmutableArray<DiagnosticDescriptor> UnfixedDiagnostics { get; }

        public ImmutableArray<DiagnosticAnalyzer> Analyzers { get; }

        public ImmutableArray<CodeFixProvider> Fixers { get; }

        public ProjectFixKind Kind { get; }

        internal ProjectFixResult WithUnfixedDiagnostics(ImmutableArray<DiagnosticDescriptor> unfixedDiagnostics)
        {
            return new ProjectFixResult(FixedDiagnostics, unfixedDiagnostics, Analyzers, Fixers, Kind);
        }
    }
}
