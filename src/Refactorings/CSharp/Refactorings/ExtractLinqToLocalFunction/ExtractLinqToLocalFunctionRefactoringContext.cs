// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.ExtractLinqToLocalFunction
{
    internal readonly struct ExtractLinqToLocalFunctionRefactoringContext
    {
        public ExtractLinqToLocalFunctionRefactoringContext(
            ParameterSyntax parameter,
            IMethodSymbol methodSymbol,
            TypeSyntax elementType,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            Parameter = parameter;
            MethodSymbol = methodSymbol;
            ElementType = elementType;
            SemanticModel = semanticModel;
            CancellationToken = cancellationToken;
        }

        public ParameterSyntax Parameter { get; }
        public IMethodSymbol MethodSymbol { get; }
        public TypeSyntax ElementType { get; }
        public SemanticModel SemanticModel { get; }
        public CancellationToken CancellationToken { get; }

        public ITypeSymbol ElementTypeSymbol
        {
            get { return MethodSymbol.TypeArguments[0]; }
        }
    }
}
