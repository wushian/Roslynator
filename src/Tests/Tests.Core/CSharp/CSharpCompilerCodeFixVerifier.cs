// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Tests.CSharp
{
    public static class CSharpCompilerCodeFixVerifier
    {
        public static void VerifyFix(
            string source,
            string fixableCode,
            string fixedCode,
            string diagnosticId,
            CodeFixProvider fixProvider,
            string equivalenceKey = null)
        {
            (string result1, string result2, TextSpan span) = TextUtility.GetMarkedSpan(source, fixableCode, fixedCode);

            VerifyFix(
                source: result1,
                expected: result2,
                diagnosticId: diagnosticId,
                fixProvider: fixProvider,
                equivalenceKey: equivalenceKey);
        }

        public static void VerifyFix(
            string source,
            string expected,
            string diagnosticId,
            CodeFixProvider fixProvider,
            string equivalenceKey = null)
        {
            CompilerCodeFixVerifier.VerifyFixAsync(
                source: source,
                expected: expected,
                diagnosticId: diagnosticId,
                fixProvider: fixProvider,
                language: LanguageNames.CSharp,
                equivalenceKey: equivalenceKey).Wait();
        }

        public static void VerifyNoFix(
            string source,
            CodeFixProvider fixProvider,
            string equivalenceKey = null)
        {
            CompilerCodeFixVerifier.VerifyNoFixAsync(
                source: source,
                fixProvider: fixProvider,
                language: LanguageNames.CSharp,
                equivalenceKey: equivalenceKey).Wait();
        }
    }
}
