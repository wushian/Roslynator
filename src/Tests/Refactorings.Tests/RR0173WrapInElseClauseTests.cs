// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Roslynator.CSharp.Refactorings;
using Xunit;
using static Roslynator.Tests.CSharp.CSharpCodeRefactoringVerifier;

namespace Roslynator.Refactorings.Tests
{
    public static class RR0173WrapInElseClauseTests
    {
        private const string RefactoringId = RefactoringIdentifiers.WrapInElseClause;

        private static CodeRefactoringProvider CodeRefactoringProvider { get; } = new RoslynatorCodeRefactoringProvider();

        [Fact]
        public static void TestCodeRefactoring()
        {
            VerifyCodeRefactoring(
@"
class C
{
    public string M(bool f)
    {
        if (f)
        {
            return null;
        }

<<<        return null;>>>
    }
}
",
@"
class C
{
    public string M(bool f)
    {
        if (f)
        {
            return null;
        }
        else
        {
            return null;
        }
    }
}
",
                codeRefactoringProvider: CodeRefactoringProvider,
                equivalenceKey: RefactoringId);
        }

        [Fact]
        public static void TestCodeRefactoring2()
        {
            VerifyCodeRefactoring(
@"
class C
{
    public string M(bool f)
    {
        if (f)
            return null;

<<<        return null;>>>
    }
}
",
@"
class C
{
    public string M(bool f)
    {
        if (f)
            return null;
        else
            return null;
    }
}
",
                codeRefactoringProvider: CodeRefactoringProvider,
                equivalenceKey: RefactoringId);
        }

        [Fact]
        public static void TestCodeRefactoring3()
        {
            VerifyCodeRefactoring(
@"
class C
{
    public string M(bool f)
    {
        if (f)
        {
            M(f);
            return null;
        }

<<<        M(f);
        return null;>>>
    }
}
",
@"
class C
{
    public string M(bool f)
    {
        if (f)
        {
            M(f);
            return null;
        }
        else
        {
            M(f);
            return null;
        }
    }
}
",
                codeRefactoringProvider: CodeRefactoringProvider,
                equivalenceKey: RefactoringId);
        }

        [Fact]
        public static void TestCodeRefactoring4()
        {
            VerifyCodeRefactoring(
@"
using System;

class C
{
    public string M(bool f)
    {
        var items = new string[0];

        foreach (string item in items)
        {
            if (f)
            {
                break;
            }
            else if (f)
            {
                continue;
            }
            else if (f)
            {
                return null;
            }
            else if (f)
            {
                throw new InvalidOperationException();
            }

<<<            return null;>>>
        }

        return null;
    }
}
",
@"
using System;

class C
{
    public string M(bool f)
    {
        var items = new string[0];

        foreach (string item in items)
        {
            if (f)
            {
                break;
            }
            else if (f)
            {
                continue;
            }
            else if (f)
            {
                return null;
            }
            else if (f)
            {
                throw new InvalidOperationException();
            }
            else
            {
                return null;
            }
        }

        return null;
    }
}
",
                codeRefactoringProvider: CodeRefactoringProvider,
                equivalenceKey: RefactoringId);
        }

        [Fact]
        public static void TestNoCodeRefactoring()
        {
            VerifyNoCodeRefactoring(
@"
class C
{
    public string M(bool f)
    {
        if (f)
        {
            M(f);
        }

<<<        return null;>>>
    }

    public string M2(bool f)
    {
        if (f)
        {
            return null;
        }
        else
        {
        }

<<<        return null;>>>
    }
}
",
                codeRefactoringProvider: CodeRefactoringProvider,
                equivalenceKey: RefactoringId);
        }
    }
}
