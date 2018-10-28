// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Telemetry;

namespace Roslynator.Diagnostics
{
    public class ProjectAnalysisResult
    {
        internal ProjectAnalysisResult(
            ProjectId projectId,
            ImmutableArray<DiagnosticAnalyzer> analyzers,
            ImmutableArray<Diagnostic> diagnostics,
            ImmutableArray<Diagnostic> compilerDiagnostics,
            ImmutableDictionary<DiagnosticAnalyzer, AnalyzerTelemetryInfo> telemetry)
        {
            ProjectId = projectId;
            Analyzers = analyzers;
            Diagnostics = diagnostics;
            CompilerDiagnostics = compilerDiagnostics;
            Telemetry = telemetry;
        }

        public ProjectId ProjectId { get; }

        public ImmutableArray<DiagnosticAnalyzer> Analyzers { get; }

        public ImmutableArray<Diagnostic> Diagnostics { get; }

        public ImmutableArray<Diagnostic> CompilerDiagnostics { get; }

        public ImmutableDictionary<DiagnosticAnalyzer, AnalyzerTelemetryInfo> Telemetry { get; }

        public bool IsDefault => ProjectId == null;
    }
}
