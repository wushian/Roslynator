// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using static Roslynator.Test.CSharpCodeFixVerifier;

namespace Roslynator.Test
{
    public static class CSharpDiagnosticVerifier
    {
        public static void VerifyDiagnosticAndCodeFix(
            string source,
            string newSource,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider codeFixProvider,
            bool allowNewCompilerDiagnostics = false)
        {
            (string source2, List<Diagnostic> diagnostics) = TextUtility.GetSourceAndDiagnostics(source, descriptor, WorkspaceUtility.DefaultCSharpFileName);

            VerifyDiagnostic(source2, analyzer, diagnostics.ToArray());

            VerifyFix(source2, newSource, analyzer, codeFixProvider, allowNewCompilerDiagnostics);
        }

        public static void VerifyDiagnosticAndCodeFix(
            string source,
            string newSource,
            TextSpan span,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider codeFixProvider,
            bool allowNewCompilerDiagnostics = false)
        {
            VerifyDiagnostic(source, analyzer, span, descriptor);

            VerifyFix(source, newSource, analyzer, codeFixProvider, allowNewCompilerDiagnostics);
        }

        public static void VerifyDiagnosticAndCodeFix(
            string text,
            string codeWithDiagnostic,
            string codeWithFix,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider codeFixProvider,
            bool allowNewCompilerDiagnostics = false)
        {
            int index = text.IndexOf(TextUtility.OpenMarker + TextUtility.CloseMarker);

            var span = new TextSpan(index, codeWithDiagnostic.Length);

            text = text.Remove(index, TextUtility.OpenMarker.Length + TextUtility.CloseMarker.Length);

            string source = text.Insert(index, codeWithDiagnostic);

            string newSource = text.Insert(index, codeWithFix);

            VerifyDiagnostic(source, analyzer, span, descriptor);

            VerifyFix(source, newSource, analyzer, codeFixProvider, allowNewCompilerDiagnostics);
        }

        public static void VerifyDiagnostic(
            string source,
            DiagnosticAnalyzer analyzer,
            TextSpan span,
            DiagnosticDescriptor descriptor)
        {
            Location location = Location.Create(WorkspaceUtility.DefaultCSharpFileName, span, span.ToLinePositionSpan(source));

            Diagnostic diagnostic = Diagnostic.Create(descriptor, location);

            VerifyDiagnostic(source, analyzer, diagnostic);
        }

        public static void VerifyDiagnostic(
            string source,
            DiagnosticAnalyzer analyzer,
            params Diagnostic[] expected)
        {
            VerifyDiagnostic(new string[] { source }, analyzer, expected);
        }

        public static void VerifyDiagnostic(
            IEnumerable<string> sources,
            DiagnosticAnalyzer analyzer,
            params Diagnostic[] expected)
        {
            DiagnosticVerifier.VerifyDiagnostic(sources, analyzer, LanguageNames.CSharp, expected);
        }

        public static void VerifyNoDiagnostic(
            string sourceTemplate,
            string source,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer)
        {
            int index = sourceTemplate.IndexOf(TextUtility.OpenMarker + TextUtility.CloseMarker);

            source = sourceTemplate.Remove(index, TextUtility.OpenMarker.Length + TextUtility.CloseMarker.Length).Insert(index, source);

            VerifyNoDiagnostic(source, descriptor, analyzer);
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
            DiagnosticVerifier.VerifyNoDiagnostic(sources, descriptor, analyzer, LanguageNames.CSharp);
        }
    }
}
