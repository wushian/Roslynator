// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RRX003ReplaceLinqWithForEachTests : AbstractCSharpCodeRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.ReplaceLinqWithForEach;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceLinqWithForEach)]
        public async Task Test_Any_SimpleLambda()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    bool M(IEnumerable<string> items, string s)
    {
        return items.[||]Any(f => f == s);
    }
}
", @"
using System.Collections.Generic;
using System.Linq;

class C
{
    bool M(IEnumerable<string> items, string s)
    {
        foreach (string f in items)
        {
            if (f == s)
            {
                return true;
            }
        }

        return false;
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceLinqWithForEach)]
        public async Task Test_Any_SimpleLambda_ExpressionBody()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    bool M(IEnumerable<string> items, string s) => items.[||]Any(f => f == s);
}
", @"
using System.Collections.Generic;
using System.Linq;

class C
{
    bool M(IEnumerable<string> items, string s)
    {
        foreach (string f in items)
        {
            if (f == s)
            {
                return true;
            }
        }

        return false;
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceLinqWithForEach)]
        public async Task Test_Any_SimpleLambda_AnonymousType()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    bool M(IEnumerable<string> items, string s)
    {
        return items.Select(f => new { P = f }).[||]Any(f => object.Equals(f, s));
    }
}
", @"
using System.Collections.Generic;
using System.Linq;

class C
{
    bool M(IEnumerable<string> items, string s)
    {
        foreach (var f in items.Select(f => new { P = f }))
        {
            if (object.Equals(f, s))
            {
                return true;
            }
        }

        return false;
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceLinqWithForEach)]
        public async Task Test_Any_ParenthesizedLambda()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    bool M(IEnumerable<string> items, string s)
    {
        return items.[||]Any((f) => f == s);
    }
}
", @"
using System.Collections.Generic;
using System.Linq;

class C
{
    bool M(IEnumerable<string> items, string s)
    {
        foreach (string f in items)
        {
            if (f == s)
            {
                return true;
            }
        }

        return false;
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceLinqWithForEach)]
        public async Task Test_All_SimpleLambda()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    bool M(IEnumerable<string> items, string s)
    {
        return items.[||]All(f => f == s);
    }
}
", @"
using System.Collections.Generic;
using System.Linq;

class C
{
    bool M(IEnumerable<string> items, string s)
    {
        foreach (string f in items)
        {
            if (f != s)
            {
                return false;
            }
        }

        return true;
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceLinqWithForEach)]
        public async Task Test_FirstOrDefault_SimpleLambda()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    string M(IEnumerable<string> items, string s)
    {
        return items.[||]FirstOrDefault(f => f == s);
    }
}
", @"
using System.Collections.Generic;
using System.Linq;

class C
{
    string M(IEnumerable<string> items, string s)
    {
        foreach (string f in items)
        {
            if (f == s)
            {
                return f;
            }
        }

        return null;
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceLinqWithForEach)]
        public async Task TestNoRefactoring_Any_SimpleLambda_BlockBody()
        {
            await VerifyNoRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    bool M(IEnumerable<string> items, string s)
    {
        return items.[||]Any(f =>
        {
            if (f == s)
            {
                return true;
            }
            else
            {
                return false;
            }
        });
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceLinqWithForEach)]
        public async Task TestNoRefactoring_Any_ParenthesizedLambda_BlockBody()
        {
            await VerifyNoRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    bool M(IEnumerable<string> items, string s)
    {
        return items.[||]Any((f) =>
        {
            if (f == s)
            {
                return true;
            }
            else
            {
                return false;
            }
        });
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceLinqWithForEach)]
        public async Task TestNoRefactoring_Any_AnonymousMethod()
        {
            await VerifyNoRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    bool M(IEnumerable<string> items, string s)
    {
        return items.[||]Any(delegate(string f)
        {
            if (f == s)
            {
                return true;
            }
            else
            {
                return false;
            }
        });
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceLinqWithForEach)]
        public async Task TestNoRefactoring_NoCapturedVariable()
        {
            await VerifyNoRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    bool M(IEnumerable<string> items)
    {
        return items.[||]Any(f => string.IsNullOrEmpty(f));
    }
}
", equivalenceKey: RefactoringId);
        }
    }
}
