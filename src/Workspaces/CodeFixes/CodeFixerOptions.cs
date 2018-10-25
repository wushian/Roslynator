// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator.CodeFixes
{
    //TODO: DiagnosticCodeFixProviderMap RCS1155=Roslynator.CodeFixes.MyCodeFixProvider
    //TODO: DiagnosticEquivalenceKeyMap RCS1155=Roslynator.RCS1155.CurrentCultureIgnoreCase
    public class CodeFixerOptions
    {
        public static CodeFixerOptions Default { get; } = new CodeFixerOptions();

        public CodeFixerOptions(
            DiagnosticSeverity minimalSeverity = DiagnosticSeverity.Info,
            bool ignoreCompilerErrors = false,
            bool ignoreAnalyzerReferences = false,
            IEnumerable<string> supportedDiagnosticIds = null,
            IEnumerable<string> ignoredDiagnosticIds = null,
            IEnumerable<string> ignoredCompilerDiagnosticIds = null,
            IEnumerable<string> ignoredProjectNames = null,
            string language = null,
            int batchSize = -1,
            bool format = false)
        {
            MinimalSeverity = minimalSeverity;
            IgnoreCompilerErrors = ignoreCompilerErrors;
            IgnoreAnalyzerReferences = ignoreAnalyzerReferences;
            SupportedDiagnosticIds = supportedDiagnosticIds?.ToImmutableHashSet() ?? ImmutableHashSet<string>.Empty;

            //TODO: 
            IgnoredDiagnosticIds = (ignoredDiagnosticIds != null && SupportedDiagnosticIds.Count == 0)
                ? ignoredDiagnosticIds.ToImmutableHashSet()
                : ImmutableHashSet<string>.Empty;

            IgnoredCompilerDiagnosticIds = ignoredCompilerDiagnosticIds?.ToImmutableHashSet() ?? ImmutableHashSet<string>.Empty;
            IgnoredProjectNames = ignoredProjectNames?.ToImmutableHashSet() ?? ImmutableHashSet<string>.Empty;
            Language = language;
            BatchSize = batchSize;
            Format = format;
        }

        public DiagnosticSeverity MinimalSeverity { get; }

        public bool IgnoreCompilerErrors { get; }

        public bool IgnoreAnalyzerReferences { get; }

        public ImmutableHashSet<string> SupportedDiagnosticIds { get; }

        public ImmutableHashSet<string> IgnoredDiagnosticIds { get; }

        public ImmutableHashSet<string> IgnoredCompilerDiagnosticIds { get; }

        public ImmutableHashSet<string> IgnoredProjectNames { get; }

        public string Language { get; }

        public int BatchSize { get; }

        public bool Format { get; }

        internal bool IsSupported(string diagnosticId)
        {
            return (SupportedDiagnosticIds.Count > 0)
                ? SupportedDiagnosticIds.Contains(diagnosticId)
                : !IgnoredDiagnosticIds.Contains(diagnosticId);
        }
    }
}
