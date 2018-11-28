// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Tests.VisualBasic
{
    public abstract class VisualBasicCodeFixVerifier : FixVerifier
    {
        public override CodeVerificationOptions Options => VisualBasicCodeVerificationOptions.Default;

        protected override WorkspaceFactory WorkspaceFactory => VisualBasicWorkspaceFactory.Instance;
    }
}
