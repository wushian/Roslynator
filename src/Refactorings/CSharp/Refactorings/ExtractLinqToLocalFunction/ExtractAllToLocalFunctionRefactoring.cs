// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.ExtractLinqToLocalFunction
{
    internal class ExtractAllToLocalFunctionRefactoring : ExtractLinqToLocalFunctionRefactoring
    {
        public override string MethodName
        {
            get { return "All"; }
        }

        protected override ExpressionSyntax GetCondition(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return Negator.LogicallyNegate(expression, semanticModel, cancellationToken);
        }

        protected override ReturnStatementSyntax GetFirstReturnStatement()
        {
            return ReturnStatement(FalseLiteralExpression());
        }

        protected override ReturnStatementSyntax GetLastReturnStatement()
        {
            return ReturnStatement(TrueLiteralExpression());
        }
    }
}
