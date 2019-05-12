// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Formatting.CodeFixes;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class UseLinefeedAsNewlineTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseLinefeedAsNewline;

        public override DiagnosticAnalyzer Analyzer { get; } = new UseLinefeedAsNewlineAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new NewlineCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseLinefeedAsNewline)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync("\n"
+ "class C[|\r\n|]"
+ "{[|\r\n|]"
+ "    /// <summary>[|\r\n|]"
+ "    /// [|\r\n|]"
+ "    /// </summary>[|\r\n|]"
+ "    void M()[|\r\n|]"
+ "    {[|\r\n|]"
+ "    }[|\r\n|]"
+ "}\n",
"\n"
+ "class C\n"
+ "{\n"
+ "    /// <summary>\n"
+ "    /// \n"
+ "    /// </summary>\n"
+ "    void M()\n"
+ "    {\n"
+ "    }\n"
+ "}\n");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseLinefeedAsNewline)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync("\n"
+ "class C\n"
+ "{\n"
+ "    void M()\n"
+ "    {\n"
+ "    }\n"
+ "}\n");
        }
}
}
