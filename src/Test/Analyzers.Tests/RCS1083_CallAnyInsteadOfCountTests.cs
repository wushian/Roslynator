// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Test;
using Xunit;

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class RCS1083_CallAnyInsteadOfCountTest
    {
        [Fact]
        public static void DiagnosticWithFix1()
        {
            DiagnosticVerifier.VerifyCSharpDiagnosticWithFix(
@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M(IEnumerable<object> items)
    {
        if (<<<items.Count() != 0>>>) { }
    }
}",
@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M(IEnumerable<object> items)
    {
        if (items.Any()) { }
    }
}",
                DiagnosticDescriptors.CallAnyInsteadOfCount,
                new InvocationExpressionAnalyzer(),
                new BinaryExpressionCodeFixProvider());
        }

        [Fact]
        public static void DiagnosticWithFix2()
        {
            DiagnosticVerifier.VerifyCSharpDiagnosticWithFix(
@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M(IEnumerable<object> items)
    {
        if (<<<items.Count() != 0>>>) { }
        if (<<<items.Count() > 0>>>) { }
        if (<<<items.Count() >= 1>>>) { }
        if (<<<0 != items.Count()>>>) { }
        if (<<<0 < items.Count()>>>) { }
        if (<<<1 <= items.Count()>>>) { }

        if (<<<items.Count() == 0>>>) { }
        if (<<<items.Count() < 1>>>) { }
        if (<<<items.Count() <= 0>>>) { }
        if (<<<0 == items.Count()>>>) { }
        if (<<<1 > items.Count()>>>) { }
        if (<<<0 >= items.Count()>>>) { }
    }
}",
@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M(IEnumerable<object> items)
    {
        if (items.Any()) { }
        if (items.Any()) { }
        if (items.Any()) { }
        if (items.Any()) { }
        if (items.Any()) { }
        if (items.Any()) { }

        if (!items.Any()) { }
        if (!items.Any()) { }
        if (!items.Any()) { }
        if (!items.Any()) { }
        if (!items.Any()) { }
        if (!items.Any()) { }
    }
}",
                DiagnosticDescriptors.CallAnyInsteadOfCount,
                new InvocationExpressionAnalyzer(),
                new BinaryExpressionCodeFixProvider());
        }

        [Fact]
        public static void NoDiagnostic()
        {
            DiagnosticVerifier.VerifyCSharpDiagnostic(
@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M(IEnumerable<object> items)
    {
        if (items.Count() == 1) { }
        if (items.Count() != 1) { }

        if (1 == items.Count()) { }
        if (1 != items.Count()) { }

        if (items.Count() == i) { }
        if (i == items.Count()) { }

        if (items.Count() != i) { }
        if (i != items.Count()) { }
    }
}",
                new InvocationExpressionAnalyzer());
        }
    }
}
