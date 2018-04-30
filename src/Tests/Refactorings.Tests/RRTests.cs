// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CodeRefactorings;
using Roslynator.CSharp.Refactorings;
using Xunit;
using static Roslynator.Tests.CSharp.CSharpCodeRefactoringVerifier;

namespace Roslynator.Refactorings.Tests
{
    public static class RRTests
    {
        private const string RefactoringId = RefactoringIdentifiers.AddBraces;

        private static CodeRefactoringProvider CodeRefactoringProvider { get; } = new RoslynatorCodeRefactoringProvider();

        //[Fact]
        public static void TestRefactoring()
        {
            Instance.VerifyRefactoring(@"
using System;
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
    }
}
", @"
", CodeRefactoringProvider, RefactoringId);
        }

        //[Theory]
        //[InlineData("", "")]
        public static void TestRefactoring2(string fixableCode, string fixedCode)
        {
            Instance.VerifyRefactoring(@"
using System;
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
    }
}
", fixableCode, fixedCode, CodeRefactoringProvider, RefactoringId);
        }

        //[Fact]
        public static void TestNoRefactoring()
        {
            Instance.VerifyNoRefactoring(@"
using System;
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
    }
}
", CodeRefactoringProvider, RefactoringId);
        }
    }
}
