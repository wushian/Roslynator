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
    public class RCS1084UseCoalesceExpressionInsteadOfConditionalExpressionTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseCoalesceExpressionInsteadOfConditionalExpression;

        public override DiagnosticAnalyzer Analyzer { get; } = new SimplifyNullCheckAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new ConditionalExpressionCodeFixProvider();

        [Theory]
        [InlineData("s != null ? s : \"\"", "s ?? \"\"")]
        [InlineData("s == null ? \"\" : s", "s ?? \"\"")]

        [InlineData("(s != null) ? (s) : (\"\")", "s ?? \"\"")]
        [InlineData("(s == null) ? (\"\") : (s)", "s ?? \"\"")]
        public async Task TestDiagnosticWithCodeFix_ReferenceType(string fixableCode, string fixedCode)
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = null;

        s = [||];
    }
}
", fixableCode, fixedCode);
        }

        [Theory]
        [InlineData("(ni != null) ? ni.Value : 1", "ni ?? 1")]
        [InlineData("(ni == null) ? 1 : ni.Value", "ni ?? 1")]
        [InlineData("(ni.HasValue) ? ni.Value : 1", "ni ?? 1")]
        [InlineData("(!ni.HasValue) ? 1 : ni.Value", "ni ?? 1")]
        public async Task TestDiagnosticWithCodeFix_ValuType(string fixableCode, string fixedCode)
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        int i = 0;
        int? ni = null;

        i = [||];
    }
}
", fixableCode, fixedCode);
        }

        [Fact]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    public unsafe void M()
    {
        string s = """";

        s = (s != null) ? """" : s;
        s = (s == null) ? s : """";
    }
}
");
        }

        [Fact]
        public async Task TestNoDiagnostic_Pointer()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    public unsafe void M()
    {
        int* i = null;

        i = (i == null) ? default(int*) : i;
        i = (i != null) ? i : default(int*);
    }
}
");
        }
    }
}
