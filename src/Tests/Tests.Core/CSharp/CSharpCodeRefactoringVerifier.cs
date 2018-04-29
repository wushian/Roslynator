// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Tests.CSharp
{
    public static class CSharpCodeRefactoringVerifier
    {
        public static void VerifyRefactoring(
            string source,
            string expected,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null,
            CodeVerificationSettings settings = null)
        {
            (string result, List<TextSpan> spans) = TextUtility.GetMarkedSpans(source);

            VerifyRefactoring(
                source: result,
                expected: expected,
                spans: spans.ToImmutableArray(),
                refactoringProvider: refactoringProvider,
                equivalenceKey: equivalenceKey,
                settings: settings);
        }

        public static void VerifyRefactoring(
            string source,
            string fixableCode,
            string fixedCode,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null,
            CodeVerificationSettings settings = null)
        {
            (string result1, string result2, TextSpan span) = TextUtility.GetMarkedSpan(source, fixableCode, fixedCode);

            (string result3, List<TextSpan> spans) = TextUtility.GetMarkedSpans(result1);

            if (spans != null)
            {
                result1 = result3;
                span = spans[0];
            }

            VerifyRefactoring(
                source: result1,
                expected: result2,
                span: span,
                refactoringProvider: refactoringProvider,
                equivalenceKey: equivalenceKey,
                settings: settings);
        }

        public static void VerifyRefactoring(
            string source,
            string expected,
            TextSpan span,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null,
            CodeVerificationSettings settings = null)
        {
            CodeRefactoringVerifier.VerifyRefactoringAsync(
                source: source,
                expected: expected,
                span: span,
                refactoringProvider: refactoringProvider,
                language: LanguageNames.CSharp,
                equivalenceKey: equivalenceKey,
                settings: settings).Wait();
        }

        public static void VerifyRefactoring(
            string source,
            string expected,
            ImmutableArray<TextSpan> spans,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null,
            CodeVerificationSettings settings = null)
        {
            CodeRefactoringVerifier.VerifyRefactoringAsync(
                source: source,
                expected: expected,
                spans: spans,
                refactoringProvider: refactoringProvider,
                language: LanguageNames.CSharp,
                equivalenceKey: equivalenceKey,
                settings: settings).Wait();
        }

        public static void VerifyNoRefactoring(
            string source,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null)
        {
            (string source2, List<TextSpan> spans) = TextUtility.GetMarkedSpans(source);

            CodeRefactoringVerifier.VerifyNoRefactoringAsync(
                source: source2,
                spans: spans,
                refactoringProvider: refactoringProvider,
                language: LanguageNames.CSharp,
                equivalenceKey: equivalenceKey).Wait();
        }

        public static void VerifyNoRefactoring(
            string source,
            TextSpan span,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null)
        {
            CodeRefactoringVerifier.VerifyNoRefactoringAsync(
                source: source,
                span: span,
                refactoringProvider: refactoringProvider,
                language: LanguageNames.CSharp,
                equivalenceKey: equivalenceKey).Wait();
        }
    }
}
