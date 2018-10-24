// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Text;

namespace Roslynator
{
    internal static class ConsoleHelpers
    {
        public static TextWriter Log { get; set; }

        public static void Write(char value)
        {
            Console.Write(value);
            Log?.Write(value);
        }

        public static void Write(char[] buffer)
        {
            Console.Write(buffer);
            Log?.Write(buffer);
        }

        public static void Write(char[] buffer, int index, int count)
        {
            Console.Write(buffer, index, count);
            Log?.Write(buffer, index, count);
        }

        public static void Write(bool value)
        {
            Console.Write(value);
            Log?.Write(value);
        }

        public static void Write(int value)
        {
            Console.Write(value);
            Log?.Write(value);
        }

        public static void Write(uint value)
        {
            Console.Write(value);
            Log?.Write(value);
        }

        public static void Write(long value)
        {
            Console.Write(value);
            Log?.Write(value);
        }

        public static void Write(ulong value)
        {
            Console.Write(value);
            Log?.Write(value);
        }

        public static void Write(float value)
        {
            Console.Write(value);
            Log?.Write(value);
        }

        public static void Write(double value)
        {
            Console.Write(value);
            Log?.Write(value);
        }

        public static void Write(decimal value)
        {
            Console.Write(value);
            Log?.Write(value);
        }

        public static void Write(string value)
        {
            Console.Write(value);
            Log?.Write(value);
        }

        public static void Write(string message, ConsoleColor color)
        {
            ConsoleColor tmp = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Write(message);
            Console.ForegroundColor = tmp;
        }

        public static void Write(object value)
        {
            Console.Write(value);
            Log?.Write(value);
        }

        public static void Write(string format, object arg0)
        {
            Console.Write(format, arg0);
            Log?.Write(format, arg0);
        }

        public static void Write(string format, object arg0, object arg1)
        {
            Console.Write(format, arg0, arg1);
            Log?.Write(format, arg0, arg1);
        }

        public static void Write(string format, object arg0, object arg1, object arg2)
        {
            Console.Write(format, arg0, arg1, arg2);
            Log?.Write(format, arg0, arg1, arg2);
        }

        public static void Write(string format, params object[] arg)
        {
            Console.Write(format, arg);
            Log?.Write(format, arg);
        }

        public static void WriteLine()
        {
            Console.WriteLine();
            Log?.WriteLine();
        }

        public static void WriteLine(char value)
        {
            Console.WriteLine(value);
            Log?.WriteLine(value);
        }

        public static void WriteLine(char[] buffer)
        {
            Console.WriteLine(buffer);
            Log?.WriteLine(buffer);
        }

        public static void WriteLine(char[] buffer, int index, int count)
        {
            Console.WriteLine(buffer, index, count);
            Log?.WriteLine(buffer, index, count);
        }

        public static void WriteLine(bool value)
        {
            Console.WriteLine(value);
            Log?.WriteLine(value);
        }

        public static void WriteLine(int value)
        {
            Console.WriteLine(value);
            Log?.WriteLine(value);
        }

        public static void WriteLine(uint value)
        {
            Console.WriteLine(value);
            Log?.WriteLine(value);
        }

        public static void WriteLine(long value)
        {
            Console.WriteLine(value);
            Log?.WriteLine(value);
        }

        public static void WriteLine(ulong value)
        {
            Console.WriteLine(value);
            Log?.WriteLine(value);
        }

        public static void WriteLine(float value)
        {
            Console.WriteLine(value);
            Log?.WriteLine(value);
        }

        public static void WriteLine(double value)
        {
            Console.WriteLine(value);
            Log?.WriteLine(value);
        }

        public static void WriteLine(decimal value)
        {
            Console.WriteLine(value);
            Log?.WriteLine(value);
        }

        public static void WriteLine(string value)
        {
            Console.WriteLine(value);
            Log?.WriteLine(value);
        }

        public static void WriteLine(string message, ConsoleColor color)
        {
            ConsoleColor tmp = Console.ForegroundColor;
            Console.ForegroundColor = color;
            WriteLine(message);
            Console.ForegroundColor = tmp;
        }

        public static void WriteLine(object value)
        {
            Console.WriteLine(value);
            Log?.WriteLine(value);
        }

        public static void WriteLine(string format, object arg0)
        {
            Console.WriteLine(format, arg0);
            Log?.WriteLine(format, arg0);
        }

        public static void WriteLine(string format, object arg0, object arg1)
        {
            Console.WriteLine(format, arg0, arg1);
            Log?.WriteLine(format, arg0, arg1);
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            Console.WriteLine(format, arg0, arg1, arg2);
            Log?.WriteLine(format, arg0, arg1, arg2);
        }

        public static void WriteLine(string format, params object[] arg)
        {
            Console.WriteLine(format, arg);
            Log?.WriteLine(format, arg);
        }

        public static void WriteIf(bool condition, string message)
        {
            if (condition)
                Write(message);
        }

        public static void WriteIf(bool condition, string message, ConsoleColor color)
        {
            if (condition)
                Write(message, color);
        }

        public static void WriteLineIf(bool condition, string message)
        {
            if (condition)
                WriteLine(message);
        }

        public static void WriteLineIf(bool condition, string message, ConsoleColor color)
        {
            if (condition)
                WriteLine(message, color);
        }

        public static void WriteDiagnostics(IEnumerable<Diagnostic> diagnostics, int max, ConsoleColor color)
        {
            using (IEnumerator<Diagnostic> en = diagnostics.GetEnumerator())
            {
                int count = 0;

                while (en.MoveNext())
                {
                    count++;

                    if (count <= max)
                    {
                        WriteLine(en.Current.ToString(), color);
                    }
                    else
                    {
                        count = 0;

                        while (en.MoveNext())
                            count++;

                        WriteLine($"and {count} more diagnostics", color);
                    }
                }
            }
        }

        public static void WriteDiagnostics(
            ImmutableArray<Diagnostic> diagnostics,
            string baseDirectoryPath = null,
            IFormatProvider formatProvider = null)
        {
            if (!diagnostics.Any())
                return;

            int maxIdLength = diagnostics.Max(f => f.Id.Length);
            int maxSeverityLength = diagnostics.Max(f => GetSeverity(f.Severity).Length);
            int maxMessageLength = diagnostics.Max(f => f.GetMessage(formatProvider).Length);

            foreach (Diagnostic diagnostic in diagnostics
                .OrderBy(f => f.Id)
                .ThenBy(f => f.Location.SourceTree.FilePath)
                .ThenBy(f => f.Location.SourceSpan.Start))
            {
                WriteLine($"  {FormatDiagnostic(diagnostic)}", GetColor(diagnostic.Severity));
            }

            string FormatDiagnostic(Diagnostic diagnostic)
            {
                StringBuilder sb = StringBuilderCache.GetInstance();

                string severity = GetSeverity(diagnostic.Severity);

                sb.Append(severity);
                sb.Append(' ', maxSeverityLength - severity.Length + 1);

                sb.Append(diagnostic.Id);
                sb.Append(' ', maxIdLength - diagnostic.Id.Length + 1);

                string message = diagnostic.GetMessage(formatProvider);

                sb.Append(message);
                sb.Append(' ', maxMessageLength - message.Length);

                switch (diagnostic.Location.Kind)
                {
                    case LocationKind.SourceFile:
                    case LocationKind.XmlFile:
                    case LocationKind.ExternalFile:
                        {
                            FileLinePositionSpan span = diagnostic.Location.GetMappedLineSpan();

                            if (span.IsValid)
                            {
                                sb.Append(' ');
                                sb.Append(PathUtilities.MakeRelativePath(span.Path, baseDirectoryPath));

                                LinePosition linePosition = span.Span.Start;

                                sb.Append('(');
                                sb.Append(linePosition.Line + 1);
                                sb.Append(',');
                                sb.Append(linePosition.Character + 1);
                                sb.Append(')');
                            }

                            break;
                        }
                }

                return StringBuilderCache.GetStringAndFree(sb);
            }

            string GetSeverity(DiagnosticSeverity diagnosticSeverity)
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

            ConsoleColor GetColor(DiagnosticSeverity diagnosticSeverity)
            {
                switch (diagnosticSeverity)
                {
                    case DiagnosticSeverity.Hidden:
                        return ConsoleColor.DarkGray;
                    case DiagnosticSeverity.Info:
                        return ConsoleColor.Cyan;
                    case DiagnosticSeverity.Warning:
                        return ConsoleColor.Yellow;
                    case DiagnosticSeverity.Error:
                        return ConsoleColor.Red;
                    default:
                        throw new InvalidOperationException($"Unknown diagnostic severity '{diagnosticSeverity}'.");
                }
            }
        }
    }
}
