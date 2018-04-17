// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.CSharp.Refactorings;
using Xunit;
using static Roslynator.Tests.CSharpCodeRefactoringVerifier;

namespace Roslynator.Refactorings.Tests
{
    public static class RR0048FormatArgumentListTests
    {
        [Fact]
        public static void TestFormatArgumentListToMultiLine()
        {
            VerifyRefactoring(
@"
class C
{
    void M(string p1, string p2, string p3)
    {
        M(p1<<<>>>, p2, p3);
    }
}
",
@"
class C
{
    void M(string p1, string p2, string p3)
    {
        M(
            p1,
            p2,
            p3);
    }
}
",
                codeRefactoringProvider: new RoslynatorCodeRefactoringProvider(),
                equivalenceKey: RefactoringIdentifiers.FormatArgumentList);
        }

        [Fact]
        public static void TestFormatArgumentListToMultiLine2()
        {
            VerifyRefactoring(
@"
class C
{
    void M(string p1, string p2, string p3)
    {
        M<<<(p1, p2, p3)>>>;
    }
}
",
@"
class C
{
    void M(string p1, string p2, string p3)
    {
        M(
            p1,
            p2,
            p3);
    }
}
",
                codeRefactoringProvider: new RoslynatorCodeRefactoringProvider(),
                equivalenceKey: RefactoringIdentifiers.FormatArgumentList);
        }

        [Fact]
        public static void TestFormatArgumentListToSingleLine()
        {
            VerifyRefactoring(
@"
class C
{
    void M(string p1, string p2, string p3)
    {
        M(
            p1<<<>>>,
            p2,
            p3);
    }
}
",
@"
class C
{
    void M(string p1, string p2, string p3)
    {
        M(p1, p2, p3);
    }
}
",
                codeRefactoringProvider: new RoslynatorCodeRefactoringProvider(),
                equivalenceKey: RefactoringIdentifiers.FormatArgumentList);
        }

        [Fact]
        public static void TestFormatArgumentListToSingleLine2()
        {
            VerifyRefactoring(
@"
class C
{
    void M(string p1, string p2, string p3)
    {
        M<<<(
            p1,
            p2,
            p3)>>>;
    }
}
",
@"
class C
{
    void M(string p1, string p2, string p3)
    {
        M(p1, p2, p3);
    }
}
",
                codeRefactoringProvider: new RoslynatorCodeRefactoringProvider(),
                equivalenceKey: RefactoringIdentifiers.FormatArgumentList);
        }

        [Fact]
        public static void TestNoRefactoring()
        {
            VerifyNoRefactoring(
@"
class C
{
    void M(string p1, string p2, string p3)
    {
        M(
            <<<p1,
            p2, //x
            p3>>>);
    }
}
",
                codeRefactoringProvider: new RoslynatorCodeRefactoringProvider(),
                equivalenceKey: RefactoringIdentifiers.FormatArgumentList);
        }
    }
}
