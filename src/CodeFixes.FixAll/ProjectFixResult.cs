// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CodeFixes
{
    internal readonly struct ProjectFixResult
    {
        public static ProjectFixResult Skipped { get; } = new ProjectFixResult(
            ImmutableArray<string>.Empty,
            ImmutableArray<DiagnosticAnalyzer>.Empty,
            ImmutableArray<CodeFixProvider>.Empty,
            FixResult.Skipped);

        public static ProjectFixResult NoAnalyzers { get; } = new ProjectFixResult(
            ImmutableArray<string>.Empty,
            ImmutableArray<DiagnosticAnalyzer>.Empty,
            ImmutableArray<CodeFixProvider>.Empty,
            FixResult.NoAnalyzers);

        public ProjectFixResult(
            ImmutableArray<string> fixedDiagnosticIds,
            ImmutableArray<DiagnosticAnalyzer> analyzers,
            ImmutableArray<CodeFixProvider> fixers,
            FixResult result)
        {
            FixedDiagnosticIds = fixedDiagnosticIds;
            Analyzers = analyzers;
            Fixers = fixers;
            Result = result;
        }

        public ImmutableArray<string> FixedDiagnosticIds { get; }

        public ImmutableArray<DiagnosticAnalyzer> Analyzers { get; }

        public ImmutableArray<CodeFixProvider> Fixers { get; }

        public FixResult Result { get; }
    }
}
