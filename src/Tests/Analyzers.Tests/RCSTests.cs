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
    public static class RCSTests
    {
        private static DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddBracesWhenExpressionSpansOverMultipleLines;

        private static DiagnosticAnalyzer Analyzer { get; }

        private static CodeFixProvider CodeFixProvider { get; }

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

        //[Fact]
        public static void TestDiagnosticWithCodeFix()
        {
            VerifyDiagnosticAndFix(
@"
",
@"
",
                descriptor: Descriptor,
                analyzer: Analyzer,
                fixProvider: CodeFixProvider);
        }

        //[Theory]
        //[InlineData("", "")]
        public static void TestDiagnosticWithCodeFix2(string fixableCode, string fixedCode)
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
        public static void TestNoDiagnostic()
        {
            VerifyNoDiagnostic(
@"
",
                descriptor: Descriptor,
                analyzer: Analyzer);
        }

        //[Theory]
        //[InlineData("")]
        public static void TestNoDiagnostic2(string fixableCode)
        {
            VerifyNoDiagnostic(
                SourceTemplate,
                fixableCode,
                descriptor: Descriptor,
                analyzer: Analyzer);
        }
    }
}
