// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.CSharp;
using Roslynator.Tests.CSharp;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.CodeFixes.Tests
{
    public class CSTests : AbstractCSharpCompilerCodeFixVerifier
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.OperatorCannotBeAppliedToOperands;

        public override CodeFixProvider FixProvider { get; }

        //[Fact]
        public async Task TestFix()
        {
            await VerifyFixAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
    }
}
", @"
", EquivalenceKey.Create(DiagnosticId));
        }

        //[Theory]
        //[InlineData("", "")]
        public async Task TestFix2(string fixableCode, string fixedCode)
        {
            await VerifyFixAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
    }
}
", fixableCode, fixedCode, EquivalenceKey.Create(DiagnosticId));
        }

        //[Fact]
        public async Task TestNoFix()
        {
            await VerifyNoFixAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
    }
}
", EquivalenceKey.Create(DiagnosticId));
        }
    }
}
