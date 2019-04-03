// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0213AddMissingPropertiesToObjectInitializerTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.AddMissingPropertiesToObjectInitializer;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddMissingPropertiesToObjectInitializer)]
        public async Task Test_EmptyInitializer()
        {
            await VerifyRefactoringAsync(@"
using System;

class C
{
    public string P1 { get; set; }
    public int? P3 { get; set; }
    public int P2 { get; set; }
    public DateTime P4 { get; set; }

    void M()
    {
        var x = new C() {[||] };
    }
}
", @"
using System;

class C
{
    public string P1 { get; set; }
    public int? P3 { get; set; }
    public int P2 { get; set; }
    public DateTime P4 { get; set; }

    void M()
    {
        var x = new C() { P1 = null, P2 = 0, P3 = null, P4 = default };
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddMissingPropertiesToObjectInitializer)]
        public async Task Test_Accessibility()
        {
            await VerifyRefactoringAsync(@"
class C
{
    public string P0 { get; set; }
    public string P1 { get; set; }
    public string P2 { get; protected set; }
    public string P3 { get; private set; }
    public string P4 { get; }
}

class C2 : C
{
    void M()
    {
        var x = new C2() { P0 = null, [||] };
    }
}
", @"
class C
{
    public string P0 { get; set; }
    public string P1 { get; set; }
    public string P2 { get; protected set; }
    public string P3 { get; private set; }
    public string P4 { get; }
}

class C2 : C
{
    void M()
    {
        var x = new C2() { P0 = null, P1 = null, P2 = null };
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddMissingPropertiesToObjectInitializer)]
        public async Task TestNoRefactoring_AllPropertiesInitialized()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    public string P1 { get; set; }
    public int P2 { get; set; }

    void M()
    {
        var x = new C() { P1 = null, P2 = 0 [||] };
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddMissingPropertiesToObjectInitializer)]
        public async Task TestNoRefactoring_AnonymousType()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    void M()
    {
        var x = new {[||] P = 0 };
    }
}
", equivalenceKey: RefactoringId);
        }
    }
}
