// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Roslynator.Test
{
    public static class DiagnosticVerifier
    {
        public static void VerifyCSharpDiagnostic(
            string source,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer)
        {
            (string newSource, List<LinePositionSpan> spans) = TestUtility.GetTextAndSpans(source);

            DiagnosticResult[] expectedResults = spans
                .Select(f => new DiagnosticResult(descriptor, new FileLinePositionSpan(TestUtility.CreateFileName(), f)))
                .ToArray();

            VerifyCSharpDiagnostic(newSource, analyzer, expectedResults);
        }

        public static void VerifyCSharpNoDiagnostic(
            string text,
            DiagnosticAnalyzer analyzer)
        {
            VerifyCSharpDiagnostic(text, analyzer);
        }

        public static void VerifyCSharpDiagnosticWithFix(
            string source,
            string sourceWithFix,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider codeFixProvider,
            int? codeFixIndex = null,
            bool allowNewCompilerDiagnostics = false)
        {
            (string newSource, List<LinePositionSpan> spans) = TestUtility.GetTextAndSpans(source);

            DiagnosticResult[] expectedResults = spans
                .Select(f => new DiagnosticResult(descriptor, new FileLinePositionSpan(TestUtility.CreateFileName(), f)))
                .ToArray();

            VerifyCSharpDiagnostic(newSource, analyzer, expectedResults);

            CodeFixVerifier.VerifyFix(LanguageNames.CSharp, analyzer, codeFixProvider, newSource, sourceWithFix, codeFixIndex, allowNewCompilerDiagnostics);
        }

        public static void VerifyCSharpDiagnostic(
            string source,
            DiagnosticAnalyzer analyzer,
            params DiagnosticResult[] expectedResults)
        {
            VerifyDiagnostics(new string[] { source }, LanguageNames.CSharp, analyzer, expectedResults);
        }

        public static void VerifyCSharpDiagnostic(
            IEnumerable<string> sources,
            DiagnosticAnalyzer analyzer,
            params DiagnosticResult[] expectedResults)
        {
            VerifyDiagnostics(sources, LanguageNames.CSharp, analyzer, expectedResults);
        }

        public static void VerifyDiagnostic(
            string source,
            string language,
            DiagnosticAnalyzer analyzer,
            params DiagnosticResult[] expectedResults)
        {
            VerifyDiagnostics(new string[] { source }, language, analyzer, expectedResults);
        }

        private static void VerifyDiagnostics(
            IEnumerable<string> sources,
            string language,
            DiagnosticAnalyzer analyzer,
            params DiagnosticResult[] expectedResults)
        {
            ImmutableArray<DiagnosticDescriptor> supportedDiagnostics = analyzer.SupportedDiagnostics;

            foreach (DiagnosticResult result in expectedResults)
            {
                Assert.True(supportedDiagnostics.Contains(result.Descriptor),
                    $"Diagnostic \"{result.Descriptor.Id}\" is not supported by analyzer \"{analyzer.GetType().Name}\"");
            }

            Diagnostic[] diagnostics = TestUtility.GetSortedDiagnostics(analyzer, sources, language);

            VerifyDiagnosticResults(diagnostics, expectedResults);
        }

        private static void VerifyDiagnosticResults(
            Diagnostic[] actualResults,
            params DiagnosticResult[] expectedResults)
        {
            int expectedCount = expectedResults.Length;
            int actualCount = actualResults.Length;

            if (expectedCount != actualCount)
            {
                Assert.True(false,
                    $"Mismatch between number of diagnostics returned, expected: {expectedCount} actual: {actualCount}\r\n\r\nDiagnostics:\r\n{string.Join<Diagnostic>("\r\n", actualResults)}\r\n");
            }

            for (int i = 0; i < expectedCount; i++)
            {
                Diagnostic actual = actualResults[i];
                DiagnosticResult expected = expectedResults[i];

                if (expected.Spans.IsDefaultOrEmpty)
                {
                    if (actual.Location != Location.None)
                    {
                        Assert.True(false,
                            $"Expected:\nA project diagnostic with no location\nActual:\n{actual}");
                    }
                }
                else
                {
                    VerifyDiagnosticSpan(actual, actual.Location.GetLineSpan(), expected.Spans[0]);

                    IReadOnlyList<Location> additionalLocations = actual.AdditionalLocations;

                    if (additionalLocations.Count != expected.Spans.Length - 1)
                    {
                        Assert.True(false,
                            $"Expected {expected.Spans.Length - 1} additional locations but got {additionalLocations.Count} for Diagnostic:\r\n{actual}\r\n");
                    }

                    for (int j = 0; j < additionalLocations.Count; j++)
                    {
                        VerifyDiagnosticSpan(actual, additionalLocations[j].GetLineSpan(), expected.Spans[j + 1]);
                    }
                }

                if (actual.Id != expected.Descriptor.Id)
                {
                    Assert.True(false,
                        $"Expected diagnostic id to be \"{expected.Descriptor.Id}\" was \"{actual.Id}\"\r\n\r\nDiagnostic:\r\n{actual}\r\n");
                }
            }
        }

        private static void VerifyDiagnosticSpan(
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

            if (actualLine > 0
                && actualLine != expectedLine)
            {
                Assert.True(false,
                    $"Expected diagnostic to {name} on line \"{expectedLine}\" actually {name}s on line \"{actualLine}\"\r\n\r\nDiagnostic:\r\n{diagnostic}\r\n");
            }

            int actualCharacter = actual.Character;
            int expectedCharacter = expected.Character;

            if (actualCharacter > 0
                && actualCharacter != expectedCharacter)
            {
                Assert.True(false,
                    $"Expected diagnostic to {name} at column \"{expectedCharacter}\" actually {name}s at column \"{actualCharacter}\"\r\n\r\nDiagnostic:\r\n{diagnostic}\r\n");
            }
        }
    }
}
