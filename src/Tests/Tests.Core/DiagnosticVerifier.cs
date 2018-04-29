// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Roslynator.Tests
{
    public static class DiagnosticVerifier
    {
        public static async Task VerifyDiagnosticAsync(
            string source,
            DiagnosticAnalyzer analyzer,
            string language,
            Diagnostic[] expectedDiagnostics,
            CodeVerificationSettings settings = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            await VerifyDiagnosticAsync(new string[] { source }, analyzer, language, expectedDiagnostics, settings, cancellationToken).ConfigureAwait(false);
        }

        public static async Task VerifyDiagnosticAsync(
            IEnumerable<string> sources,
            DiagnosticAnalyzer analyzer,
            string language,
            Diagnostic[] expectedDiagnostics,
            CodeVerificationSettings settings = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (expectedDiagnostics == null)
                throw new ArgumentNullException(nameof(expectedDiagnostics));

            if (settings == null)
                settings = CodeVerificationSettings.Default;

            foreach (Diagnostic diagnostic in expectedDiagnostics)
            {
                Assert.True(analyzer.Supports(diagnostic.Descriptor),
                    $"Diagnostic \"{diagnostic.Descriptor.Id}\" is not supported by analyzer \"{analyzer.GetType().Name}\".");
            }

            Project project = WorkspaceFactory.Project(sources, language);

            Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

            VerifyDiagnostics(compilerDiagnostics, settings.MaxAllowedCompilerDiagnosticSeverity);

            if (settings.EnableDiagnosticsDisabledByDefault)
                compilation = compilation.EnableDiagnosticsDisabledByDefault(analyzer);

            ImmutableArray<Diagnostic> diagnostics = await compilation.GetAnalyzerDiagnosticsAsync(analyzer, cancellationToken).ConfigureAwait(false);

            if (diagnostics.Length > 0
                && analyzer.SupportedDiagnostics.Length > 1)
            {
                diagnostics = diagnostics
                    .Where(diagnostic => expectedDiagnostics.Any(expectedDiagnostic => DiagnosticComparer.Id.Equals(diagnostic, expectedDiagnostic)))
                    .ToImmutableArray();
            }

            VerifyDiagnostics(diagnostics, expectedDiagnostics);
        }

        private static void VerifyDiagnostics(
            IList<Diagnostic> actual,
            IList<Diagnostic> expected)
        {
            int expectedCount = expected.Count;
            int actualCount = actual.Count;

            Assert.True(expectedCount == actualCount,
                $"Mismatch between number of diagnostics returned, expected: {expectedCount} actual: {actualCount}{actual.ToDebugString()}");

            for (int i = 0; i < expectedCount; i++)
                VerifyDiagnostic(actual[i], expected[i]);
        }

        private static void VerifyDiagnostic(Diagnostic actual, Diagnostic expected)
        {
            Assert.True(actual.Id == expected.Descriptor.Id,
                $"Expected diagnostic id to be \"{expected.Descriptor.Id}\" was \"{actual.Id}\"\r\n\r\nDiagnostic:\r\n{actual}\r\n");

            VerifyLocation(actual, actual.Location, expected.Location);

            //TODO: 
            //VerifyAdditionalLocations(actual, actual.AdditionalLocations, expected.AdditionalLocations);
        }

        private static void VerifyAdditionalLocations(
            Diagnostic diagnostic,
            IReadOnlyList<Location> actual,
            IReadOnlyList<Location> expected)
        {
            int actualCount = actual.Count;
            int expectedCount = expected.Count;

            Assert.True(actualCount == expectedCount,
                $"Expected {expectedCount} additional locations, actual: {actualCount}\r\n\r\nDiagnostic:\r\n{diagnostic}\r\n");

            for (int j = 0; j < actualCount; j++)
                VerifyLocation(diagnostic, actual[j], expected[j]);
        }

        private static void VerifyLocation(
            Diagnostic diagnostic,
            Location actual,
            Location expected)
        {
            VerifyFileLinePositionSpan(diagnostic, actual.GetLineSpan(), expected.GetLineSpan());
        }

        private static void VerifyFileLinePositionSpan(
            Diagnostic diagnostic,
            FileLinePositionSpan actual,
            FileLinePositionSpan expected)
        {
            Assert.True(actual.Path == expected.Path,
                $"Expected diagnostic to be in file \"{expected.Path}\", actual: \"{actual.Path}\"\r\n\r\nDiagnostic:\r\n{diagnostic}\r\n");

            VerifyLinePosition(diagnostic, actual.StartLinePosition, expected.StartLinePosition, "start");

            VerifyLinePosition(diagnostic, actual.EndLinePosition, expected.EndLinePosition, "end");
        }

        private static void VerifyLinePosition(
            Diagnostic diagnostic,
            LinePosition actual,
            LinePosition expected,
            string name)
        {
            int actualLine = actual.Line;
            int expectedLine = expected.Line;

            Assert.True(actualLine == expectedLine,
                $"Expected diagnostic to {name} on line {expectedLine}, actual: {actualLine}\r\n\r\nDiagnostic:\r\n{diagnostic}\r\n");

            int actualCharacter = actual.Character;
            int expectedCharacter = expected.Character;

            Assert.True(actualCharacter == expectedCharacter,
                $"Expected diagnostic to {name} at column {expectedCharacter}, actual: {actualCharacter}\r\n\r\nDiagnostic:\r\n{diagnostic}\r\n");
        }

        public static async Task VerifyNoDiagnosticAsync(
            string source,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer,
            string language)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            await VerifyNoDiagnosticAsync(new string[] { source }, descriptor, analyzer, language).ConfigureAwait(false);
        }

        public static async Task VerifyNoDiagnosticAsync(
            IEnumerable<string> sources,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer,
            string language,
            CodeVerificationSettings settings = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));

            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (analyzer == null)
                throw new ArgumentNullException(nameof(analyzer));

            if (language == null)
                throw new ArgumentNullException(nameof(language));

            if (settings == null)
                settings = CodeVerificationSettings.Default;

            Assert.True(analyzer.Supports(descriptor),
                $"Diagnostic \"{descriptor.Id}\" is not supported by analyzer \"{analyzer.GetType().Name}\".");

            Project project = WorkspaceFactory.Project(sources, language);

            Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

            VerifyDiagnostics(compilerDiagnostics, settings.MaxAllowedCompilerDiagnosticSeverity);

            if (settings.EnableDiagnosticsDisabledByDefault)
                compilation = compilation.EnableDiagnosticsDisabledByDefault(analyzer);

            ImmutableArray<Diagnostic> analyzerDiagnostics = await compilation.GetAnalyzerDiagnosticsAsync(analyzer, cancellationToken).ConfigureAwait(false);

            Assert.True(analyzerDiagnostics.Length == 0 || analyzerDiagnostics.All(f => !string.Equals(f.Id, descriptor.Id, StringComparison.Ordinal)),
                    $"No diagnostic expected{analyzerDiagnostics.Where(f => string.Equals(f.Id, descriptor.Id, StringComparison.Ordinal)).ToDebugString()}");
        }

        public static void VerifyDiagnostics(ImmutableArray<Diagnostic> diagnostics, DiagnosticSeverity maxAllowedSeverity = DiagnosticSeverity.Info)
        {
            Assert.False(diagnostics.Any(f => f.Severity > maxAllowedSeverity),
                $"No compiler error expected{diagnostics.Where(f => f.Severity > maxAllowedSeverity).ToDebugString()}");
        }

        public static async Task VerifyNoNewCompilerDiagnosticsAsync(
            Document document,
            ImmutableArray<Diagnostic> compilerDiagnostics,
            ImmutableArray<Diagnostic> compilerDiagnostics2,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<Diagnostic> newCompilerDiagnostics = GetNewDiagnostics(compilerDiagnostics, compilerDiagnostics2);

            if (!newCompilerDiagnostics.Any())
                return;

            SyntaxNode root = await document.GetSyntaxRootAsync().ConfigureAwait(false);

            document = document.WithSyntaxRoot(Formatter.Format(root, Formatter.Annotation, document.Project.Solution.Workspace, cancellationToken: cancellationToken));

            Assert.True(false,
                $"Code fix introduced new compiler diagnostics{newCompilerDiagnostics.ToDebugString()}");
        }

        private static IEnumerable<Diagnostic> GetNewDiagnostics(
            IEnumerable<Diagnostic> diagnostics,
            IEnumerable<Diagnostic> diagnostics2)
        {
            using (IEnumerator<Diagnostic> en2 = GetEnumerator(diagnostics2))
            using (IEnumerator<Diagnostic> en = GetEnumerator(diagnostics))
            {
                while (en2.MoveNext())
                {
                    if (en.MoveNext())
                    {
                        if (en.Current.Id != en2.Current.Id)
                            yield return en2.Current;
                    }
                    else
                    {
                        yield return en2.Current;

                        while (en2.MoveNext())
                            yield return en2.Current;

                        yield break;
                    }
                }
            }

            IEnumerator<Diagnostic> GetEnumerator(IEnumerable<Diagnostic> items)
            {
                return items
                    .Where(f => f.Severity != DiagnosticSeverity.Hidden)
                    .OrderBy(f => f, DiagnosticComparer.SpanStart)
                    .GetEnumerator();
            }
        }
    }
}
