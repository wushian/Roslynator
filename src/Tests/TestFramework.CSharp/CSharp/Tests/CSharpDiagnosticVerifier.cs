﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.Tests;

namespace Roslynator.CSharp.Tests
{
    public abstract class CSharpDiagnosticVerifier : DiagnosticVerifier
    {
        public override CodeVerificationOptions Options => CSharpCodeVerificationOptions.Default;

        internal override WorkspaceFactory WorkspaceFactory => CSharpWorkspaceFactory.Instance;
    }
}
