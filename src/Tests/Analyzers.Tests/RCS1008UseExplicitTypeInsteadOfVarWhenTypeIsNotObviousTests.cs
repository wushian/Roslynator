// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.CodeFixes;
using Xunit;
using static Roslynator.Tests.CSharp.CSharpDiagnosticVerifier;

namespace Roslynator.Analyzers.Tests
{
    public static class RCS1008UseExplicitTypeInsteadOfVarWhenTypeIsNotObviousTests
    {
        private static DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseExplicitTypeInsteadOfVarWhenTypeIsNotObvious;

        private static DiagnosticAnalyzer Analyzer { get; } = new UseExplicitTypeInsteadOfVarWhenTypeIsNotObviousAnalyzer();

        private static CodeFixProvider CodeFixProvider { get; } = new UseExplicitTypeInsteadOfVarCodeFixProvider();

        private const string SourceTemplate = @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
    }
}
";

        [Fact]
        public static void TestDiagnosticWithCodeFix()
        {
            VerifyDiagnosticAndFix(
@"
using System;

class C
{
    public void B()
    {
        <<<var>>> a = ""a"";

        <<<var>>> s = a;

        string value = null;
        if (DateTime.TryParse(s, out <<<var>>> result))
        {
        }
    }
}
",
@"
using System;

class C
{
    public void B()
    {
        string a = ""a"";

        string s = a;

        string value = null;
        if (DateTime.TryParse(s, out DateTime result))
        {
        }
    }
}
",
                descriptor: Descriptor,
                analyzer: Analyzer,
                fixProvider: CodeFixProvider);
        }

        [Fact]
        public static void TestDiagnosticWithCodeFix_Tuple()
        {
            VerifyDiagnosticAndFix(
@"
using System;
using System.Collections.Generic;

class C
{
    public (IEnumerable<DateTime> e1, string e2) M()
    {
        <<<var>>> x = M();

        return default((IEnumerable<DateTime>, string));
    }
}
",
@"
using System;
using System.Collections.Generic;

class C
{
    public (IEnumerable<DateTime> e1, string e2) M()
    {
        (IEnumerable<DateTime> e1, string e2) x = M();

        return default((IEnumerable<DateTime>, string));
    }
}
",
                descriptor: Descriptor,
                analyzer: Analyzer,
                fixProvider: CodeFixProvider);
        }

        //[Theory]
        //[InlineData("", "")]
        internal static void TestDiagnosticWithCodeFix2(string fixableCode, string fixedCode)
        {
            VerifyDiagnosticAndFix(
                SourceTemplate,
                fixableCode,
                fixedCode,
                descriptor: Descriptor,
                analyzer: Analyzer,
                fixProvider: CodeFixProvider);
        }

        //[Fact]
        internal static void TestNoDiagnostic()
        {
            VerifyNoDiagnostic(
@"
",
                descriptor: Descriptor,
                analyzer: Analyzer);
        }

        //[Theory]
        //[InlineData("")]
        internal static void TestNoDiagnostic2(string fixableCode)
        {
            VerifyNoDiagnostic(
                SourceTemplate,
                fixableCode,
                descriptor: Descriptor,
                analyzer: Analyzer);
        }
    }
}
