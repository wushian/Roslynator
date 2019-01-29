// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class DiagnosticIdPrefix
    {
        public static string GetPrefix(string id)
        {
            int length = id.Length;

            if (length == 0)
                return "";

            switch (id[0])
            {
                case 'A':
                    {
                        if (HasPrefix("Async"))
                        {
                            return "Async";
                        }

                        break;
                    }
                case 'C':
                    {
                        if (HasPrefix("CA"))
                        {
                            return "CA";
                        }
                        else if (HasPrefix("CC"))
                        {
                            return "CC";
                        }
                        else if (HasPrefix("CS"))
                        {
                            return "CS";
                        }

                        break;
                    }
                case 'R':
                    {
                        if (HasPrefix("RS"))
                        {
                            return "RS";
                        }
                        else if (HasPrefix("RCS"))
                        {
                            return "RCS";
                        }
                        else if (HasPrefix("RECS"))
                        {
                            return "RECS";
                        }
                        else if (HasPrefix("REVB"))
                        {
                            return "REVB";
                        }

                        break;
                    }
                case 'U':
                    {
                        if (HasPrefix("U2U"))
                        {
                            return "U2U";
                        }

                        break;
                    }
                case 'V':
                    {
                        if (HasPrefix("VB"))
                        {
                            return "VB";
                        }

                        break;
                    }
                case 'x':
                    {
                        if (HasPrefix("xUnit"))
                        {
                            return "xUnit";
                        }

                        break;
                    }
            }

            int prefixLength = GetPrefixLength(id);

            string prefix = id.Substring(0, prefixLength);

            Debug.Fail((prefix.Length > 0) ? prefix : id);

            return prefix;

            bool HasPrefix(string value)
            {
                return length > value.Length
                    && char.IsDigit(id, value.Length)
                    && string.Compare(id, 1, value, 1, value.Length - 1, StringComparison.Ordinal) == 0;
            }
        }

        public static int GetPrefixLength(string id)
        {
            int length = id.Length;

            int i = length - 1;

            while (i >= 0
                && char.IsLetter(id[i]))
            {
                i--;
            }

            while (i >= 0
                && char.IsDigit(id[i]))
            {
                i--;
            }

            return i + 1;
        }

        public static IEnumerable<(string prefix, int count)> CountPrefixes(IEnumerable<string> values)
        {
            foreach (IGrouping<string, string> grouping in values
                .GroupBy(f => GetPrefix(f))
                .OrderBy(f => f.Key))
            {
                yield return (grouping.Key, grouping.Count());
            }
        }
    }
}
