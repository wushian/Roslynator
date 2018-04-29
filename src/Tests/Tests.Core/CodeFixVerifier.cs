// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace Roslynator.Tests
{
    public static class CodeFixVerifier
    {
        public static async Task VerifyFixAsync(
            string source,
            string expected,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fixProvider,
            string language,
            CodeVerificationSettings settings = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (settings == null)
                settings = CodeVerificationSettings.Default;

            Assert.True(fixProvider.CanFixAny(analyzer.SupportedDiagnostics), $"Code fix provider '{fixProvider.GetType().Name}' cannot fix any diagnostic supported by analyzer '{analyzer}'.");

            Document document = WorkspaceFactory.Document(source, language);

            Compilation compilation = await document.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

            DiagnosticVerifier.VerifyDiagnostics(compilerDiagnostics, settings.MaxAllowedCompilerDiagnosticSeverity);

            if (settings.EnableDiagnosticsDisabledByDefault)
                compilation = compilation.EnableDiagnosticsDisabledByDefault(analyzer);

            ImmutableArray<Diagnostic> analyzerDiagnostics = await compilation.GetAnalyzerDiagnosticsAsync(analyzer, cancellationToken).ConfigureAwait(false);

            ImmutableArray<string> fixableDiagnosticIds = fixProvider.FixableDiagnosticIds;

            while (analyzerDiagnostics.Length > 0)
            {
                Diagnostic diagnostic = FindFirstFixableDiagnostic(analyzerDiagnostics, fixableDiagnosticIds);

                if (diagnostic == null)
                    break;

                CodeAction action = null;

                var context = new CodeFixContext(
                    document,
                    diagnostic,
                    (a, diagnostics) =>
                    {
                        if (diagnostics.Contains(diagnostic)
                            && action == null)
                        {
                            action = a;
                        }
                    },
                    CancellationToken.None);

                await fixProvider.RegisterCodeFixesAsync(context).ConfigureAwait(false);

                if (action == null)
                    break;

                document = await document.ApplyCodeActionAsync(action).ConfigureAwait(false);

                compilation = await document.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

                ImmutableArray<Diagnostic> compilerDiagnostics2 = compilation.GetDiagnostics(cancellationToken);

                DiagnosticVerifier.VerifyDiagnostics(compilerDiagnostics2, settings.MaxAllowedCompilerDiagnosticSeverity);

                if (!settings.AllowNewCompilerDiagnostics)
                {
                    await DiagnosticVerifier.VerifyNoNewCompilerDiagnosticsAsync(document, compilerDiagnostics, compilerDiagnostics2).ConfigureAwait(false);
                }

                analyzerDiagnostics = await compilation.GetAnalyzerDiagnosticsAsync(analyzer, cancellationToken).ConfigureAwait(false);
            }

            string actual = await document.ToSimplifiedAndFormattedFullStringAsync().ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        //TODO: diagnosticId
        public static async Task VerifyNoFixAsync(
            string source,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fixProvider,
            string language,
            CodeVerificationSettings settings = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (settings == null)
                settings = CodeVerificationSettings.Default;

            Document document = WorkspaceFactory.Document(source, language);

            Compilation compilation = await document.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

            DiagnosticVerifier.VerifyDiagnostics(compilerDiagnostics, settings.MaxAllowedCompilerDiagnosticSeverity);

            if (settings.EnableDiagnosticsDisabledByDefault)
                compilation = compilation.EnableDiagnosticsDisabledByDefault(analyzer);

            ImmutableArray<Diagnostic> analyzerDiagnostics = await compilation.GetAnalyzerDiagnosticsAsync(analyzer, cancellationToken).ConfigureAwait(false);

            ImmutableArray<string> fixableDiagnosticIds = fixProvider.FixableDiagnosticIds;

            foreach (Diagnostic diagnostic in await compilation.GetAnalyzerDiagnosticsAsync(analyzer, cancellationToken).ConfigureAwait(false))
            {
                if (!fixableDiagnosticIds.Contains(diagnostic.Id))
                    return;

                var context = new CodeFixContext(
                    document,
                    diagnostic,
                    (_, __) => Assert.True(false, "Expected no code fix."),
                    CancellationToken.None);

                await fixProvider.RegisterCodeFixesAsync(context).ConfigureAwait(false);
            }
        }

        private static Diagnostic FindFirstFixableDiagnostic(ImmutableArray<Diagnostic> diagnostics, ImmutableArray<string> fixableDiagnosticIds)
        {
            foreach (Diagnostic diagnostic in diagnostics)
            {
                if (fixableDiagnosticIds.Contains(diagnostic.Id))
                {
                    return diagnostic;
                }
            }

            return null;
        }
    }
}
