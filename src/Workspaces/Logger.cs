// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class Logger
    {
        public static ConsoleWriter ConsoleOut { get; } = ConsoleWriter.Instance;

        public static LogWriter LogOut { get; set; }

        public static void Write(char value)
        {
            ConsoleOut.Write(value);
            LogOut?.Write(value);
        }

        public static void Write(char[] buffer)
        {
            ConsoleOut.Write(buffer);
            LogOut?.Write(buffer);
        }

        public static void Write(char[] buffer, int index, int count)
        {
            ConsoleOut.Write(buffer, index, count);
            LogOut?.Write(buffer, index, count);
        }

        public static void Write(bool value)
        {
            ConsoleOut.Write(value);
            LogOut?.Write(value);
        }

        public static void Write(int value)
        {
            ConsoleOut.Write(value);
            LogOut?.Write(value);
        }

        public static void Write(uint value)
        {
            ConsoleOut.Write(value);
            LogOut?.Write(value);
        }

        public static void Write(long value)
        {
            ConsoleOut.Write(value);
            LogOut?.Write(value);
        }

        public static void Write(ulong value)
        {
            ConsoleOut.Write(value);
            LogOut?.Write(value);
        }

        public static void Write(float value)
        {
            ConsoleOut.Write(value);
            LogOut?.Write(value);
        }

        public static void Write(double value)
        {
            ConsoleOut.Write(value);
            LogOut?.Write(value);
        }

        public static void Write(decimal value)
        {
            ConsoleOut.Write(value);
            LogOut?.Write(value);
        }

        public static void Write(string value)
        {
            ConsoleOut.Write(value);
            LogOut?.Write(value);
        }

        public static void Write(string value, Verbosity verbosity)
        {
            ConsoleOut.Write(value, verbosity: verbosity);
            LogOut?.Write(value, verbosity: verbosity);
        }

        public static void Write(string value, ConsoleColor color)
        {
            ConsoleOut.Write(value, color);
            LogOut?.Write(value);
        }

        public static void Write(string value, ConsoleColor color, Verbosity verbosity)
        {
            ConsoleOut.Write(value, color, verbosity: verbosity);
            LogOut?.Write(value, verbosity: verbosity);
        }

        public static void WriteIf(bool condition, string value)
        {
            ConsoleOut.WriteIf(condition, value);
            LogOut?.WriteIf(condition, value);
        }

        public static void WriteIf(bool condition, string value, ConsoleColor color)
        {
            ConsoleOut.WriteIf(condition, value, color);
            LogOut?.WriteIf(condition, value);
        }

        public static void Write(object value)
        {
            ConsoleOut.Write(value);
            LogOut?.Write(value);
        }

        public static void Write(string format, object arg0)
        {
            ConsoleOut.Write(format, arg0);
            LogOut?.Write(format, arg0);
        }

        public static void Write(string format, object arg0, object arg1)
        {
            ConsoleOut.Write(format, arg0, arg1);
            LogOut?.Write(format, arg0, arg1);
        }

        public static void Write(string format, object arg0, object arg1, object arg2)
        {
            ConsoleOut.Write(format, arg0, arg1, arg2);
            LogOut?.Write(format, arg0, arg1, arg2);
        }

        public static void Write(string format, params object[] arg)
        {
            ConsoleOut.Write(format, arg);
            LogOut?.Write(format, arg);
        }

        public static void WriteLine()
        {
            ConsoleOut.WriteLine();
            LogOut?.WriteLine();
        }

        public static void WriteLine(Verbosity verbosity)
        {
            ConsoleOut.WriteLine(verbosity);
            LogOut?.WriteLine(verbosity);
        }

        public static void WriteLineIf(bool condition)
        {
            ConsoleOut.WriteLineIf(condition);
            LogOut?.WriteLineIf(condition);
        }

        public static void WriteLine(char value)
        {
            ConsoleOut.WriteLine(value);
            LogOut?.WriteLine(value);
        }

        public static void WriteLine(char[] buffer)
        {
            ConsoleOut.WriteLine(buffer);
            LogOut?.WriteLine(buffer);
        }

        public static void WriteLine(char[] buffer, int index, int count)
        {
            ConsoleOut.WriteLine(buffer, index, count);
            LogOut?.WriteLine(buffer, index, count);
        }

        public static void WriteLine(bool value)
        {
            ConsoleOut.WriteLine(value);
            LogOut?.WriteLine(value);
        }

        public static void WriteLine(int value)
        {
            ConsoleOut.WriteLine(value);
            LogOut?.WriteLine(value);
        }

        public static void WriteLine(uint value)
        {
            ConsoleOut.WriteLine(value);
            LogOut?.WriteLine(value);
        }

        public static void WriteLine(long value)
        {
            ConsoleOut.WriteLine(value);
            LogOut?.WriteLine(value);
        }

        public static void WriteLine(ulong value)
        {
            ConsoleOut.WriteLine(value);
            LogOut?.WriteLine(value);
        }

        public static void WriteLine(float value)
        {
            ConsoleOut.WriteLine(value);
            LogOut?.WriteLine(value);
        }

        public static void WriteLine(double value)
        {
            ConsoleOut.WriteLine(value);
            LogOut?.WriteLine(value);
        }

        public static void WriteLine(decimal value)
        {
            ConsoleOut.WriteLine(value);
            LogOut?.WriteLine(value);
        }

        public static void WriteLine(string value)
        {
            ConsoleOut.WriteLine(value);
            LogOut?.WriteLine(value);
        }

        public static void WriteLine(string value, Verbosity verbosity)
        {
            ConsoleOut.WriteLine(value, verbosity: verbosity);
            LogOut?.WriteLine(value, verbosity: verbosity);
        }

        public static void WriteLine(string value, ConsoleColor color)
        {
            ConsoleOut.WriteLine(value, color);
            LogOut?.WriteLine(value);
        }

        public static void WriteLine(string value, ConsoleColor color, Verbosity verbosity)
        {
            ConsoleOut.WriteLine(value, color, verbosity: verbosity);
            LogOut?.WriteLine(value, verbosity: verbosity);
        }

        public static void WriteLineIf(bool condition, string value)
        {
            ConsoleOut.WriteLineIf(condition, value);
            LogOut?.WriteLineIf(condition, value);
        }

        public static void WriteLineIf(bool condition, string value, ConsoleColor color)
        {
            ConsoleOut.WriteLineIf(condition, value, color);
            LogOut?.WriteLineIf(condition, value);
        }

        public static void WriteLine(object value)
        {
            ConsoleOut.WriteLine(value);
            LogOut?.WriteLine(value);
        }

        public static void WriteLine(string format, object arg0)
        {
            ConsoleOut.WriteLine(format, arg0);
            LogOut?.WriteLine(format, arg0);
        }

        public static void WriteLine(string format, object arg0, object arg1)
        {
            ConsoleOut.WriteLine(format, arg0, arg1);
            LogOut?.WriteLine(format, arg0, arg1);
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            ConsoleOut.WriteLine(format, arg0, arg1, arg2);
            LogOut?.WriteLine(format, arg0, arg1, arg2);
        }

        public static void WriteLine(string format, params object[] arg)
        {
            ConsoleOut.WriteLine(format, arg);
            LogOut?.WriteLine(format, arg);
        }

        public static void WriteDiagnostic(
            Diagnostic diagnostic,
            string indentation = null,
            Verbosity verbosity = Verbosity.None)
        {
            Write(indentation, verbosity);

            string message = diagnostic.ToString();

            ConsoleOut.WriteLine(message, diagnostic.Severity.GetColor(), verbosity);
            LogOut?.WriteLine(message, verbosity);
        }

        public static void WriteDiagnostics(
            ImmutableArray<Diagnostic> diagnostics,
            string baseDirectoryPath = null,
            IFormatProvider formatProvider = null,
            string indentation = null,
            int maxCount = int.MaxValue,
            DiagnosticDisplayParts parts = DiagnosticDisplayParts.All,
            Verbosity verbosity = Verbosity.None)
        {
            if (!diagnostics.Any())
                return;

            if (verbosity > ConsoleOut.Verbosity
                && (LogOut == null || verbosity > LogOut.Verbosity))
            {
                return;
            }

            int count = 0;

            foreach ((Diagnostic diagnostic, string message) in DiagnosticFormatter.FormatDiagnostics(diagnostics, baseDirectoryPath, formatProvider, parts))
            {
                Write(indentation, verbosity);
                ConsoleOut.WriteLine(message, diagnostic.Severity.GetColor(), verbosity);
                LogOut?.WriteLine(message, verbosity);

                count++;

                if (count > maxCount)
                {
                    int remainingCount = diagnostics.Length - count;

                    if (remainingCount > 0)
                    {
                        Write(indentation, verbosity);
                        WriteLine($"and {remainingCount} more diagnostics", verbosity);
                    }
                }
            }
        }
    }
}
