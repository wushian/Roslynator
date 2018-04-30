// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Tests.CSharp
{
    public class CSharpCodeRefactoringVerifier : CodeRefactoringVerifier
    {
        public static CSharpCodeRefactoringVerifier Instance { get; } = new CSharpCodeRefactoringVerifier();

        public override string Language
        {
            get { return LanguageNames.CSharp; }
        }

        public void VerifyRefactoring(
            string source,
            string expected,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            VerifyRefactoring(
                source: source,
                additionalSources: Array.Empty<string>(),
                expected: expected,
                refactoringProvider: refactoringProvider,
                equivalenceKey: equivalenceKey,
                options: options,
                cancellationToken: cancellationToken);
        }

        public void VerifyRefactoring(
            string source,
            string[] additionalSources,
            string expected,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            (string result, List<TextSpan> spans) = TextUtility.GetMarkedSpans(source);

            VerifyRefactoringAsync(
                source: result,
                additionalSources: additionalSources,
                expected: expected,
                spans: spans.ToImmutableArray(),
                refactoringProvider: refactoringProvider,
                equivalenceKey: equivalenceKey,
                options: options,
                cancellationToken: cancellationToken).Wait();
        }

        public void VerifyRefactoring(
            string source,
            string fixableCode,
            string fixedCode,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null,
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

            VerifyRefactoring(
                source: result1,
                expected: result2,
                span: span,
                refactoringProvider: refactoringProvider,
                equivalenceKey: equivalenceKey,
                options: options,
                cancellationToken: cancellationToken);
        }

        public void VerifyRefactoring(
            string source,
            string expected,
            TextSpan span,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            VerifyRefactoringAsync(
                source: source,
                expected: expected,
                span: span,
                refactoringProvider: refactoringProvider,
                equivalenceKey: equivalenceKey,
                options: options,
                cancellationToken: cancellationToken).Wait();
        }

        public void VerifyRefactoring(
            string source,
            string expected,
            ImmutableArray<TextSpan> spans,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            VerifyRefactoringAsync(
                source: source,
                expected: expected,
                spans: spans,
                refactoringProvider: refactoringProvider,
                equivalenceKey: equivalenceKey,
                options: options,
                cancellationToken: cancellationToken).Wait();
        }

        public void VerifyNoRefactoring(
            string source,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            (string source2, List<TextSpan> spans) = TextUtility.GetMarkedSpans(source);

            VerifyNoRefactoringAsync(
                source: source2,
                spans: spans,
                refactoringProvider: refactoringProvider,
                equivalenceKey: equivalenceKey,
                options: options,
                cancellationToken: cancellationToken).Wait();
        }

        public void VerifyNoRefactoring(
            string source,
            TextSpan span,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            VerifyNoRefactoringAsync(
                source: source,
                span: span,
                refactoringProvider: refactoringProvider,
                equivalenceKey: equivalenceKey,
                options: options,
                cancellationToken: cancellationToken).Wait();
        }
    }
}
