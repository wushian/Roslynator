// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.CSharp.Refactorings;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.Refactorings.Tests
{
    public class RR0173WrapInElseClauseTests : RoslynatorCSharpCodeRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.WrapInElseClause;

        [Fact]
        public async Task TestCodeRefactoring()
        {
            await VerifyRefactoringAsync(@"
class C
{
    public string M(bool f)
    {
        if (f)
        {
            return null;
        }

[|        return null;|]
    }
}
", @"
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
", RefactoringId);
        }

        [Fact]
        public async Task TestCodeRefactoring2()
        {
            await VerifyRefactoringAsync(@"
class C
{
    public string M(bool f)
    {
        if (f)
            return null;

[|        return null;|]
    }
}
", @"
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
", RefactoringId);
        }

        [Fact]
        public async Task TestCodeRefactoring3()
        {
            await VerifyRefactoringAsync(@"
class C
{
    public string M(bool f)
    {
        if (f)
        {
            M(f);
            return null;
        }

[|        M(f);
        return null;|]
    }
}
", @"
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
", RefactoringId);
        }

        [Fact]
        public async Task TestCodeRefactoring4()
        {
            await VerifyRefactoringAsync(@"
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

[|            return null;|]
        }

        return null;
    }
}
", @"
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
", RefactoringId);
        }

        [Fact]
        public async Task TestNoCodeRefactoring()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    public string M(bool f)
    {
        if (f)
        {
            M(f);
        }

[|        return null;|]
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

[|        return null;|]
    }
}
", RefactoringId);
        }
    }
}
