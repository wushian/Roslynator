// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
        public abstract string RefactoringId { get; }

        public abstract CodeRefactoringProvider RefactoringProvider { get; }

        public async Task VerifyRefactoringAsync(
            string source,
            string expected,
            string equivalenceKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyRefactoringAsync(
                source: source,
                additionalSources: Array.Empty<string>(),
                expected: expected,
                equivalenceKey: equivalenceKey,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        //TODO: additionalSources
        public async Task VerifyRefactoringAsync(
            string source,
            string[] additionalSources,
            string expected,
            string equivalenceKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ParseResult result = TextUtility.GetSpans(source);

            await VerifyRefactoringAsync(
                source: result.Source,
                additionalSources: additionalSources,
                expected: expected,
                spans: result.Spans.Select(f => f.Span).ToImmutableArray(),
                equivalenceKey: equivalenceKey,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyRefactoringAsync(
            string source,
            string fixableCode,
            string fixedCode,
            string equivalenceKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            (string result1, string result2, TextSpan span) = TextUtility.GetMarkedSpan(source, fixableCode, fixedCode);

            ParseResult result = TextUtility.GetSpans(result1);

            if (result.Spans.Any())
            {
                await VerifyRefactoringAsync(
                    source: result.Source,
                    expected: result2,
                    spans: result.Spans.Select(f => f.Span).ToArray(),
                    equivalenceKey: equivalenceKey,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await VerifyRefactoringAsync(
                    source: result1,
                    expected: result2,
                    span: span,
                    equivalenceKey: equivalenceKey,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task VerifyRefactoringAsync(
            string source,
            string expected,
            TextSpan span,
            string equivalenceKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyRefactoringAsync(
                source: source,
                additionalSources: Array.Empty<string>(),
                expected: expected,
                span: span,
                equivalenceKey: equivalenceKey,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyRefactoringAsync(
            string source,
            string[] additionalSources,
            string expected,
            TextSpan span,
            string equivalenceKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyRefactoringAsync(
                source: source,
                additionalSources: additionalSources,
                expected: expected,
                spans: ImmutableArray.Create(span),
                equivalenceKey: equivalenceKey,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyRefactoringAsync(
            string source,
            string expected,
            IList<TextSpan> spans,
            string equivalenceKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyRefactoringAsync(
                source: source,
                additionalSources: Array.Empty<string>(),
                expected: expected,
                spans: spans,
                equivalenceKey: equivalenceKey,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyRefactoringAsync(
            string source,
            string[] additionalSources,
            string expected,
            IList<TextSpan> spans,
            string equivalenceKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Document document = WorkspaceFactory.Document(source, additionalSources, Language);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

            DiagnosticVerifier.VerifyDiagnostics(compilerDiagnostics, Options.MaxAllowedCompilerDiagnosticSeverity);

            foreach (TextSpan span in spans.OrderByDescending(f => f.Start))
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

                await RefactoringProvider.ComputeRefactoringsAsync(context).ConfigureAwait(false);

                Assert.True(action != null, "No code refactoring has been registered.");

                document = await document.ApplyCodeActionAsync(action).ConfigureAwait(false);

                semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                ImmutableArray<Diagnostic> newCompilerDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

                DiagnosticVerifier.VerifyDiagnostics(newCompilerDiagnostics, Options.MaxAllowedCompilerDiagnosticSeverity);

                if (!Options.AllowNewCompilerDiagnostics)
                    DiagnosticVerifier.VerifyNoNewCompilerDiagnostics(compilerDiagnostics, newCompilerDiagnostics);
            }

            string actual = await document.ToFullStringAsync(simplify: true, format: true).ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        public async Task VerifyNoRefactoringAsync(
            string source,
            string equivalenceKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ParseResult result = TextUtility.GetSpans(source);

            await VerifyNoRefactoringAsync(
                source: result.Source,
                spans: result.Spans.Select(f => f.Span).ToArray(),
                equivalenceKey: equivalenceKey,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyNoRefactoringAsync(
            string source,
            TextSpan span,
            string equivalenceKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyNoRefactoringAsync(
                source,
                ImmutableArray.Create(span),
                equivalenceKey,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyNoRefactoringAsync(
            string source,
            IEnumerable<TextSpan> spans,
            string equivalenceKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Document document = WorkspaceFactory.Document(source, Language);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

            DiagnosticVerifier.VerifyDiagnostics(compilerDiagnostics, Options.MaxAllowedCompilerDiagnosticSeverity);

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

                await RefactoringProvider.ComputeRefactoringsAsync(context).ConfigureAwait(false);
            }
        }
    }
}
