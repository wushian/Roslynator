// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Text;

namespace Roslynator
{
    internal static class DiagnosticFormatter
    {
        public static IEnumerable<(Diagnostic diagnostic, string message)> FormatDiagnostics(
            ImmutableArray<Diagnostic> diagnostics,
            string baseDirectoryPath = null,
            IFormatProvider formatProvider = null,
            DiagnosticDisplayParts parts = DiagnosticDisplayParts.All)
        {
            if (!diagnostics.Any())
                yield break;

            int maxIdLength = diagnostics.Max(f => f.Id.Length);
            int maxSeverityLength = diagnostics.Max(f => GetSeverityText(f.Severity).Length);
            int maxMessageLength = diagnostics.Max(f => f.GetMessage(formatProvider).Length);

            foreach (Diagnostic diagnostic in diagnostics
                .OrderBy(f => f.Id)
                .ThenBy(f => f.Location.SourceTree.FilePath)
                .ThenBy(f => f.Location.SourceSpan.Start))
            {
                string message = FormatDiagnostic(diagnostic, baseDirectoryPath, formatProvider, parts, maxIdLength, maxSeverityLength, maxMessageLength);

                yield return (diagnostic, message);
            }
        }

        public static string FormatDiagnostic(
            Diagnostic diagnostic,
            string baseDirectoryPath = null,
            IFormatProvider formatProvider = null,
            DiagnosticDisplayParts parts = DiagnosticDisplayParts.All)
        {
            return FormatDiagnostic(diagnostic, baseDirectoryPath, formatProvider, parts, 0, 0, 0);
        }

        private static string FormatDiagnostic(
            Diagnostic diagnostic,
            string baseDirectoryPath,
            IFormatProvider formatProvider,
            DiagnosticDisplayParts parts,
            int maxIdLength,
            int maxSeverityLength,
            int maxMessageLength)
        {
            StringBuilder sb = StringBuilderCache.GetInstance();

            string severity = GetSeverityText(diagnostic.Severity);

            if ((parts & DiagnosticDisplayParts.Severity) != 0)
            {
                sb.Append(severity);
                sb.Append(' ', maxSeverityLength - severity.Length + 1);
            }

            if ((parts & DiagnosticDisplayParts.Id) != 0)
            {
                sb.Append(diagnostic.Id);
                sb.Append(' ', maxIdLength - diagnostic.Id.Length + 1);
            }

            string message = diagnostic.GetMessage(formatProvider);

            if ((parts & DiagnosticDisplayParts.Message) != 0)
            {
                sb.Append(message);
                sb.Append(' ', maxMessageLength - message.Length);
            }

            switch (diagnostic.Location.Kind)
            {
                case LocationKind.SourceFile:
                case LocationKind.XmlFile:
                case LocationKind.ExternalFile:
                    {
                        FileLinePositionSpan span = diagnostic.Location.GetMappedLineSpan();

                        if (span.IsValid)
                        {
                            if ((parts & DiagnosticDisplayParts.Path) != 0)
                            {
                                sb.Append(' ');
                                sb.Append(PathUtilities.MakeRelativePath(span.Path, baseDirectoryPath));
                            }

                            if ((parts & DiagnosticDisplayParts.Location) != 0)
                            {
                                LinePosition linePosition = span.Span.Start;

                                sb.Append('(');
                                sb.Append(linePosition.Line + 1);
                                sb.Append(',');
                                sb.Append(linePosition.Character + 1);
                                sb.Append(')');
                            }
                        }

                        break;
                    }
            }

            return StringBuilderCache.GetStringAndFree(sb);
        }

        private static string GetSeverityText(DiagnosticSeverity diagnosticSeverity)
        {
            switch (diagnosticSeverity)
            {
                case DiagnosticSeverity.Hidden:
                    return "hidden";
                case DiagnosticSeverity.Info:
                    return "info";
                case DiagnosticSeverity.Warning:
                    return "warning";
                case DiagnosticSeverity.Error:
                    return "error";
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
