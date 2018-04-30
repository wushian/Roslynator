// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Roslynator.Tests
{
    public abstract class DiagnosticVerifier : CodeVerifier
    {
        public abstract DiagnosticDescriptor Descriptor { get; }

        public abstract DiagnosticAnalyzer Analyzer { get; }

        public async Task VerifyDiagnosticAsync(
            string source,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ParseResult result = TextUtility.GetSpans(source);

            IEnumerable<Diagnostic> diagnostics = result.Spans.Select(f => CreateDiagnostic(f.Span, f.LineSpan));

            await VerifyDiagnosticAsync(result.Source, diagnostics, cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyDiagnosticAsync(
            string theory,
            string diagnosticData,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            (string source, TextSpan span) = TextUtility.GetMarkedSpan(theory, diagnosticData);

            ParseResult result = TextUtility.GetSpans(source);

            if (result.Spans.Any())
            {
                await VerifyDiagnosticAsync(result.Source, result.Spans.Select(f => f.Span).ToList(), cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await VerifyDiagnosticAsync(source, span, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task VerifyDiagnosticAsync(
            string source,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Diagnostic diagnostic = CreateDiagnostic(source, span);

            await VerifyDiagnosticAsync(source, diagnostic, cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyDiagnosticAsync(
            string source,
            IList<TextSpan> spans,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Diagnostic[] diagnostics = spans.Select(span => CreateDiagnostic(source, span)).ToArray();

            await VerifyDiagnosticAsync(source, diagnostics, cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyDiagnosticAsync(
            string source,
            Diagnostic diagnostic,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyDiagnosticAsync(new string[] { source }, new Diagnostic[] { diagnostic }, cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyDiagnosticAsync(
            string source,
            IEnumerable<Diagnostic> expectedDiagnostics,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyDiagnosticAsync(
                new string[] { source },
                expectedDiagnostics,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyDiagnosticAsync(
            IEnumerable<string> sources,
            IEnumerable<Diagnostic> expectedDiagnostics,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (Diagnostic diagnostic in expectedDiagnostics)
            {
                Assert.True(Analyzer.Supports(diagnostic.Descriptor),
                    $"Diagnostic \"{diagnostic.Descriptor.Id}\" is not supported by analyzer \"{Analyzer.GetType().Name}\".");
            }

            Project project = WorkspaceFactory.Project(sources, Language);

            Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

            VerifyDiagnostics(compilerDiagnostics, Options.MaxAllowedCompilerDiagnosticSeverity);

            if (Options.EnableDiagnosticsDisabledByDefault)
                compilation = compilation.EnableDiagnosticsDisabledByDefault(Analyzer);

            ImmutableArray<Diagnostic> analyzerDiagnostics = await compilation.GetAnalyzerDiagnosticsAsync(Analyzer, DiagnosticComparer.SpanStart, cancellationToken).ConfigureAwait(false);

            IEnumerable<Diagnostic> actualDiagnostics = analyzerDiagnostics;

            if (analyzerDiagnostics.Length > 0
                && Analyzer.SupportedDiagnostics.Length > 1)
            {
                actualDiagnostics = analyzerDiagnostics.Where(diagnostic => expectedDiagnostics.Any(expectedDiagnostic => DiagnosticComparer.Id.Equals(diagnostic, expectedDiagnostic)));
            }

            VerifyDiagnostics(actualDiagnostics, expectedDiagnostics);
        }

        public async Task VerifyNoDiagnosticAsync(
            string theory,
            string diagnosticData,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            (string source, TextSpan span) = TextUtility.GetMarkedSpan(theory, diagnosticData);

            await VerifyNoDiagnosticAsync(source, cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyNoDiagnosticAsync(
            string source,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyNoDiagnosticAsync(
                new string[] { source },
                cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyNoDiagnosticAsync(
            IEnumerable<string> sources,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Assert.True(Analyzer.Supports(Descriptor),
                $"Diagnostic \"{Descriptor.Id}\" is not supported by analyzer \"{Analyzer.GetType().Name}\".");

            Project project = WorkspaceFactory.Project(sources, Language);

            Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

            VerifyDiagnostics(compilerDiagnostics, Options.MaxAllowedCompilerDiagnosticSeverity);

            if (Options.EnableDiagnosticsDisabledByDefault)
                compilation = compilation.EnableDiagnosticsDisabledByDefault(Analyzer);

            ImmutableArray<Diagnostic> analyzerDiagnostics = await compilation.GetAnalyzerDiagnosticsAsync(Analyzer, DiagnosticComparer.SpanStart, cancellationToken).ConfigureAwait(false);

            Assert.True(analyzerDiagnostics.Length == 0 || analyzerDiagnostics.All(f => !string.Equals(f.Id, Descriptor.Id, StringComparison.Ordinal)),
                    $"No diagnostic expected{analyzerDiagnostics.Where(f => string.Equals(f.Id, Descriptor.Id, StringComparison.Ordinal)).ToDebugString()}");
        }

        private protected Diagnostic CreateDiagnostic(string source, TextSpan span)
        {
            LinePositionSpan lineSpan = span.ToLinePositionSpan(source);

            return CreateDiagnostic(span, lineSpan);
        }

        private protected Diagnostic CreateDiagnostic(TextSpan span, LinePositionSpan lineSpan)
        {
            Location location = Location.Create(FileUtility.DefaultCSharpFileName, span, lineSpan);

            return Diagnostic.Create(Descriptor, location);
        }

        internal static void VerifyNoNewCompilerDiagnostics(
            ImmutableArray<Diagnostic> compilerDiagnostics,
            ImmutableArray<Diagnostic> newCompilerDiagnostics)
        {
            IEnumerable<Diagnostic> diff = newCompilerDiagnostics
                .Except(compilerDiagnostics, DiagnosticDeepEqualityComparer.Instance)
                .ToImmutableArray();

            if (diff.Any())
            {
                Assert.True(false,
                    $"Code fix introduced new compiler diagnostics{diff.ToDebugString()}");
            }
        }

        internal static void VerifyDiagnostics(ImmutableArray<Diagnostic> diagnostics, DiagnosticSeverity maxAllowedSeverity = DiagnosticSeverity.Info)
        {
            Assert.False(diagnostics.Any(f => f.Severity > maxAllowedSeverity),
                $"No compiler diagnostics with severity higher than '{maxAllowedSeverity}' expected{diagnostics.Where(f => f.Severity > maxAllowedSeverity).ToDebugString()}");
        }

        internal void VerifyDiagnostics(
            IEnumerable<Diagnostic> actual,
            IEnumerable<Diagnostic> expected,
            bool checkAdditionalLocations = false)
        {
            int expectedCount = 0;
            int actualCount = 0;

            using (IEnumerator<Diagnostic> expectedEnumerator = expected.GetEnumerator())
            using (IEnumerator<Diagnostic> actualEnumerator = actual.GetEnumerator())
            {
                while (expectedEnumerator.MoveNext())
                {
                    expectedCount++;

                    Diagnostic expectedDiagnostic = expectedEnumerator.Current;

                    if (!Analyzer.Supports(expectedDiagnostic.Descriptor))
                        Assert.True(false,$"Diagnostic \"{expectedDiagnostic.Id}\" is not supported by analyzer \"{Analyzer.GetType().Name}\".");

                    if (actualEnumerator.MoveNext())
                    {
                        actualCount++;

                        VerifyDiagnostic(actualEnumerator.Current, expectedDiagnostic, checkAdditionalLocations: checkAdditionalLocations);
                    }
                    else
                    {
                        while (expectedEnumerator.MoveNext())
                            expectedCount++;

                        Assert.True(false, $"Mismatch between number of diagnostics returned, expected: {expectedCount} actual: {actualCount}{actual.ToDebugString()}");
                    }
                }

                if (actualEnumerator.MoveNext())
                {
                    actualCount++;

                    while (actualEnumerator.MoveNext())
                        actualCount++;

                    Assert.True(false, $"Mismatch between number of diagnostics returned, expected: {expectedCount} actual: {actualCount}{actual.ToDebugString()}");
                }
            }
        }

        private static void VerifyDiagnostic(
            Diagnostic actualDiagnostic,
            Diagnostic expectedDiagnostic,
            bool checkAdditionalLocations = false)
        {
            if (actualDiagnostic.Id != expectedDiagnostic.Descriptor.Id)
                Assert.True(false, $"Expected diagnostic id to be \"{expectedDiagnostic.Descriptor.Id}\" was \"{actualDiagnostic.Id}\"{GetMessage()}");

            VerifyLocation(actualDiagnostic.Location, expectedDiagnostic.Location);

            if (checkAdditionalLocations)
                VerifyAdditionalLocations(actualDiagnostic.AdditionalLocations, expectedDiagnostic.AdditionalLocations);

            void VerifyLocation(
                Location actualLocation,
                Location expectedLocation)
            {
                VerifyFileLinePositionSpan(actualLocation.GetLineSpan(), expectedLocation.GetLineSpan());
            }

            void VerifyAdditionalLocations(
                IReadOnlyList<Location> actual,
                IReadOnlyList<Location> expected)
            {
                int actualCount = actual.Count;
                int expectedCount = expected.Count;

                if (actualCount != expectedCount)
                    Assert.True(false, $"Expected {expectedCount} additional location(s), actual: {actualCount}{GetMessage()}");

                for (int j = 0; j < actualCount; j++)
                    VerifyLocation(actual[j], expected[j]);
            }

            void VerifyFileLinePositionSpan(
                FileLinePositionSpan actual,
                FileLinePositionSpan expected)
            {
                if (actual.Path != expected.Path)
                    Assert.True(false, $"Expected diagnostic to be in file \"{expected.Path}\", actual: \"{actual.Path}\"{GetMessage()}");

                VerifyLinePosition(actual.StartLinePosition, expected.StartLinePosition, "start");

                VerifyLinePosition(actual.EndLinePosition, expected.EndLinePosition, "end");
            }

            void VerifyLinePosition(
                LinePosition actual,
                LinePosition expected,
                string startOrEnd)
            {
                int actualLine = actual.Line;
                int expectedLine = expected.Line;

                if (actualLine != expectedLine)
                    Assert.True(false, $"Expected diagnostic to {startOrEnd} on line {expectedLine}, actual: {actualLine}{GetMessage()}");

                int actualCharacter = actual.Character;
                int expectedCharacter = expected.Character;

                if (actualCharacter != expectedCharacter)
                    Assert.True(false, $"Expected diagnostic to {startOrEnd} at column {expectedCharacter}, actual: {actualCharacter}{GetMessage()}");
            }

            string GetMessage()
            {
                return $"\r\n\r\nExpected diagnostic:\r\n{expectedDiagnostic}\r\n\r\nActual diagnostic:\r\n{actualDiagnostic}\r\n";
            }
        }
    }
}
