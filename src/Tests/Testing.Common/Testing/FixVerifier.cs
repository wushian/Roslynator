﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Testing.Text;

namespace Roslynator.Testing
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class FixVerifier : DiagnosticVerifier
    {
        private ImmutableArray<string> _fixableDiagnosticIds;

        internal FixVerifier(WorkspaceFactory workspaceFactory) : base(workspaceFactory)
        {
        }

        public abstract CodeFixProvider FixProvider { get; }

        internal ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                if (_fixableDiagnosticIds.IsDefault)
                    ImmutableInterlocked.InterlockedInitialize(ref _fixableDiagnosticIds, FixProvider.FixableDiagnosticIds);

                return _fixableDiagnosticIds;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{Descriptor.Id} {Analyzer.GetType().Name} {FixProvider.GetType().Name}"; }
        }

        public async Task VerifyDiagnosticAndFixAsync(
            string source,
            string expected,
            IEnumerable<(string source, string expected)> additionalData = null,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default)
        {
            TextParserResult result = TextParser.GetSpans(source);

            IEnumerable<Diagnostic> diagnostics = result.Spans.Select(f => CreateDiagnostic(f.Span, f.LineSpan));

            string[] additionalSources = additionalData?.Select(f => f.source).ToArray() ?? Array.Empty<string>();

            await VerifyDiagnosticAsync(
                result.Text,
                diagnostics,
                additionalSources: additionalSources,
                options: options,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            await VerifyFixAsync(
                result.Text,
                expected,
                additionalData,
                equivalenceKey,
                options,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyDiagnosticAndFixAsync(
            string source,
            string inlineSource,
            string inlineExpected,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default)
        {
            (TextSpan span, string source2, string expected) = TextParser.ReplaceEmptySpan(source, inlineSource, inlineExpected);

            TextParserResult result = TextParser.GetSpans(source2);

            if (result.Spans.Any())
            {
                IEnumerable<Diagnostic> diagnostics = result.Spans.Select(f => CreateDiagnostic(f.Span, f.LineSpan));

                await VerifyDiagnosticAsync(result.Text, diagnostics, additionalSources: null, options: options, cancellationToken).ConfigureAwait(false);

                await VerifyFixAsync(result.Text, expected, additionalData: null, equivalenceKey: equivalenceKey, options: options, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await VerifyDiagnosticAsync(source2, span, options, cancellationToken).ConfigureAwait(false);

                await VerifyFixAsync(source2, expected, additionalData: null, equivalenceKey: equivalenceKey, options: options, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task VerifyFixAsync(
            string source,
            string inlineSource,
            string inlineExpected,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default)
        {
            (_, string source2, string expected) = TextParser.ReplaceEmptySpan(source, inlineSource, inlineExpected);

            await VerifyFixAsync(
                source: source2,
                expected: expected,
                equivalenceKey: equivalenceKey,
                options: options,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyFixAsync(
            string source,
            string expected,
            IEnumerable<(string source, string expected)> additionalData = null,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            options ??= Options;

            if (!SupportedDiagnostics.Contains(Descriptor, DiagnosticDescriptorComparer.Id))
                Assert.True(false, $"Diagnostic '{Descriptor.Id}' is not supported by analyzer '{Analyzer.GetType().Name}'.");

            if (!FixableDiagnosticIds.Contains(Descriptor.Id))
                Assert.True(false, $"Diagnostic '{Descriptor.Id}' is not fixable by code fix provider '{FixProvider.GetType().Name}'.");

            using (Workspace workspace = new AdhocWorkspace())
            {
                Project project = WorkspaceFactory.AddProject(workspace.CurrentSolution, options);

                Document document = WorkspaceFactory.AddDocument(project, source);

                project = document.Project;

                ImmutableArray<ExpectedDocument> expectedDocuments = (additionalData != null)
                    ? WorkspaceFactory.AddAdditionalDocuments(additionalData, ref project)
                    : ImmutableArray<ExpectedDocument>.Empty;

                document = project.GetDocument(document.Id);

                Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

                ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

                VerifyCompilerDiagnostics(compilerDiagnostics, options);

                if (!Descriptor.IsEnabledByDefault)
                    compilation = compilation.EnsureEnabled(Descriptor);

                ImmutableArray<Diagnostic> previousDiagnostics = ImmutableArray<Diagnostic>.Empty;

                bool fixRegistered = false;

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    ImmutableArray<Diagnostic> diagnostics = await compilation.GetAnalyzerDiagnosticsAsync(Analyzer, DiagnosticComparer.SpanStart, cancellationToken).ConfigureAwait(false);

                    int length = diagnostics.Length;

                    if (length == 0)
                        break;

                    if (length == previousDiagnostics.Length
                        && !diagnostics.Except(previousDiagnostics, DiagnosticDeepEqualityComparer.Instance).Any())
                    {
                        Assert.True(false, "Same diagnostics returned before and after the fix was applied.");
                    }

                    Diagnostic diagnostic = null;
                    foreach (Diagnostic d in diagnostics)
                    {
                        if (d.Id == Descriptor.Id)
                        {
                            diagnostic = d;
                            break;
                        }
                    }

                    if (diagnostic == null)
                        break;

                    CodeAction action = null;

                    var context = new CodeFixContext(
                        document,
                        diagnostic,
                        (a, d) =>
                        {
                            if (action == null
                                && (equivalenceKey == null || string.Equals(a.EquivalenceKey, equivalenceKey, StringComparison.Ordinal))
                                && d.Contains(diagnostic))
                            {
                                action = a;
                            }
                        },
                        CancellationToken.None);

                    await FixProvider.RegisterCodeFixesAsync(context).ConfigureAwait(false);

                    if (action == null)
                        break;

                    fixRegistered = true;

                    document = await document.ApplyCodeActionAsync(action).ConfigureAwait(false);

                    compilation = await document.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

                    ImmutableArray<Diagnostic> newCompilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

                    VerifyCompilerDiagnostics(newCompilerDiagnostics, options);

                    VerifyNoNewCompilerDiagnostics(compilerDiagnostics, newCompilerDiagnostics, options);

                    if (!Descriptor.IsEnabledByDefault)
                        compilation = compilation.EnsureEnabled(Descriptor);

                    previousDiagnostics = diagnostics;
                }

                Assert.True(fixRegistered, "No code fix has been registered.");

                string actual = await document.ToFullStringAsync(simplify: true, format: true, cancellationToken).ConfigureAwait(false);

                Assert.Equal(expected, actual);

                if (expectedDocuments.Any())
                    await VerifyAdditionalDocumentsAsync(document.Project, expectedDocuments, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task VerifyNoFixAsync(
            string source,
            IEnumerable<string> additionalSources = null,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            options ??= Options;

            using (Workspace workspace = new AdhocWorkspace())
            {
                Project project = WorkspaceFactory.AddProject(workspace.CurrentSolution, options);

                Document document = WorkspaceFactory.AddDocument(project, source, additionalSources);

                Compilation compilation = await document.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

                ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

                VerifyCompilerDiagnostics(compilerDiagnostics, options);

                if (!Descriptor.IsEnabledByDefault)
                    compilation = compilation.EnsureEnabled(Descriptor);

                ImmutableArray<Diagnostic> diagnostics = await compilation.GetAnalyzerDiagnosticsAsync(Analyzer, DiagnosticComparer.SpanStart, cancellationToken).ConfigureAwait(false);

                foreach (Diagnostic diagnostic in diagnostics)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!string.Equals(diagnostic.Id, Descriptor.Id, StringComparison.Ordinal))
                        continue;

                    if (!FixableDiagnosticIds.Contains(diagnostic.Id))
                        continue;

                    var context = new CodeFixContext(
                        document,
                        diagnostic,
                        (a, d) =>
                        {
                            if ((equivalenceKey == null || string.Equals(a.EquivalenceKey, equivalenceKey, StringComparison.Ordinal))
                                && d.Contains(diagnostic))
                            {
                                Assert.True(false, "No code fix expected.");
                            }
                        },
                        CancellationToken.None);

                    await FixProvider.RegisterCodeFixesAsync(context).ConfigureAwait(false);
                }
            }
        }
    }
}
