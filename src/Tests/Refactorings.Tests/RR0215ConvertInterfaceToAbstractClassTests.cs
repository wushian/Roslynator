// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0215ConvertInterfaceToAbstractClassTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.ConvertInterfaceToAbstractClass;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertInterfaceToAbstractClass)]
        public async Task Test_InsideCompilationUnit()
        {
            await VerifyRefactoringAsync(@"
using System;

public interface [||]IFoo
{
    void M(object parameter);

    string P { get; }

    string this[int index] { get; set; }

    event EventHandler E;
}
", @"
using System;

public interface IFoo
{
    void M(object parameter);

    string P { get; }

    string this[int index] { get; set; }

    event EventHandler E;
}

public abstract class Foo
{
    public abstract void M(object parameter);

    public abstract string P { get; }

    public abstract string this[int index] { get; set; }

    public abstract event EventHandler E;
}", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertInterfaceToAbstractClass)]
        public async Task Test_InsideNamespace()
        {
            await VerifyRefactoringAsync(@"
using System;

namespace N
{
    public interface [||]IFoo
    {
        void M(object parameter);
    }
}
", @"
using System;

namespace N
{
    public interface IFoo
    {
        void M(object parameter);
    }
    public abstract class Foo
    {
        public abstract void M(object parameter);
    }
}
", equivalenceKey: RefactoringId);
        }
    }
}
