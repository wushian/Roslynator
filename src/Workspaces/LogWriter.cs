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
    internal class LogWriter : TextWriter
    {
        public LogWriter(TextWriter writer)
        {
            Writer = writer;
        }

        public LogWriter(TextWriter writer, IFormatProvider formatProvider) : base(formatProvider)
        {
            Writer = writer;
        }

        public Verbosity Verbosity { get; set; } = Verbosity.Detailed;

        public override Encoding Encoding => Writer.Encoding;

        protected TextWriter Writer { get; }

        public override void Write(bool value)
        {
            Writer.Write(value);
        }

        public override void Write(char value)
        {
            Writer.Write(value);
        }

        public override void Write(char[] buffer)
        {
            Writer.Write(buffer);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            Writer.Write(buffer, index, count);
        }

        public override void Write(decimal value)
        {
            Writer.Write(value);
        }

        public override void Write(double value)
        {
            Writer.Write(value);
        }

        public override void Write(int value)
        {
            Writer.Write(value);
        }

        public override void Write(long value)
        {
            Writer.Write(value);
        }

        public override void Write(object value)
        {
            Writer.Write(value);
        }

        public override void Write(float value)
        {
            Writer.Write(value);
        }

        public override void Write(string value)
        {
            Writer.Write(value);
        }

        public void Write(string value, Verbosity verbosity)
        {
            WriteIf(verbosity <= Verbosity, value);
        }

        public void WriteIf(bool condition, string value)
        {
            if (condition)
                Write(value);
        }

        public override void Write(string format, object arg0)
        {
            Writer.Write(format, arg0);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            Writer.Write(format, arg0, arg1);
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            Writer.Write(format, arg0, arg1, arg2);
        }

        public override void Write(string format, params object[] arg)
        {
            Writer.Write(format, arg);
        }

        public override void Write(uint value)
        {
            Writer.Write(value);
        }

        public override void Write(ulong value)
        {
            Writer.Write(value);
        }

        public override void WriteLine()
        {
            Writer.WriteLine();
        }

        public void WriteLine(Verbosity verbosity)
        {
            WriteLineIf(verbosity <= Verbosity);
        }

        public void WriteLineIf(bool condition)
        {
            if (condition)
                WriteLine();
        }

        public override void WriteLine(bool value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(char value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(char[] buffer)
        {
            Writer.WriteLine(buffer);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            Writer.WriteLine(buffer, index, count);
        }

        public override void WriteLine(decimal value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(double value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(int value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(long value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(object value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(float value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(string value)
        {
            Writer.WriteLine(value);
        }

        public void WriteLine(string value, Verbosity verbosity)
        {
            WriteLineIf(verbosity <= Verbosity, value);
        }

        public void WriteLineIf(bool condition, string value)
        {
            if (condition)
                WriteLine(value);
        }

        public override void WriteLine(string format, object arg0)
        {
            Writer.WriteLine(format, arg0);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            Writer.WriteLine(format, arg0, arg1);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            Writer.WriteLine(format, arg0, arg1, arg2);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            Writer.WriteLine(format, arg);
        }

        public override void WriteLine(uint value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(ulong value)
        {
            Writer.WriteLine(value);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Writer.Dispose();

            base.Dispose(disposing);
        }

        public void WriteDiagnostic(
            Diagnostic diagnostic,
            string indentation = null,
            Verbosity verbosity = Verbosity.None)
        {
            if (verbosity > Verbosity)
                return;

            Write(indentation);
            WriteLine(diagnostic.ToString(), diagnostic.Severity.GetColor());
        }

        public void WriteDiagnostics(
            IEnumerable<Diagnostic> diagnostics,
            int maxCount = int.MaxValue,
            string indentation = null,
            Verbosity verbosity = Verbosity.None)
        {
            if (verbosity > Verbosity)
                return;

            using (IEnumerator<Diagnostic> en = diagnostics
                .OrderBy(f => f.Id)
                .ThenBy(f => f.Location.SourceTree.FilePath)
                .ThenBy(f => f.Location.SourceSpan.Start)
                .GetEnumerator())
            {
                int count = 0;

                while (en.MoveNext())
                {
                    count++;

                    if (count <= maxCount)
                    {
                        WriteDiagnostic(en.Current, indentation);
                    }
                    else
                    {
                        count = 0;

                        while (en.MoveNext())
                            count++;

                        Write(indentation);
                        WriteLine($"and {count} more diagnostics");
                    }
                }
            }
        }

        public void WriteDiagnostics(
            ImmutableArray<Diagnostic> diagnostics,
            string baseDirectoryPath = null,
            IFormatProvider formatProvider = null,
            Verbosity verbosity = Verbosity.None)
        {
            if (verbosity > Verbosity)
                return;

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
                WriteLine($"  {FormatDiagnostic(diagnostic)}", diagnostic.Severity.GetColor());
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
        }
    }
}
