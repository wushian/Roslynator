// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.CSharp;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.CodeFixes;
using Xunit;
using static Roslynator.Tests.CSharpDiagnosticVerifier;

namespace Roslynator.Analyzers.Tests
{
    public static class RCS1083CallAnyInsteadOfCountTests
    {
        private const string SourceTemplate = @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        int i = 0;
        IEnumerable<object> items = null;

        if (<<<>>>)
        {
        }
    }
}
";

        [Theory]
        [InlineData("items.Count() != 0", "items.Any()")]
        [InlineData("items.Count() > 0", "items.Any()")]
        [InlineData("items.Count() >= 1", "items.Any()")]
        [InlineData("0 != items.Count()", "items.Any()")]
        [InlineData("0 < items.Count()", "items.Any()")]
        [InlineData("1 <= items.Count()", "items.Any()")]
        [InlineData("items.Count() == 0", "!items.Any()")]
        [InlineData("items.Count() < 1", "!items.Any()")]
        [InlineData("items.Count() <= 0", "!items.Any()")]
        [InlineData("0 == items.Count()", "!items.Any()")]
        [InlineData("1 > items.Count()", "!items.Any()")]
        [InlineData("0 >= items.Count()", "!items.Any()")]
        public static void TestDiagnosticWithFix(string source, string sourceWithFix)
        {
            VerifyDiagnosticAndCodeFix(
                SourceTemplate,
                source,
                sourceWithFix,
                DiagnosticDescriptors.CallAnyInsteadOfCount,
                new InvocationExpressionAnalyzer(),
                new BinaryExpressionCodeFixProvider());
        }

        [Theory]
        [InlineData("items.Count() == 1")]
        [InlineData("items.Count() == i")]
        [InlineData("items.Count() != 1")]
        [InlineData("items.Count() != i")]
        [InlineData("items.Count() > i")]
        [InlineData("items.Count() >= i")]
        [InlineData("items.Count() <= i")]
        [InlineData("items.Count() < i")]
        [InlineData("1 == items.Count()")]
        [InlineData("i == items.Count()")]
        [InlineData("1 != items.Count()")]
        [InlineData("i != items.Count()")]
        [InlineData("i < items.Count()")]
        [InlineData("i <= items.Count()")]
        [InlineData("i >= items.Count()")]
        [InlineData("i > items.Count()")]
        public static void TestNoDiagnostic(string source)
        {
            VerifyNoDiagnostic(
                SourceTemplate,
                source,
                DiagnosticDescriptors.CallAnyInsteadOfCount,
                new InvocationExpressionAnalyzer());
        }
    }
}
