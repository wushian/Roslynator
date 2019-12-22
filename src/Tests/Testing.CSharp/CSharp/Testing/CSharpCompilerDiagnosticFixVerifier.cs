// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.Testing;

namespace Roslynator.CSharp.Testing
{
    public abstract class CSharpCompilerDiagnosticFixVerifier : CompilerDiagnosticFixVerifier
    {
        protected CSharpCompilerDiagnosticFixVerifier() : base(CSharpWorkspaceFactory.Instance)
        {
        }

        public override CodeVerificationOptions Options => CSharpCodeVerificationOptions.Default;
    }
}
