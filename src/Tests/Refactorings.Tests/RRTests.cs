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

        public static void TestRefactoring()
        {
            VerifyRefactoring(
@"
",
@"
",
                span: default,
                codeRefactoringProvider: default,
                equivalenceKey: null);
        }

        [InlineData("", "")]
        public static void TestRefactoring2(string source, string newSource)
        {
            VerifyRefactoring(
                source,
                newSource,
                span: default,
                codeRefactoringProvider: default,
                equivalenceKey: null);
        }

        public static void TestNoRefactoring()
        {
            VerifyNoRefactoring(
@"
",
                span: default,
                codeRefactoringProvider: default,
                equivalenceKey: default);
        }

        [InlineData("")]
        public static void TestNoRefactoring2(string source)
        {
            VerifyNoRefactoring(
                source,
                span: default,
                codeRefactoringProvider: default,
                equivalenceKey: default);
        }
    }
}
