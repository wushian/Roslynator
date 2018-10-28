// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public abstract class CodeAnalysisOptions
    {
        protected CodeAnalysisOptions(
            DiagnosticSeverity minimalSeverity = DiagnosticSeverity.Info,
            bool ignoreAnalyzerReferences = false,
            IEnumerable<string> supportedDiagnosticIds = null,
            IEnumerable<string> ignoredDiagnosticIds = null,
            IEnumerable<string> ignoredProjectNames = null,
            string language = null)
        {
            if (supportedDiagnosticIds?.Any() == true
                && ignoredDiagnosticIds?.Any() == true)
            {
                throw new ArgumentException($"Cannot specify both '{supportedDiagnosticIds}' and '{ignoredDiagnosticIds}'.", nameof(ignoredDiagnosticIds));
            }

            MinimalSeverity = minimalSeverity;
            IgnoreAnalyzerReferences = ignoreAnalyzerReferences;
            SupportedDiagnosticIds = supportedDiagnosticIds?.ToImmutableHashSet() ?? ImmutableHashSet<string>.Empty;
            IgnoredDiagnosticIds = ignoredDiagnosticIds?.ToImmutableHashSet() ?? ImmutableHashSet<string>.Empty;
            IgnoredProjectNames = ignoredProjectNames?.ToImmutableHashSet() ?? ImmutableHashSet<string>.Empty;
            Language = language;
        }

        public DiagnosticSeverity MinimalSeverity { get; }

        public bool IgnoreAnalyzerReferences { get; }

        public ImmutableHashSet<string> SupportedDiagnosticIds { get; }

        public ImmutableHashSet<string> IgnoredDiagnosticIds { get; }

        public ImmutableHashSet<string> IgnoredProjectNames { get; }

        public string Language { get; }

        internal bool IsSupported(string diagnosticId)
        {
            return (SupportedDiagnosticIds.Count > 0)
                ? SupportedDiagnosticIds.Contains(diagnosticId)
                : !IgnoredDiagnosticIds.Contains(diagnosticId);
        }
    }
}
