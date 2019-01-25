// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1241UseLoopStatementInsteadOfForEachMethodTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseLoopStatementInsteadOfForEachMethod;

        public override DiagnosticAnalyzer Analyzer { get; } = new InvocationExpressionAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new InvocationExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseLoopStatementInsteadOfForEachMethod)]
        public async Task Test_List_SimpleLambda()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    void M()
    {
        var items = new List<string>();

        [|items.ForEach(item => M(item))|];
    }

    public void M(string s)
    {
    }
}
", @"
using System.Collections.Generic;

class C
{
    void M()
    {
        var items = new List<string>();

        foreach (var item in items)
        {
            M(item);
        }
    }

    public void M(string s)
    {
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseLoopStatementInsteadOfForEachMethod)]
        public async Task Test_List_ParenthesizedLambda()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    void M()
    {
        var items = new List<string>();

        [|items.ForEach((item) => M(item))|];
    }

    public void M(string s)
    {
    }
}
", @"
using System.Collections.Generic;

class C
{
    void M()
    {
        var items = new List<string>();

        foreach (var item in items)
        {
            M(item);
        }
    }

    public void M(string s)
    {
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseLoopStatementInsteadOfForEachMethod)]
        public async Task Test_List_AnonymousMethod()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    void M()
    {
        var items = new List<string>();

        [|items.ForEach(delegate(string item) { M(item); } )|];
    }

    public void M(string s)
    {
    }
}
", @"
using System.Collections.Generic;

class C
{
    void M()
    {
        var items = new List<string>();

        foreach (var item in items)
        { M(item); }
    }

    public void M(string s)
    {
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseLoopStatementInsteadOfForEachMethod)]
        public async Task Test_Array_SimpleLambda()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        var items = new string[0];

        [|Array.ForEach(items, item => M(item))|];
    }

    public void M(string s)
    {
    }
}
", @"
using System;

class C
{
    void M()
    {
        var items = new string[0];

        foreach (var item in items)
        {
            M(item);
        }
    }

    public void M(string s)
    {
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseLoopStatementInsteadOfForEachMethod)]
        public async Task Test_Array_ParenthesizedLambda()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        var items = new string[0];

        [|Array.ForEach(items, (item) => M(item))|];
    }

    public void M(string s)
    {
    }
}
", @"
using System;

class C
{
    void M()
    {
        var items = new string[0];

        foreach (var item in items)
        {
            M(item);
        }
    }

    public void M(string s)
    {
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseLoopStatementInsteadOfForEachMethod)]
        public async Task Test_Array_AnonymousMethod()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        var items = new string[0];

        [|Array.ForEach(items, delegate(string item) { M(item); })|];
    }

    public void M(string s)
    {
    }
}
", @"
using System;

class C
{
    void M()
    {
        var items = new string[0];

        foreach (var item in items)
        { M(item); }
    }

    public void M(string s)
    {
    }
}
");
        }
    }
}
