// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.Tests
{
    public class CodeVerificationOptions
    {
        public CodeVerificationOptions()
        {
        }

        public CodeVerificationOptions(
            bool allowNewCompilerDiagnostics,
            bool enableDiagnosticsDisabledByDefault,
            DiagnosticSeverity maxAllowedCompilerDiagnosticSeverity)
        {
            MaxAllowedCompilerDiagnosticSeverity = maxAllowedCompilerDiagnosticSeverity;
            EnableDiagnosticsDisabledByDefault = enableDiagnosticsDisabledByDefault;
            AllowNewCompilerDiagnostics = allowNewCompilerDiagnostics;
        }

        public static CodeVerificationOptions Default { get; } = new CodeVerificationOptions();

        public bool AllowNewCompilerDiagnostics { get; }

        public bool EnableDiagnosticsDisabledByDefault { get; } = true;

        public DiagnosticSeverity MaxAllowedCompilerDiagnosticSeverity { get; } = DiagnosticSeverity.Warning;
    }
}
