// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Tests.CSharp;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.Analyzers.Tests
{
    public class RCS1008UseExplicitTypeInsteadOfVarWhenTypeIsNotObviousTests : CSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseExplicitTypeInsteadOfVarWhenTypeIsNotObvious;

        public override DiagnosticAnalyzer Analyzer { get; } = new UseExplicitTypeInsteadOfVarWhenTypeIsNotObviousAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new UseExplicitTypeInsteadOfVarCodeFixProvider();

        [Fact]
        public async Task TestDiagnosticWithCodeFix()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        [|var|] a = ""a"";

        [|var|] s = a;

        string value = null;
        if (DateTime.TryParse(s, out [|var|] result))
        {
        }
    }
}
", @"
using System;

class C
{
    void M()
    {
        string a = ""a"";

        string s = a;

        string value = null;
        if (DateTime.TryParse(s, out DateTime result))
        {
        }
    }
}
");
        }

        [Fact]
        public async Task TestDiagnosticWithCodeFix_Tuple()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Collections.Generic;

class C
{
    (IEnumerable<DateTime> e1, string e2) M()
    {
        [|var|] x = M();

        return default((IEnumerable<DateTime>, string));
    }
}
", @"
using System;
using System.Collections.Generic;

class C
{
    (IEnumerable<DateTime> e1, string e2) M()
    {
        (IEnumerable<DateTime> e1, string e2) x = M();

        return default((IEnumerable<DateTime>, string));
    }
}
");
        }

        [Fact]
        internal async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    void M()
    {
        string a = ""a"";

        string s = a;

        string value = null;
        if (DateTime.TryParse(s, out DateTime result))
        {
        }
    }
}
");
        }
    }
}
