// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
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
            bool allowUnsafe,
            DiagnosticSeverity maxAllowedCompilerDiagnosticSeverity)
        {
            MaxAllowedCompilerDiagnosticSeverity = maxAllowedCompilerDiagnosticSeverity;
            EnableDiagnosticsDisabledByDefault = enableDiagnosticsDisabledByDefault;
            AllowNewCompilerDiagnostics = allowNewCompilerDiagnostics;
            AllowUnsafe = allowUnsafe;
        }

        public static CodeVerificationOptions Default { get; } = new CodeVerificationOptions();

        public bool AllowNewCompilerDiagnostics { get; }

        public bool EnableDiagnosticsDisabledByDefault { get; } = true;

        //TODO: 
        public bool AllowUnsafe { get; } = true;

        public DiagnosticSeverity MaxAllowedCompilerDiagnosticSeverity { get; } = DiagnosticSeverity.Warning;
    }
}
