// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RRX002ReplaceLinqWithForEachTests : AbstractCSharpCodeRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.ReplaceLinqWithForEach;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceLinqWithForEach)]
        public async Task Test()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        var items = new List<string>();

        bool x = items.[||]Any(f => string.IsNullOrEmpty(f));
    }
}
", @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        var items = new List<string>();

        bool x = false;
        foreach (var f in items)
        {
            if (string.IsNullOrEmpty(f))
            {
                x = true;
                break;
            }
        }
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceLinqWithForEach)]
        public async Task TestNoRefactoring_BodyIsBlock()
        {
            await VerifyNoRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        var items = new List<string>();

        bool x = items.[||]Any(f =>
        {
            return string.IsNullOrEmpty(f);
        });
    }
}
", equivalenceKey: RefactoringId);
        }
    }
}
