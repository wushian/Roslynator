// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator.Tests
{
    public class CodeVerificationSettings
    {
        public CodeVerificationSettings(bool allowNewCompilerDiagnostics)
        {
            AllowNewCompilerDiagnostics = allowNewCompilerDiagnostics;
        }

        public static CodeVerificationSettings Default { get; } = new CodeVerificationSettings(allowNewCompilerDiagnostics: false);

        public DiagnosticSeverity MaxAllowedCompilerDiagnosticSeverity { get; } = DiagnosticSeverity.Info;

        public ImmutableArray<string> AllowedCompilerDiagnosticIds { get; }

        public bool EnableDiagnosticsDisabledByDefault { get; }

        public bool AllowNewCompilerDiagnostics { get; }

        public bool AllowUnsafe { get; }
    }
}
