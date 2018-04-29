// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Roslynator.Tests
{
    public static class CodeRefactoringVerifier
    {
        public static async Task VerifyRefactoringAsync(
            string source,
            string expected,
            TextSpan span,
            CodeRefactoringProvider refactoringProvider,
            string language,
            string equivalenceKey = null,
            CodeVerificationSettings settings = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyRefactoringAsync(
                source: source,
                expected: expected,
                spans: ImmutableArray.Create(span),
                refactoringProvider: refactoringProvider,
                language: language,
                equivalenceKey: equivalenceKey,
                settings: settings,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public static async Task VerifyRefactoringAsync(
            string source,
            string expected,
            ImmutableArray<TextSpan> spans,
            CodeRefactoringProvider refactoringProvider,
            string language,
            string equivalenceKey = null,
            CodeVerificationSettings settings = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (settings == null)
                settings = CodeVerificationSettings.Default;

            Document document = WorkspaceFactory.Document(source, language);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

            DiagnosticVerifier.VerifyDiagnostics(compilerDiagnostics, settings.MaxAllowedCompilerDiagnosticSeverity);

            spans = spans.Sort((x, y) => y.Start.CompareTo(x.Start));

            foreach (TextSpan span in spans)
            {
                CodeAction action = null;

                var context = new CodeRefactoringContext(
                    document,
                    span,
                    a =>
                    {
                        if (equivalenceKey == null
                            || string.Equals(a.EquivalenceKey, equivalenceKey, StringComparison.Ordinal))
                        {
                            if (action != null)
                            {
                                action = a;
                            }
                        }
                    },
                    CancellationToken.None);

                refactoringProvider.ComputeRefactoringsAsync(context).Wait();

                Assert.True(action != null, "No code refactoring has been registered.");

                document = await document.ApplyCodeActionAsync(action).ConfigureAwait(false);

                semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                ImmutableArray<Diagnostic> compilerDiagnostics2 = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

                DiagnosticVerifier.VerifyDiagnostics(compilerDiagnostics2, settings.MaxAllowedCompilerDiagnosticSeverity);

                if (!settings.AllowNewCompilerDiagnostics)
                {
                    await DiagnosticVerifier.VerifyNoNewCompilerDiagnosticsAsync(document, compilerDiagnostics, compilerDiagnostics2).ConfigureAwait(false);
                }
            }

            string actual = await document.ToSimplifiedAndFormattedFullStringAsync().ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        public static async Task VerifyNoRefactoringAsync(
            string source,
            TextSpan span,
            CodeRefactoringProvider refactoringProvider,
            string language,
            string equivalenceKey = null,
            CodeVerificationSettings settings = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyNoRefactoringAsync(
                source,
                ImmutableArray.Create(span),
                refactoringProvider,
                language,
                equivalenceKey,
                settings,
                cancellationToken).ConfigureAwait(false);
        }

        public static async Task VerifyNoRefactoringAsync(
            string source,
            IEnumerable<TextSpan> spans,
            CodeRefactoringProvider refactoringProvider,
            string language,
            string equivalenceKey = null,
            CodeVerificationSettings settings = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (settings == null)
                settings = CodeVerificationSettings.Default;

            Document document = WorkspaceFactory.Document(source, language);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

            DiagnosticVerifier.VerifyDiagnostics(compilerDiagnostics, settings.MaxAllowedCompilerDiagnosticSeverity);

            foreach (TextSpan span in spans)
            {
                var context = new CodeRefactoringContext(
                    document,
                    span,
                    codeAction =>
                    {
                        if (equivalenceKey == null
                            || string.Equals(codeAction.EquivalenceKey, equivalenceKey, StringComparison.Ordinal))
                        {
                            Assert.True(false, "Expected no code refactoring.");
                        }
                    },
                    CancellationToken.None);

                await refactoringProvider.ComputeRefactoringsAsync(context).ConfigureAwait(false);
            }
        }
    }
}
