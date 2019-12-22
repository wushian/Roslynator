﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.Tests;

namespace Roslynator.VisualBasic.Tests
{
    public abstract class VisualBasicCompilerDiagnosticFixVerifier : CompilerDiagnosticFixVerifier
    {
        public override CodeVerificationOptions Options => VisualBasicCodeVerificationOptions.Default;

        internal override WorkspaceFactory WorkspaceFactory => VisualBasicWorkspaceFactory.Instance;
    }
}
