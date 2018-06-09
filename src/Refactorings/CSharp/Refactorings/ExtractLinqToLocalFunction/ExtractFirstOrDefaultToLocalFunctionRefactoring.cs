// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.ExtractLinqToLocalFunction
{
    internal sealed class ExtractFirstOrDefaultToLocalFunctionRefactoring : ExtractLinqToLocalFunctionRefactoring
    {
        public static ExtractFirstOrDefaultToLocalFunctionRefactoring Instance { get; } = new ExtractFirstOrDefaultToLocalFunctionRefactoring();

        private ExtractFirstOrDefaultToLocalFunctionRefactoring()
        {
        }

        public override string MethodName
        {
            get { return "FirstOrDefault"; }
        }

        protected override ReturnStatementSyntax GetFirstReturnStatement(in ExtractLinqToLocalFunctionRefactoringContext context)
        {
            return ReturnStatement(IdentifierName(context.Parameter.Identifier.ValueText));
        }

        protected override ReturnStatementSyntax GetLastReturnStatement(in ExtractLinqToLocalFunctionRefactoringContext context)
        {
            ExpressionSyntax expression = context.ElementTypeSymbol.GetDefaultValueSyntax(context.ElementType);

            return ReturnStatement(expression);
        }

        protected override TypeSyntax GetReturnType(in ExtractLinqToLocalFunctionRefactoringContext context)
        {
            return context.ElementType;
        }
    }
}
