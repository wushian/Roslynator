// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator.CodeFixes
{
    //TODO: DiagnosticCodeFixProviderMap RCS1155=Roslynator.CodeFixes.MyCodeFixProvider
    //TODO: DiagnosticEquivalenceKeyMap RCS1155=Roslynator.RCS1155.CurrentCultureIgnoreCase
    public class CodeFixerOptions : CodeAnalysisOptions
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
            bool format = false) : base(minimalSeverity, ignoreAnalyzerReferences, supportedDiagnosticIds, ignoredDiagnosticIds, ignoredProjectNames, language)
        {
            IgnoreCompilerErrors = ignoreCompilerErrors;
            IgnoredCompilerDiagnosticIds = ignoredCompilerDiagnosticIds?.ToImmutableHashSet() ?? ImmutableHashSet<string>.Empty;
            BatchSize = batchSize;
            Format = format;
        }

        public bool IgnoreCompilerErrors { get; }

        public ImmutableHashSet<string> IgnoredCompilerDiagnosticIds { get; }

        public int BatchSize { get; }

        public bool Format { get; }
    }
}
