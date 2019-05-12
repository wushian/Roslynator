// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Formatting.CodeFixes.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class PlaceConditionalOperatorBeforeExpressionTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.PlaceConditionalOperatorBeforeExpression;

        public override DiagnosticAnalyzer Analyzer { get; } = new PlaceConditionalOperatorBeforeExpressionAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new TokenCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceConditionalOperatorBeforeExpression)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x) [|?|]
            y :
            z;
    }
}
", @"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x)
            ? y
            : z;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceConditionalOperatorBeforeExpression)]
        public async Task Test2()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x) [|?|]
            y :
            z;
    }
}
", @"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x)
            ? y
            : z;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceConditionalOperatorBeforeExpression)]
        public async Task Test3()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x) [|?|]
            y :
            z;
    }
}
", @"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x)
            ? y
            : z;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceConditionalOperatorBeforeExpression)]
        public async Task Test4()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x) [|?|]
            y
            : z;
    }
}
", @"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x)
            ? y
            : z;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceConditionalOperatorBeforeExpression)]
        public async Task Test5()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x)
            ? y [|:|]
            z;
    }
}
", @"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x)
            ? y
            : z;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceConditionalOperatorBeforeExpression)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x)
            ? y
            : z;
    }
}
");
        }
    }
}
