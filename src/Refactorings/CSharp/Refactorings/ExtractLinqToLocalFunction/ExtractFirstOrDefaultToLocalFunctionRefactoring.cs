// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.ExtractLinqToLocalFunction
{
    internal class ExtractFirstOrDefaultToLocalFunctionRefactoring : ExtractLinqToLocalFunctionRefactoring
    {
        public override string MethodName
        {
            get { throw new NotImplementedException(); }
        }

        protected override ReturnStatementSyntax GetFirstReturnStatement()
        {
            throw new NotImplementedException();
        }

        protected override ReturnStatementSyntax GetLastReturnStatement()
        {
            throw new NotImplementedException();
        }
    }
}
