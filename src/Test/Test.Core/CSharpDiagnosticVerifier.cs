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
            string sourceTemplate,
            string source,
            string newSource,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider codeFixProvider,
            bool allowNewCompilerDiagnostics = false)
        {
            int index = sourceTemplate.IndexOf("<<>>");

            var span = new TextSpan(index, source.Length);

            sourceTemplate = sourceTemplate.Remove(index, 4);

            source = sourceTemplate.Insert(index, source);

            newSource = sourceTemplate.Insert(index, newSource);

            VerifyDiagnostic(source, analyzer, span, descriptor);

            VerifyFix(source, newSource, analyzer, codeFixProvider, allowNewCompilerDiagnostics);
        }

        public static void VerifyNoDiagnostic(
            string source,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer)
        {
            VerifyNoDiagnostic(source, descriptor, analyzer);
        }

        public static void VerifyNoDiagnostic(
            string sourceTemplate,
            string source,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer)
        {
            int index = sourceTemplate.IndexOf("<<>>");

            source = sourceTemplate.Remove(index, 4).Insert(index, source);

            DiagnosticVerifier.VerifyNoDiagnostic(source, analyzer, descriptor, LanguageNames.CSharp);
        }

        public static void VerifyDiagnostic(
            string source,
            DiagnosticAnalyzer analyzer,
            TextSpan span,
            DiagnosticDescriptor descriptor)
        {
            Location location = Location.Create(TestUtility.CreateFileName(), span, span.ToLinePositionSpan(source));

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
    }
}
