// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.Test
{
    public static class CSharpCodeFixVerifier
    {
        public static void VerifyFix(
            string source,
            string newSource,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider codeFixProvider,
            bool allowNewCompilerDiagnostics = false)
        {
            CodeFixVerifier.VerifyFix(
                source,
                newSource,
                analyzer,
                codeFixProvider,
                LanguageNames.CSharp,
                allowNewCompilerDiagnostics);
        }
    }
}
