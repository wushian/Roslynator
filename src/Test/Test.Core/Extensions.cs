// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Test
{
    internal static class Extensions
    {
        public static LinePositionSpan ToLinePositionSpan(this TextSpan span, string s)
        {
            int length = s.Length;

            LinePosition start = GetLinePosition(span.Start, 0);
            LinePosition end = GetLinePosition(span.End, span.Start);

            return new LinePositionSpan(start, end);

            LinePosition GetLinePosition(int startIndex, int endIndex)
            {
                int i = startIndex;

                while (i >= endIndex)
                {
                    if (s[i] == '\r'
                        || s[i] == '\n')
                    {
                        int character = startIndex - i;

                        int line = 0;

                        while (i >= endIndex)
                        {
                            switch (s[i])
                            {
                                case '\n':
                                    {
                                        if (i > endIndex
                                            && s[i - 1] == '\r')
                                        {
                                            i--;
                                        }

                                        line++;
                                        break;
                                    }
                                case '\r':
                                    {
                                        line++;
                                        break;
                                    }
                            }

                            i--;
                        }

                        return new LinePosition(line, character);
                    }

                    i--;
                }

                return new LinePosition(0, startIndex - endIndex);
            }
        }
    }
}
