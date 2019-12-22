// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator.Tests
{
    public abstract class CodeVerificationOptions
    {
        protected CodeVerificationOptions(
            bool allowNewCompilerDiagnostics = false,
            bool enableDiagnosticsDisabledByDefault = true,
            DiagnosticSeverity maxAllowedCompilerDiagnosticSeverity = DiagnosticSeverity.Info,
            IEnumerable<string> allowedCompilerDiagnosticIds = null)
        {
            AllowNewCompilerDiagnostics = allowNewCompilerDiagnostics;
            EnableDiagnosticsDisabledByDefault = enableDiagnosticsDisabledByDefault;
            MaxAllowedCompilerDiagnosticSeverity = maxAllowedCompilerDiagnosticSeverity;
            AllowedCompilerDiagnosticIds = allowedCompilerDiagnosticIds?.ToImmutableArray() ?? ImmutableArray<string>.Empty;
        }

        protected abstract ParseOptions CommonParseOptions { get; }

        public ParseOptions ParseOptions => CommonParseOptions;

        protected abstract CompilationOptions CommonCompilationOptions { get; }

        public CompilationOptions CompilationOptions => CommonCompilationOptions;

        //TODO: AllowNewCompilerDiagnostics > IgnoreNewCompilerDiagnostics
        public bool AllowNewCompilerDiagnostics { get; }

        //TODO: rename EnableDiagnosticsDisabledByDefault
        public bool EnableDiagnosticsDisabledByDefault { get; }

        //TODO: MaxAllowedCompilerDiagnosticSeverity > AllowedCompilerDiagnosticSeverity
        public DiagnosticSeverity MaxAllowedCompilerDiagnosticSeverity { get; }

        //TODO: AllowedCompilerDiagnosticIds > IgnoredCompilerDiagnosticIds
        public ImmutableArray<string> AllowedCompilerDiagnosticIds { get; }

        public abstract CodeVerificationOptions AddAllowedCompilerDiagnosticId(string diagnosticId);

        public abstract CodeVerificationOptions AddAllowedCompilerDiagnosticIds(IEnumerable<string> diagnosticIds);
    }
}
