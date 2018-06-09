// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RRX002ExtractLinqToLocalFunctionTests : AbstractCSharpCodeRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.ExtractLinqToLocalFunction;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ExtractLinqToLocalFunction)]
        public async Task Test_Any_SimpleLambda()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        string s = null;
        var items = new List<string>();

        if (true)
        {
            string s2 = null;
            bool x = items.[||]Any(f => f == s || f == s2);
        }
    }
}
", @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        string s = null;
        var items = new List<string>();

        if (true)
        {
            string s2 = null;
            bool x = Any(s2);
        }
        bool Any(string s2)
        {
            foreach (string f in items)
            {
                if (f == s || f == s2)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ExtractLinqToLocalFunction)]
        public async Task Test_Any_SimpleLambda_AnonymousType()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        string s = null;
        var items = new List<string>();

        bool x = items.Select(f => new { P = f }).[||]Any(f => object.Equals(f, s));
    }
}
", @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        string s = null;
        var items = new List<string>();

        bool x = Any();
        bool Any()
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
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ExtractLinqToLocalFunction)]
        public async Task Test_Any_ParenthesizedLambda()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        string s = null;
        var items = new List<string>();

        bool x = items.[||]Any((f) => f == s);
    }
}
", @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        string s = null;
        var items = new List<string>();

        bool x = Any();
        bool Any()
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
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ExtractLinqToLocalFunction)]
        public async Task Test_Any_SimpleLambda_BlockBody()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        string s = null;
        var items = new List<string>();

        bool x = items.[||]Any(f =>
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
", @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        string s = null;
        var items = new List<string>();

        bool x = Any2();
        bool Any2()
        {
            foreach (string f in items)
            {
                if (Any(f))
                {
                    return true;
                }
            }

            return false;
            bool Any(string f)
            {
                if (f == s)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ExtractLinqToLocalFunction)]
        public async Task Test_Any_ParenthesizedLambda_BlockBody()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        string s = null;
        var items = new List<string>();

        bool x = items.[||]Any((f) =>
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
", @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        string s = null;
        var items = new List<string>();

        bool x = Any2();
        bool Any2()
        {
            foreach (string f in items)
            {
                if (Any(f))
                {
                    return true;
                }
            }

            return false;
            bool Any(string f)
            {
                if (f == s)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ExtractLinqToLocalFunction)]
        public async Task Test_Any_AnonymousMethod_BlockBody()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        string s = null;
        var items = new List<string>();

        bool x = items.[||]Any(delegate(string f)
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
", @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        string s = null;
        var items = new List<string>();

        bool x = Any2();
        bool Any2()
        {
            foreach (string f in items)
            {
                if (Any(f))
                {
                    return true;
                }
            }

            return false;
            bool Any(string f)
            {
                if (f == s)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ExtractLinqToLocalFunction)]
        public async Task Test_Any_SimpleLambda_ExpressionBody()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    bool M(IEnumerable<string> items, string s) => items.[||]Any(f =>
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
", @"
using System.Collections.Generic;
using System.Linq;

class C
{
    bool M(IEnumerable<string> items, string s)
    {
        return Any2();
        bool Any2()
        {
            foreach (string f in items)
            {
                if (Any(f))
                {
                    return true;
                }
            }

            return false;
            bool Any(string f)
            {
                if (f == s)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ExtractLinqToLocalFunction)]
        public async Task Test_All_SimpleLambda()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        string s = null;
        var items = new List<string>();

        if (true)
        {
            string s2 = null;
            bool x = items.[||]All(f => f == s || f == s2);
        }
    }
}
", @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        string s = null;
        var items = new List<string>();

        if (true)
        {
            string s2 = null;
            bool x = All(s2);
        }
        bool All(string s2)
        {
            foreach (string f in items)
            {
                if (f != s && f != s2)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ExtractLinqToLocalFunction)]
        public async Task Test_FirstOrDefault_SimpleLambda()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        string s = null;
        var items = new List<string>();

        if (true)
        {
            string s2 = null;
            string x = items.[||]FirstOrDefault(f => f == s || f == s2);
        }
    }
}
", @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        string s = null;
        var items = new List<string>();

        if (true)
        {
            string s2 = null;
            string x = FirstOrDefault(s2);
        }
        string FirstOrDefault(string s2)
        {
            foreach (string f in items)
            {
                if (f == s || f == s2)
                {
                    return f;
                }
            }

            return null;
        }
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ExtractLinqToLocalFunction)]
        public async Task TestNoRefactoring_NoCapturedVariable()
        {
            await VerifyNoRefactoringAsync(@"
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
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ExtractLinqToLocalFunction)]
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

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ExtractLinqToLocalFunction)]
        public async Task TestNoRefactoring_Any_BlockBody_TypeDoesNotSupportExplicitDeclaration()
        {
            await VerifyNoRefactoringAsync(@"
using System.Linq;

class C
{
    void M()
    {
        string s = null;
        bool x = Enumerable.Empty<string>().Select(f => new { P = f }).[||]Any(f =>
        {
            return object.Equals(f, s);
        });
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ExtractLinqToLocalFunction)]
        public async Task TestNoRefactoring_FirstOrDefault_TypeDoesNotSupportExplicitDeclaration()
        {
            await VerifyNoRefactoringAsync(@"
using System.Linq;

class C
{
    void M()
    {
        string s = null;
        var x = Enumerable.Empty<string>().Select(f => new { P = f }).[||]FirstOrDefault(f => object.Equals(f, s));
    }
}
", equivalenceKey: RefactoringId);
        }
    }
}
