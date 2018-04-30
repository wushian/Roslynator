// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CodeRefactorings;
using Roslynator.CSharp.Refactorings;
using Roslynator.Tests.CSharp;

namespace Roslynator.Refactorings.Tests
{
    public abstract class RoslynatorCSharpCodeRefactoringVerifier : CSharpCodeRefactoringVerifier
    {
        public override CodeRefactoringProvider RefactoringProvider { get; } = new RoslynatorCodeRefactoringProvider();
    }
}
