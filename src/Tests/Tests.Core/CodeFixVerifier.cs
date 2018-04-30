// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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
    public abstract class CodeFixVerifier : CodeVerifier
    {
        public async Task VerifyFixAsync(
            string source,
            string expected,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fixProvider,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (options == null)
                options = CodeVerificationOptions.Default;

            Assert.True(fixProvider.CanFixAny(analyzer.SupportedDiagnostics), $"Code fix provider '{fixProvider.GetType().Name}' cannot fix any diagnostic supported by analyzer '{analyzer}'.");

            Document document = WorkspaceFactory.Document(source, Language);

            Compilation compilation = await document.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

            DiagnosticVerifier.VerifyDiagnostics(compilerDiagnostics, options.MaxAllowedCompilerDiagnosticSeverity);

            if (options.EnableDiagnosticsDisabledByDefault)
                compilation = compilation.EnableDiagnosticsDisabledByDefault(analyzer);

            ImmutableArray<Diagnostic> diagnostics = await compilation.GetAnalyzerDiagnosticsAsync(analyzer, DiagnosticComparer.SpanStart, cancellationToken).ConfigureAwait(false);

            ImmutableArray<string> fixableDiagnosticIds = fixProvider.FixableDiagnosticIds;

            while (diagnostics.Length > 0)
            {
                Diagnostic diagnostic = FindFirstFixableDiagnostic();

                if (diagnostic == null)
                    break;

                CodeAction action = null;

                var context = new CodeFixContext(
                    document,
                    diagnostic,
                    (a, d) =>
                    {
                        if (d.Contains(diagnostic)
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

                if (!options.AllowNewCompilerDiagnostics)
                {
                    DiagnosticVerifier.VerifyNoNewCompilerDiagnostics(
                        compilerDiagnostics,
                        compilation.GetDiagnostics(cancellationToken));
                }

                if (options.EnableDiagnosticsDisabledByDefault)
                    compilation = compilation.EnableDiagnosticsDisabledByDefault(analyzer);

                diagnostics = await compilation.GetAnalyzerDiagnosticsAsync(analyzer, DiagnosticComparer.SpanStart, cancellationToken).ConfigureAwait(false);
            }

            string actual = await document.ToSimplifiedAndFormattedFullStringAsync().ConfigureAwait(false);

            Assert.Equal(expected, actual);

            Diagnostic FindFirstFixableDiagnostic()
            {
                foreach (Diagnostic diagnostic in diagnostics)
                {
                    if (fixableDiagnosticIds.Contains(diagnostic.Id))
                        return diagnostic;
                }

                return null;
            }
        }

        public async Task VerifyNoFixAsync(
            string source,
            string diagnosticId,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fixProvider,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (options == null)
                options = CodeVerificationOptions.Default;

            Document document = WorkspaceFactory.Document(source, Language);

            Compilation compilation = await document.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

            DiagnosticVerifier.VerifyDiagnostics(compilerDiagnostics, options.MaxAllowedCompilerDiagnosticSeverity);

            if (options.EnableDiagnosticsDisabledByDefault)
                compilation = compilation.EnableDiagnosticsDisabledByDefault(analyzer);

            ImmutableArray<Diagnostic> diagnostics = await compilation.GetAnalyzerDiagnosticsAsync(analyzer, DiagnosticComparer.SpanStart, cancellationToken).ConfigureAwait(false);

            ImmutableArray<string> fixableDiagnosticIds = fixProvider.FixableDiagnosticIds;

            foreach (Diagnostic diagnostic in diagnostics)
            {
                if (!string.Equals(diagnostic.Id, diagnosticId, StringComparison.Ordinal))
                    continue;

                if (!fixableDiagnosticIds.Contains(diagnostic.Id))
                    continue;

                var context = new CodeFixContext(
                    document,
                    diagnostic,
                    (_, d) => Assert.True(!d.Contains(diagnostic), "Expected no code fix."),
                    CancellationToken.None);

                await fixProvider.RegisterCodeFixesAsync(context).ConfigureAwait(false);
            }
        }
    }
}
