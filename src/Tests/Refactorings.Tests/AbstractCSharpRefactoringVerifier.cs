// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CodeRefactorings;
using Roslynator.CSharp.Testing;
using Roslynator.Testing;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public abstract class AbstractCSharpRefactoringVerifier : CSharpRefactoringVerifier
    {
        protected override Assert Assert => XunitAssert.Instance;

        public override CodeRefactoringProvider RefactoringProvider { get; } = new RoslynatorCodeRefactoringProvider();
    }
}
