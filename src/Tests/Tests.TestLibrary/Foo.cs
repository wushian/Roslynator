// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#region usings
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;
#endregion usings

#pragma warning disable RCS1018, RCS1118, RCS1213, CA1822

namespace Roslynator.Tests
{
    class C
    {
        void M()
        {
            var items = new List<string>();

            bool x = items.Any(f => string.IsNullOrEmpty(f));
        }

        void M2()
        {
            var items = new List<string>();

            bool x = false;
            foreach (string item in items)
            {
                if (string.IsNullOrEmpty(item))
                {
                    x = true;
                    break;
                }
            }
        }
    }

    class C_SimpleLambda_BlockBody
    {
        void M()
        {
            var items = new List<string>();

            bool x = items.Any(f =>
            {
                object obj = null;

                if (obj == null)
                {
                    return false;
                }
                else
                {
                    return string.IsNullOrEmpty(f);
                }
            });
        }

        void M2()
        {
            var items = new List<string>();

            bool x = Any();

            bool Any()
            {
                foreach (string f in items)
                {
                    object obj = null;

                    if (obj == null)
                    {
                        return false;
                    }
                    else
                    {
                        return string.IsNullOrEmpty(f);
                    }
                }

                return false;
            }
        }
    }

    class C_IfStatement
    {
        void M()
        {
            var items = new List<string>();

            if (items.Any(f => string.IsNullOrEmpty(f)))
            {
            }
        }

        void M2()
        {
            var items = new List<string>();

            bool x = false;
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item))
                {
                    x = true;
                    break;
                }
            }

            if (x)
            {
            }
        }
    }

    class C_While
    {
        void M()
        {
            var items = new List<string>();

            while (items.Any(f => string.IsNullOrEmpty(f)))
            {
            }
        }

        void M2()
        {
            var items = new List<string>();

            bool x = false;
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item))
                {
                    x = true;
                    break;
                }
            }

            while (x)
            {
            }
        }
    }

    class C_ConditionalExpression
    {
        void M()
        {
            var items = new List<string>();


            string x = (items.Any(f => string.IsNullOrEmpty(f))) ? "true" : "false";
        }

        void M2()
        {
            var items = new List<string>();

            string x = "false";
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item))
                {
                    x = "true";
                    break;
                }
            }
        }
    }

    class C_FirstOrDefault
    {
        void M()
        {
            string s = null;
            IEnumerable<string> items = Enumerable.Empty<string>();

            var x = items.Select(f => new { P = f }).FirstOrDefault(f => object.Equals(f, s));
        }
    }
}
