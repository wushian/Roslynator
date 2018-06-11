// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RRX004ExpandLinqOperationTests : AbstractCSharpCodeRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.ExpandLinqOperation;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceLinqWithForEach)]
        public async Task Test_Select_SimpleLambda()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M(IEnumerable<string> items, string s)
    {
        foreach (int item in items.[||]Select(f => f.Length + s.Length))
        {
            int i = item;
        }
    }
}
", @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M(IEnumerable<string> items, string s)
    {
        foreach (string f in items)
        {
            int item = f.Length + s.Length;
            int i = item;
        }
    }
}
", equivalenceKey: RefactoringId);
    }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceLinqWithForEach)]
        public async Task Test_Where_SimpleLambda()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M(IEnumerable<string> items, string s)
    {
        foreach (string item in items.[||]Where(f => f == s))
        {
            string s2 = item;
        }
    }
}
", @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M(IEnumerable<string> items, string s)
    {
        foreach (string item in items)
        {
            if (item == s)
            {
                string s2 = item;
            }
        }
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceLinqWithForEach)]
        public async Task Test_OfType_SimpleLambda()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M(IEnumerable<object> items)
    {
        foreach (string item in items.[||]OfType<string>())
        {
            string s2 = item;
        }
    }
}
", @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M(IEnumerable<object> items)
    {
        foreach (var item2 in items)
        {
            if (item2 is string item)
            {
                string s2 = item;
            }
        }
    }
}
", equivalenceKey: RefactoringId);
        }
    }
}
