// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Tests.CSharp
{
    public class CSharpDiagnosticVerifier : DiagnosticVerifier
    {
        public static CSharpDiagnosticVerifier Instance { get; } = new CSharpDiagnosticVerifier();

        public override string Language => LanguageNames.CSharp;

        public void VerifyDiagnosticAndFix(
            string source,
            string expected,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fixProvider,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            (string result, List<Diagnostic> diagnostics) = TextUtility.GetMarkedDiagnostics(source, descriptor, FileUtility.DefaultCSharpFileName);

            VerifyDiagnostic(result, analyzer, diagnostics.ToArray());

            //TODO: 
            CSharpCodeFixVerifier.Instance.VerifyFix(result, expected, analyzer, fixProvider, options, cancellationToken);
        }

        public void VerifyDiagnosticAndFix(
            string source,
            string expected,
            TextSpan span,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fixProvider,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            VerifyDiagnostic(source, span, analyzer, descriptor);

            //TODO: 
            CSharpCodeFixVerifier.Instance.VerifyFix(source, expected, analyzer, fixProvider, options, cancellationToken);
        }

        public void VerifyDiagnosticAndFix(
            string source,
            string fixableCode,
            string fixedCode,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fixProvider,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            (string result1, string result2, TextSpan span) = TextUtility.GetMarkedSpan(source, fixableCode, fixedCode);

            (string result3, List<TextSpan> spans) = TextUtility.GetMarkedSpans(result1);

            if (spans != null)
            {
                result1 = result3;
                span = spans[0];
            }

            VerifyDiagnostic(result1, span, analyzer, descriptor);

            //TODO: 
            CSharpCodeFixVerifier.Instance.VerifyFix(result1, result2, analyzer, fixProvider, options, cancellationToken);
        }

        public void VerifyDiagnostic(
            string source,
            TextSpan span,
            DiagnosticAnalyzer analyzer,
            DiagnosticDescriptor descriptor,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Location location = Location.Create(FileUtility.DefaultCSharpFileName, span, span.ToLinePositionSpan(source));

            Diagnostic diagnostic = Diagnostic.Create(descriptor, location);

            VerifyDiagnostic(source, analyzer, diagnostic, options, cancellationToken);
        }

        public void VerifyDiagnostic(
            string source,
            DiagnosticAnalyzer analyzer,
            Diagnostic diagnostic,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            VerifyDiagnostic(new string[] { source }, analyzer, new Diagnostic[] { diagnostic }, options, cancellationToken);
        }

        public void VerifyDiagnostic(
            string source,
            DiagnosticAnalyzer analyzer,
            Diagnostic[] diagnostics,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            VerifyDiagnostic(new string[] { source }, analyzer, diagnostics, options, cancellationToken);
        }

        public void VerifyDiagnostic(
            IEnumerable<string> sources,
            DiagnosticAnalyzer analyzer,
            Diagnostic[] diagnostics,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            VerifyDiagnosticAsync(sources, analyzer, diagnostics, options, cancellationToken).Wait();
        }

        public void VerifyNoDiagnostic(
            string source,
            string fixableCode,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            (string result, TextSpan span) = TextUtility.GetMarkedSpan(source, fixableCode);

            VerifyNoDiagnostic(result, descriptor, analyzer, options, cancellationToken);
        }

        public void VerifyNoDiagnostic(
            string source,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            VerifyNoDiagnostic(new string[] { source }, descriptor, analyzer, options, cancellationToken);
        }

        public void VerifyNoDiagnostic(
            IEnumerable<string> sources,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            VerifyNoDiagnosticAsync(sources, descriptor, analyzer, options, cancellationToken).Wait();
        }
    }
}
