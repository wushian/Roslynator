// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Text;

namespace Roslynator.Test
{
    public static class TextUtility
    {
        public static (string newText, List<Diagnostic> diagnostics) GetMarkedDiagnostics(
            string text,
            DiagnosticDescriptor descriptor,
            string filePath)
        {
            StringBuilder sb = StringBuilderCache.GetInstance(text.Length);

            var diagnostics = new List<Diagnostic>();

            int lastPos = 0;

            int line = 0;
            int column = 0;

            int startLine = -1;
            int startColumn = -1;

            int length = text.Length;

            for (int i = 0; i < length; i++)
            {
                switch (text[i])
                {
                    case '\r':
                        {
                            if (i < length - 1
                                && text[i + 1] == '\n')
                            {
                                i++;
                            }

                            line++;
                            column = 0;
                            continue;
                        }
                    case '\n':
                        {
                            line++;
                            column = 0;
                            continue;
                        }
                    case '<':
                        {
                            if (i < length - 1
                                && text[i + 1] == '<'
                                && i < length - 2
                                && text[i + 2] == '<'
                                && i < length - 3
                                && text[i + 3] != '<')
                            {
                                sb.Append(text, lastPos, i - lastPos);

                                startLine = line;
                                startColumn = column;

                                i += 2;

                                lastPos = i + 1;

                                continue;
                            }

                            break;
                        }
                    case '>':
                        {
                            if (startColumn != -1
                                && i < length - 1
                                && text[i + 1] == '>'
                                && i < length - 2
                                && text[i + 2] == '>'
                                && i < length - 3
                                && text[i + 3] != '>')
                            {
                                sb.Append(text, lastPos, i - lastPos);

                                var lineSpan = new LinePositionSpan(
                                    new LinePosition(startLine, startColumn),
                                    new LinePosition(line, column));

                                TextSpan span = TextSpan.FromBounds(lastPos, i);

                                Location location = Location.Create(filePath, span, lineSpan);

                                Diagnostic diagnostic = Diagnostic.Create(descriptor, location);

                                diagnostics.Add(diagnostic);

                                i += 2;

                                lastPos = i + 1;

                                startLine = -1;
                                startColumn = -1;

                                continue;
                            }

                            break;
                        }
                }

                column++;
            }

            sb.Append(text, lastPos, text.Length - lastPos);

            return (StringBuilderCache.GetStringAndFree(sb), diagnostics);
        }
    }
}
