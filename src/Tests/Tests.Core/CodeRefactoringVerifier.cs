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
    public abstract class CodeRefactoringVerifier : CodeVerifier
    {
        public async Task VerifyRefactoringAsync(
            string source,
            string expected,
            TextSpan span,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyRefactoringAsync(
                source: source,
                additionalSources: Array.Empty<string>(),
                expected: expected,
                span: span,
                refactoringProvider: refactoringProvider,
                equivalenceKey: equivalenceKey,
                options: options,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyRefactoringAsync(
            string source,
            string[] additionalSources,
            string expected,
            TextSpan span,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyRefactoringAsync(
                source: source,
                additionalSources: additionalSources,
                expected: expected,
                spans: ImmutableArray.Create(span),
                refactoringProvider: refactoringProvider,
                equivalenceKey: equivalenceKey,
                options: options,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyRefactoringAsync(
            string source,
            string expected,
            ImmutableArray<TextSpan> spans,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyRefactoringAsync(
                source: source,
                additionalSources: Array.Empty<string>(),
                expected: expected,
                spans: spans,
                refactoringProvider: refactoringProvider,
                equivalenceKey: equivalenceKey,
                options: options,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyRefactoringAsync(
            string source,
            string[] additionalSources,
            string expected,
            ImmutableArray<TextSpan> spans,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (options == null)
                options = CodeVerificationOptions.Default;

            Document document = WorkspaceFactory.Document(source, additionalSources, Language);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

            DiagnosticVerifier.VerifyDiagnostics(compilerDiagnostics, options.MaxAllowedCompilerDiagnosticSeverity);

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
                            if (action == null)
                                action = a;
                        }
                    },
                    CancellationToken.None);

                await refactoringProvider.ComputeRefactoringsAsync(context).ConfigureAwait(false);

                Assert.True(action != null, "No code refactoring has been registered.");

                document = await document.ApplyCodeActionAsync(action).ConfigureAwait(false);

                semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                ImmutableArray<Diagnostic> newCompilerDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

                DiagnosticVerifier.VerifyDiagnostics(newCompilerDiagnostics, options.MaxAllowedCompilerDiagnosticSeverity);

                if (!options.AllowNewCompilerDiagnostics)
                    DiagnosticVerifier.VerifyNoNewCompilerDiagnostics(compilerDiagnostics, newCompilerDiagnostics);
            }

            string actual = await document.ToSimplifiedAndFormattedFullStringAsync().ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        public async Task VerifyNoRefactoringAsync(
            string source,
            TextSpan span,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyNoRefactoringAsync(
                source,
                ImmutableArray.Create(span),
                refactoringProvider,
                equivalenceKey,
                options,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyNoRefactoringAsync(
            string source,
            IEnumerable<TextSpan> spans,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (options == null)
                options = CodeVerificationOptions.Default;

            Document document = WorkspaceFactory.Document(source, Language);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

            DiagnosticVerifier.VerifyDiagnostics(compilerDiagnostics, options.MaxAllowedCompilerDiagnosticSeverity);

            foreach (TextSpan span in spans)
            {
                var context = new CodeRefactoringContext(
                    document,
                    span,
                    a =>
                    {
                        if (equivalenceKey == null
                            || string.Equals(a.EquivalenceKey, equivalenceKey, StringComparison.Ordinal))
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
