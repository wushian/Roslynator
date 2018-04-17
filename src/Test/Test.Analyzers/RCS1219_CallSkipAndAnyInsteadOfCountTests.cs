// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.CSharp;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.CodeFixes;
using Xunit;
using static Roslynator.Test.CSharpDiagnosticVerifier;

namespace Roslynator.Test.Analyzers
{
    public static class RCS1219_CallSkipAndAnyInsteadOfCountTests
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
}";

        [Theory]
        [InlineData("items.Count() > i", "items.Skip(i).Any()")]
        [InlineData("i < items.Count()", "items.Skip(i).Any()")]
        [InlineData("items.Count() >= i", "items.Skip(i - 1).Any()")]
        [InlineData("i <= items.Count()", "items.Skip(i - 1).Any()")]
        [InlineData("items.Count() <= i", "!items.Skip(i).Any()")]
        [InlineData("i >= items.Count()", "!items.Skip(i).Any()")]
        [InlineData("items.Count() < i", "!items.Skip(i - 1).Any()")]
        [InlineData("i > items.Count()", "!items.Skip(i - 1).Any()")]
        public static void DiagnosticWithFix(string source, string sourceWithFix)
        {
            VerifyDiagnosticAndCodeFix(
                SourceTemplate,
                source,
                sourceWithFix,
                DiagnosticDescriptors.CallSkipAndAnyInsteadOfCount,
                new InvocationExpressionAnalyzer(),
                new BinaryExpressionCodeFixProvider());
        }

        [Theory]
        [InlineData("items.Count() == 0")]
        [InlineData("items.Count() == 1")]
        [InlineData("items.Count() == i")]
        [InlineData("items.Count() != 0")]
        [InlineData("items.Count() != 1")]
        [InlineData("items.Count() != i")]
        [InlineData("items.Count() > 0")]
        [InlineData("items.Count() >= 1")]
        [InlineData("items.Count() < 1")]
        [InlineData("items.Count() <= 0")]
        [InlineData("0 == items.Count()")]
        [InlineData("1 == items.Count()")]
        [InlineData("i == items.Count()")]
        [InlineData("0 != items.Count()")]
        [InlineData("1 != items.Count()")]
        [InlineData("i != items.Count()")]
        [InlineData("0 < items.Count()")]
        [InlineData("1 <= items.Count()")]
        [InlineData("1 > items.Count()")]
        [InlineData("0 >= items.Count()")]
        public static void NoDiagnostic(string source)
        {
            VerifyNoDiagnostic(
                SourceTemplate,
                source,
                DiagnosticDescriptors.CallSkipAndAnyInsteadOfCount,
                new InvocationExpressionAnalyzer());
        }
    }
}
