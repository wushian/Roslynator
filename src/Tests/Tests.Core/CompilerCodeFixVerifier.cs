// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Xunit;

namespace Roslynator.Tests
{
    public static class CompilerCodeFixVerifier
    {
        public static async Task VerifyFixAsync(
            string source,
            string expected,
            string diagnosticId,
            CodeFixProvider fixProvider,
            string language,
            string equivalenceKey = null,
            CodeVerificationSettings settings = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (settings == null)
                settings = CodeVerificationSettings.Default;

            Assert.True(fixProvider.FixableDiagnosticIds.Contains(diagnosticId), $"Code fix provider '{fixProvider.GetType().Name}' cannot fix diagnostic '{diagnosticId}'.");

            Document document = WorkspaceFactory.Document(source, language);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

            while (compilerDiagnostics.Length > 0)
            {
                Diagnostic diagnostic = FindDiagnostic(compilerDiagnostics, diagnosticId);

                if (diagnostic == null)
                    break;

                CodeAction action = null;

                var context = new CodeFixContext(
                    document,
                    diagnostic,
                    (a, _) =>
                    {
                        if (equivalenceKey == null
                            || string.Equals(a.EquivalenceKey, equivalenceKey, StringComparison.Ordinal))
                        {
                            if (action == null)
                                action = a;
                        }
                    },
                    CancellationToken.None);

                await fixProvider.RegisterCodeFixesAsync(context).ConfigureAwait(false);

                if (action == null)
                    break;

                document = await document.ApplyCodeActionAsync(action).ConfigureAwait(false);

                semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                compilerDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);
            }

            string actual = await document.ToSimplifiedAndFormattedFullStringAsync().ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        //TODO: string diagnosticId
        public static async Task VerifyNoFixAsync(
            string source,
            CodeFixProvider fixProvider,
            string language,
            string equivalenceKey = null,
            CodeVerificationSettings settings = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (settings == null)
                settings = CodeVerificationSettings.Default;

            Document document = WorkspaceFactory.Document(source, language);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            foreach (Diagnostic diagnostic in semanticModel.GetDiagnostics(cancellationToken: cancellationToken))
            {
                var context = new CodeFixContext(
                    document,
                    diagnostic,
                    (a, _) =>
                    {
                        Assert.True(
                            equivalenceKey != null && !string.Equals(a.EquivalenceKey, equivalenceKey, StringComparison.Ordinal),
                            "Expected no code fix.");
                    },
                    CancellationToken.None);

                await fixProvider.RegisterCodeFixesAsync(context).ConfigureAwait(false);
            }
        }

        private static Diagnostic FindDiagnostic(ImmutableArray<Diagnostic> diagnostics, string diagnosticId)
        {
            foreach (Diagnostic diagnostic in diagnostics)
            {
                if (string.Equals(diagnostic.Id, diagnosticId, StringComparison.Ordinal))
                    return diagnostic;
            }

            return null;
        }
    }
}
