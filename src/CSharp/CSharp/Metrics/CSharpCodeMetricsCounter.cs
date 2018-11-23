// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.Metrics;

namespace Roslynator.CSharp.Metrics
{
    public abstract class CSharpCodeMetricsCounter : CodeMetricsCounter
    {
        internal override SyntaxFactsService SyntaxFacts => CSharpSyntaxFactsService.Instance;
    }
}
