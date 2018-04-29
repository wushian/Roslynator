// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.Tests.CSharp
{
    public static class CSharpCodeFixVerifier
    {
        public static void VerifyFix(
            string source,
            string expected,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fixProvider,
            CodeVerificationSettings settings = null)
        {
            CodeFixVerifier.VerifyFixAsync(
                source: source,
                expected: expected,
                analyzer: analyzer,
                fixProvider: fixProvider,
                language: LanguageNames.CSharp,
                settings: settings).Wait();
        }

        public static void VerifyNoFix(
            string source,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fixProvider)
        {
            CodeFixVerifier.VerifyNoFixAsync(
                source: source,
                analyzer: analyzer,
                fixProvider: fixProvider,
                language: LanguageNames.CSharp).Wait();
        }
    }
}
