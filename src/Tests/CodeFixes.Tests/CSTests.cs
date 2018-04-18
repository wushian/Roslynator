// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.CSharp;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.CodeFixes;
using Xunit;
using static Roslynator.Tests.CSharpCompilerCodeFixVerifier;

#pragma warning disable xUnit1008

namespace Roslynator.CodeFixes.Tests
{
    public static class CSTests
    {
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

        public static void TestCodeFix()
        {
            VerifyCodeFix(
@"
",
@"
",
                codeFixProvider: default,
                equivalenceKey: default);
        }

        [InlineData("", "")]
        public static void TestCodeFix2(string fixableCode, string fixedCode)
        {
            VerifyCodeFix(
                SourceTemplate,
                fixableCode,
                fixedCode,
                codeFixProvider: default,
                equivalenceKey: default);
        }

        public static void TestNoCodeFix()
        {
            VerifyNoCodeFix(
@"
",
                codeFixProvider: default,
                equivalenceKey: default);
        }
    }
}
