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
    public abstract class CompilerCodeFixVerifier : CodeVerifier
    {
        public async Task VerifyFixAsync(
            string source,
            string expected,
            string diagnosticId,
            CodeFixProvider fixProvider,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (options == null)
                options = CodeVerificationOptions.Default;

            Assert.True(fixProvider.FixableDiagnosticIds.Contains(diagnosticId), $"Code fix provider '{fixProvider.GetType().Name}' cannot fix diagnostic '{diagnosticId}'.");

            Document document = WorkspaceFactory.Document(source, Language);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> diagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

            while (diagnostics.Length > 0)
            {
                Diagnostic diagnostic = FindDiagnostic();

                if (diagnostic == null)
                    break;

                CodeAction action = null;

                var context = new CodeFixContext(
                    document,
                    diagnostic,
                    (a, d) =>
                    {
                        if (action != null)
                            return;

                        if (!d.Contains(diagnostic))
                            return;

                        if (equivalenceKey != null
                            && !string.Equals(a.EquivalenceKey, equivalenceKey, StringComparison.Ordinal))
                        {
                            return;
                        }

                        action = a;
                    },
                    CancellationToken.None);

                await fixProvider.RegisterCodeFixesAsync(context).ConfigureAwait(false);

                if (action == null)
                    break;

                document = await document.ApplyCodeActionAsync(action).ConfigureAwait(false);

                semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                diagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);
            }

            string actual = await document.ToSimplifiedAndFormattedFullStringAsync().ConfigureAwait(false);

            Assert.Equal(expected, actual);

            Diagnostic FindDiagnostic()
            {
                foreach (Diagnostic diagnostic in diagnostics)
                {
                    if (string.Equals(diagnostic.Id, diagnosticId, StringComparison.Ordinal))
                        return diagnostic;
                }

                return null;
            }
        }

        public async Task VerifyNoFixAsync(
            string source,
            CodeFixProvider fixProvider,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (options == null)
                options = CodeVerificationOptions.Default;

            Document document = WorkspaceFactory.Document(source, Language);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<string> fixableDiagnosticIds = fixProvider.FixableDiagnosticIds;

            foreach (Diagnostic diagnostic in semanticModel.GetDiagnostics(cancellationToken: cancellationToken))
            {
                if (!fixableDiagnosticIds.Contains(diagnostic.Id))
                    continue;

                var context = new CodeFixContext(
                    document,
                    diagnostic,
                    (a, d) =>
                    {
                        if (!d.Contains(diagnostic))
                            return;

                        if (equivalenceKey != null
                            && !string.Equals(a.EquivalenceKey, equivalenceKey, StringComparison.Ordinal))
                        {
                            return;
                        }

                        Assert.True(false, "Expected no code fix.");
                    },
                    CancellationToken.None);

                await fixProvider.RegisterCodeFixesAsync(context).ConfigureAwait(false);
            }
        }
    }
}
