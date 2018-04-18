// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit;
using static Roslynator.Tests.CSharpCodeRefactoringVerifier;

#pragma warning disable xUnit1008

namespace Roslynator.Refactorings.Tests
{
    public static class RRTests
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

        public static void TestCodeRefactoring()
        {
            VerifyCodeRefactoring(
@"
",
@"
",
                span: default,
                codeRefactoringProvider: default,
                equivalenceKey: null);
        }

        [InlineData("", "")]
        public static void TestCodeRefactoring2(string fixableCode, string fixedCode)
        {
            VerifyCodeRefactoring(
                SourceTemplate,
                fixableCode,
                fixedCode,
                codeRefactoringProvider: default,
                equivalenceKey: null);
        }

        public static void TestNoCodeRefactoring()
        {
            VerifyNoCodeRefactoring(
@"
",
                span: default,
                codeRefactoringProvider: default,
                equivalenceKey: default);
        }
    }
}
