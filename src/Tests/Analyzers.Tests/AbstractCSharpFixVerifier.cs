// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.CSharp.Tests;
using Roslynator.Tests;

namespace Roslynator.CSharp.Analysis.Tests
{
    public abstract class AbstractCSharpFixVerifier : CSharpFixVerifier
    {
        protected override Assert Assert => XunitAssert.Instance;
    }
}
