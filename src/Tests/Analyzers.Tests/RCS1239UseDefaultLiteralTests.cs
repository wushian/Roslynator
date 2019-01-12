// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1239UseDefaultLiteralTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseDefaultLiteral;

        public override DiagnosticAnalyzer Analyzer { get; } = new DefaultExpressionAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new DefaultExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseDefaultLiteral)]
        public async Task Test_ParameterDefaultValue()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(string s = default[|(string)|])
    {
    }
}
", @"
class C
{
    void M(string s = default)
    {
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseDefaultLiteral)]
        public async Task Test_ExpressionBody()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M() => default[|(string)|];
}
", @"
class C
{
    string M() => default;
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseDefaultLiteral)]
        public async Task TestNoDiagnostic_NonObjectValueAssignedToObject()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        object x = null;

        x = default(int);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseDefaultLiteral)]
        public async Task TestNoDiagnostic_NonNullableValueAssignedToNullable()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        int? x = null;

        x = default(int);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseDefaultLiteral)]
        public async Task TestNoDiagnostic_ValueAssignedToDynamic()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        dynamic x = null;

        x = default(int);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseDefaultLiteral)]
        public async Task TestNoDiagnostic_ConditionalExpression()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool condition = false;

        object x = null;

        x = (condition) ? x : default(int);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseDefaultLiteral)]
        public async Task TestNoDiagnostic_ConditionalExpression_Nullable()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool condition = false;

        int? x = null;

        x = (condition) ? 1 : default(int?);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseDefaultLiteral)]
        public async Task TestNoDiagnostic_ReturnStatement()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    object M()
    {
        return default(int);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseDefaultLiteral)]
        public async Task TestNoDiagnostic_CoalesceExpression()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        object x = null;

        x = x ?? default(int);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseDefaultLiteral)]
        public async Task TestNoDiagnostic_ExpressionBody()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    object M() => default(int);
}
");
        }
    }
}
