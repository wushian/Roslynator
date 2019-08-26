// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Formatting.CodeFixes.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class FormatNodesInListTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.FormatNodesInList;

        public override DiagnosticAnalyzer Analyzer { get; } = new FormatNodesInListAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new FormatNodesInListCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatNodesInList)]
        public async Task Test_If()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M([|object p1,
        object p2, object p3,
        object p4|]) 
    {
    }
}
", @"
class C
{
    void M(
        object p1,
        object p2,
        object p3,
        object p4) 
    {
    }
}
");
        }
    }
}
