// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Roslynator.Test
{
    public static class DiagnosticVerifier
    {
        public static void VerifyNoDiagnostic(
            string source,
            DiagnosticAnalyzer analyzer,
            DiagnosticDescriptor descriptor,
            string language)
        {
            VerifyNoDiagnostic(new string[] { source }, analyzer, descriptor, language);
        }

        public static void VerifyNoDiagnostic(
            IEnumerable<string> sources,
            DiagnosticAnalyzer analyzer,
            DiagnosticDescriptor descriptor,
            string language)
        {
            Assert.True(analyzer.SupportedDiagnostics.IndexOf(descriptor, DiagnosticDescriptorComparer.IdOrdinal) != -1,
                $"Diagnostic \"{descriptor.Id}\" is not supported by analyzer \"{analyzer.GetType().Name}\"");

            Diagnostic[] diagnostics = TestUtility.GetSortedDiagnostics(analyzer, sources, language);

            if (diagnostics.Length > 0
                && analyzer.SupportedDiagnostics.Length > 1
                && diagnostics.Any(f => !string.Equals(f.Id, descriptor.Id, StringComparison.Ordinal)))
            {
                diagnostics = diagnostics.Where(f => string.Equals(f.Id, descriptor.Id, StringComparison.Ordinal)).ToArray();

                Assert.True(false,
                    $"Mismatch between number of diagnostics returned, expected: {0} actual: {diagnostics.Length}\r\n\r\nDiagnostics:\r\n{string.Join<Diagnostic>("\r\n", diagnostics)}\r\n");
            }
        }

        public static void VerifyDiagnostic(
            string source,
            DiagnosticAnalyzer analyzer,
            string language,
            params Diagnostic[] expectedDiagnostics)
        {
            VerifyDiagnostic(new string[] { source }, analyzer, language, expectedDiagnostics);
        }

        public static void VerifyDiagnostic(
            IEnumerable<string> sources,
            DiagnosticAnalyzer analyzer,
            string language,
            params Diagnostic[] expectedDiagnostics)
        {
            ImmutableArray<DiagnosticDescriptor> supportedDiagnostics = analyzer.SupportedDiagnostics;

            VerifySupportedDiagnostics(analyzer, expectedDiagnostics, supportedDiagnostics);

            Diagnostic[] diagnostics = TestUtility.GetSortedDiagnostics(analyzer, sources, language);

            if (diagnostics.Length > 0
                && analyzer.SupportedDiagnostics.Length > 1)
            {
                RemoveOtherDiagnostics();
            }

            VerifyDiagnostics(diagnostics, expectedDiagnostics);

            void RemoveOtherDiagnostics()
            {
                foreach (Diagnostic diagnostic in diagnostics)
                {
                    foreach (Diagnostic expectedDiagnostic in expectedDiagnostics)
                    {
                        if (!DiagnosticComparer.IdOrdinal.Equals(diagnostic, expectedDiagnostic))
                        {
                            diagnostics = diagnostics
                                .Where(d => !expectedDiagnostics.Any(ed => DiagnosticComparer.IdOrdinal.Equals(d, ed)))
                                .ToArray();

                            return;
                        }
                    }
                }
            }
        }

        private static void VerifySupportedDiagnostics(DiagnosticAnalyzer analyzer, Diagnostic[] expectedDiagnostics, ImmutableArray<DiagnosticDescriptor> supportedDiagnostics)
        {
            foreach (Diagnostic diagnostic in expectedDiagnostics)
            {
                Assert.True(supportedDiagnostics.IndexOf(diagnostic.Descriptor, DiagnosticDescriptorComparer.IdOrdinal) != -1,
                    $"Diagnostic \"{diagnostic.Descriptor.Id}\" is not supported by analyzer \"{analyzer.GetType().Name}\"");
            }
        }

        private static void VerifyDiagnostics(
            Diagnostic[] actual,
            params Diagnostic[] expected)
        {
            int expectedCount = expected.Length;
            int actualCount = actual.Length;

            Assert.True(expectedCount == actualCount,
                $"Mismatch between number of diagnostics returned, expected: {expectedCount} actual: {actualCount}\r\n\r\nDiagnostics:\r\n{string.Join<Diagnostic>("\r\n", actual)}\r\n");

            for (int i = 0; i < expectedCount; i++)
                VerifyDiagnostic(actual[i], expected[i]);
        }

        private static void VerifyDiagnostic(Diagnostic actual, Diagnostic expected)
        {
            Assert.True(actual.Id == expected.Descriptor.Id,
                $"Expected diagnostic id to be \"{expected.Descriptor.Id}\" was \"{actual.Id}\"\r\n\r\nDiagnostic:\r\n{actual}\r\n");

            VerifyLocation(actual, actual.Location, expected.Location);

            VerifyAdditionalLocations(actual, actual.AdditionalLocations, expected.AdditionalLocations);
        }

        private static void VerifyAdditionalLocations(
            Diagnostic diagnostic,
            IReadOnlyList<Location> actual,
            IReadOnlyList<Location> expected)
        {
            int actualCount = actual.Count;
            int expectedCount = expected.Count;

            Assert.True(actualCount == expectedCount,
                $"Expected {expectedCount} additional locations but got {actualCount} for Diagnostic:\r\n{diagnostic}\r\n");

            for (int j = 0; j < actualCount; j++)
                VerifyLocation(diagnostic, actual[j], expected[j]);
        }

        private static void VerifyLocation(
            Diagnostic diagnostic,
            Location actual,
            Location expected)
        {
            VerifySpan(diagnostic, actual.GetLineSpan(), expected.GetLineSpan());
        }

        private static void VerifySpan(
            Diagnostic diagnostic,
            FileLinePositionSpan actual,
            FileLinePositionSpan expected)
        {
            Assert.True(actual.Path == expected.Path,
                $"Expected diagnostic to be in file \"{expected.Path}\" was actually in file \"{actual.Path}\"\r\n\r\nDiagnostic:\r\n{diagnostic}\r\n");

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
                $"Expected diagnostic to {name} on line \"{expectedLine}\" actually {name}s on line \"{actualLine}\"\r\n\r\nDiagnostic:\r\n{diagnostic}\r\n");

            int actualCharacter = actual.Character;
            int expectedCharacter = expected.Character;

            Assert.True(actualCharacter == expectedCharacter,
                $"Expected diagnostic to {name} at column \"{expectedCharacter}\" actually {name}s at column \"{actualCharacter}\"\r\n\r\nDiagnostic:\r\n{diagnostic}\r\n");
        }
    }
}
