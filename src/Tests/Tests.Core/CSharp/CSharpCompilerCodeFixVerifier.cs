// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Tests.CSharp
{
    public class CSharpCompilerCodeFixVerifier : CompilerCodeFixVerifier
    {
        public static CSharpCompilerCodeFixVerifier Instance { get; } = new CSharpCompilerCodeFixVerifier();

        public override string Language
        {
            get { return LanguageNames.CSharp; }
        }

        public void VerifyFix(
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

        public void VerifyFix(
            string source,
            string expected,
            string diagnosticId,
            CodeFixProvider fixProvider,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            VerifyFixAsync(
                source: source,
                expected: expected,
                diagnosticId: diagnosticId,
                fixProvider: fixProvider,
                equivalenceKey: equivalenceKey,
                options: options,
                cancellationToken: cancellationToken).Wait();
        }

        public void VerifyNoFix(
            string source,
            CodeFixProvider fixProvider,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            VerifyNoFixAsync(
                source: source,
                fixProvider: fixProvider,
                equivalenceKey: equivalenceKey,
                options: options,
                cancellationToken: cancellationToken).Wait();
        }
    }
}
