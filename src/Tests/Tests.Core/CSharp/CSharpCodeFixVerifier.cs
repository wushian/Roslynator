// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.Tests.CSharp
{
    public class CSharpCodeFixVerifier : CodeFixVerifier
    {
        public static CSharpCodeFixVerifier Instance { get; } = new CSharpCodeFixVerifier();

        public override string Language
        {
            get { return LanguageNames.CSharp; }
        }

        //TODO: diagnosticId
        public void VerifyFix(
            string source,
            string expected,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fixProvider,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            VerifyFixAsync(
                source: source,
                expected: expected,
                analyzer: analyzer,
                fixProvider: fixProvider,
                options: options,
                cancellationToken: cancellationToken).Wait();
        }

        public void VerifyNoFix(
            string source,
            string diagnosticId,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fixProvider,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            VerifyNoFixAsync(
                source: source,
                diagnosticId: diagnosticId,
                analyzer: analyzer,
                fixProvider: fixProvider,
                options: options,
                cancellationToken: cancellationToken).Wait();
        }
    }
}
