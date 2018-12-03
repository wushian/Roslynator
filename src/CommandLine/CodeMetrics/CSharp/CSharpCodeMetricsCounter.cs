// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.CSharp;

namespace Roslynator.CodeMetrics.CSharp
{
    internal abstract class CSharpCodeMetricsCounter : CodeMetricsCounter
    {
        internal override ISyntaxFactsService SyntaxFacts => CSharpSyntaxFactsService.Instance;
    }
}
