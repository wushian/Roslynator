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
        public async Task Test_SimpleLambda_ToLocalFunction()
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
        public async Task Test_ParenthesizedLambda_ToLocalFunction()
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
        public async Task Test_SimpleLambda_ToLocalFunction_BlockBody()
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
        public async Task Test_ParenthesizedLambda_ToLocalFunction_BlockBody()
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
        public async Task Test_AnonymousMethod_ToLocalFunction_BlockBody()
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
        public async Task Test_SimpleLambda_ToLocalFunction_ExpressionBody()
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
    }
}
