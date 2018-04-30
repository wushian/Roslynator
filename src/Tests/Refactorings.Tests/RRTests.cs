// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Roslynator.CSharp.Refactorings;
using Roslynator.Tests.CSharp;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.Refactorings.Tests
{
    public class RRTests : AbstractCSharpCodeRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.AddBraces;

        //[Fact]
        public async Task TestRefactoring()
        {
            await VerifyRefactoringAsync(@"
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
", RefactoringId);
        }

        //[Theory]
        //[InlineData("", "")]
        public async Task TestRefactoring2(string fixableCode, string fixedCode)
        {
            await VerifyRefactoringAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
    }
}
", fixableCode, fixedCode, RefactoringId);
        }

        //[Fact]
        public async Task TestNoRefactoring()
        {
            await VerifyNoRefactoringAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
    }
}
", RefactoringId);
        }
    }
}
