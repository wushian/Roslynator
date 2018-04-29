// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using static Roslynator.Tests.CSharp.CSharpCodeFixVerifier;

namespace Roslynator.Tests.CSharp
{
    public static class CSharpDiagnosticVerifier
    {
        public static void VerifyDiagnosticAndFix(
            string source,
            string expected,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fixProvider,
            CodeVerificationSettings settings = null)
        {
            (string result, List<Diagnostic> diagnostics) = TextUtility.GetMarkedDiagnostics(source, descriptor, FileUtility.DefaultCSharpFileName);

            VerifyDiagnostic(result, analyzer, diagnostics.ToArray());

            VerifyFix(result, expected, analyzer, fixProvider, settings);
        }

        public static void VerifyDiagnosticAndFix(
            string source,
            string expected,
            TextSpan span,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fixProvider,
            CodeVerificationSettings settings = null)
        {
            VerifyDiagnostic(source, span, analyzer, descriptor);

            VerifyFix(source, expected, analyzer, fixProvider, settings);
        }

        public static void VerifyDiagnosticAndFix(
            string source,
            string fixableCode,
            string fixedCode,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fixProvider,
            CodeVerificationSettings settings = null)
        {
            (string result1, string result2, TextSpan span) = TextUtility.GetMarkedSpan(source, fixableCode, fixedCode);

            (string result3, List<TextSpan> spans) = TextUtility.GetMarkedSpans(result1);

            if (spans != null)
            {
                result1 = result3;
                span = spans[0];
            }

            VerifyDiagnostic(result1, span, analyzer, descriptor);

            VerifyFix(result1, result2, analyzer, fixProvider, settings);
        }

        public static void VerifyDiagnostic(
            string source,
            TextSpan span,
            DiagnosticAnalyzer analyzer,
            DiagnosticDescriptor descriptor)
        {
            Location location = Location.Create(FileUtility.DefaultCSharpFileName, span, span.ToLinePositionSpan(source));

            Diagnostic diagnostic = Diagnostic.Create(descriptor, location);

            VerifyDiagnostic(source, analyzer, diagnostic);
        }

        public static void VerifyDiagnostic(
            string source,
            DiagnosticAnalyzer analyzer,
            params Diagnostic[] expectedDiagnostics)
        {
            VerifyDiagnostic(new string[] { source }, analyzer, expectedDiagnostics);
        }

        public static void VerifyDiagnostic(
            IEnumerable<string> sources,
            DiagnosticAnalyzer analyzer,
            params Diagnostic[] expectedDiagnostics)
        {
            DiagnosticVerifier.VerifyDiagnosticAsync(sources, analyzer, LanguageNames.CSharp, expectedDiagnostics).Wait();
        }

        public static void VerifyNoDiagnostic(
            string source,
            string fixableCode,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer)
        {
            (string result, TextSpan span) = TextUtility.GetMarkedSpan(source, fixableCode);

            VerifyNoDiagnostic(fixableCode, descriptor, analyzer);
        }

        public static void VerifyNoDiagnostic(
            string source,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer)
        {
            VerifyNoDiagnostic(new string[] { source }, descriptor, analyzer);
        }

        public static void VerifyNoDiagnostic(
            IEnumerable<string> sources,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer)
        {
            DiagnosticVerifier.VerifyNoDiagnosticAsync(sources, descriptor, analyzer, LanguageNames.CSharp).Wait();
        }
    }
}
