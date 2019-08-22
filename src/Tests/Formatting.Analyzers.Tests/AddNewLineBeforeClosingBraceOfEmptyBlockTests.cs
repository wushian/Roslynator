// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Formatting.CodeFixes.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class AddNewLineBeforeClosingBraceOfEmptyBlockTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddNewLineBeforeClosingBraceOfEmptyBlock;

        public override DiagnosticAnalyzer Analyzer { get; } = new AddNewLineBeforeClosingBraceOfEmptyBlockAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new BlockCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeClosingBraceOfEmptyBlock)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {[||]}
}
", @"
class C
{
    void M()
    {
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeClosingBraceOfEmptyBlock)]
        public async Task Test_WithWhitespace()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    { [||]}
}
", @"
class C
{
    void M()
    {
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeClosingBraceOfEmptyBlock)]
        public async Task TestNoDiagnostic_EmptyLine()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {

    }
}
");
        }
    }
}
