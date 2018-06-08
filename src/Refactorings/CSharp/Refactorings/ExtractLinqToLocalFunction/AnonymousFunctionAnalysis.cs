// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.ExtractLinqToLocalFunction
{
    internal readonly struct AnonymousFunctionAnalysis
    {
        private AnonymousFunctionAnalysis(SyntaxKind kind, SyntaxNode body, ImmutableArray<ISymbol> captured)
        {
            Kind = kind;
            Body = body;
            Captured = captured;

            Debug.Assert(Captured.All(f => f.IsKind(SymbolKind.Local, SymbolKind.Parameter)), Captured.FirstOrDefault(f => !f.IsKind(SymbolKind.Local, SymbolKind.Parameter))?.Kind.ToString());
        }

        public static AnonymousFunctionAnalysis Create(ExpressionSyntax expression, SemanticModel semanticModel)
        {
            switch (expression?.Kind())
            {
                case SyntaxKind.SimpleLambdaExpression:
                    {
                        var simpleLambda = (SimpleLambdaExpressionSyntax)expression;

                        return Create(SyntaxKind.SimpleLambdaExpression, simpleLambda.Body, semanticModel);
                    }
                case SyntaxKind.ParenthesizedLambdaExpression:
                    {
                        var parenthesizedLambda = (ParenthesizedLambdaExpressionSyntax)expression;

                        return Create(SyntaxKind.ParenthesizedLambdaExpression, parenthesizedLambda.Body, semanticModel);
                    }
                case SyntaxKind.AnonymousMethodExpression:
                    {
                        var anonymousMethod = (AnonymousMethodExpressionSyntax)expression;

                        return Create(SyntaxKind.AnonymousMethodExpression, anonymousMethod.Block, semanticModel);
                    }
            }

            return default;
        }

        private static AnonymousFunctionAnalysis Create(SyntaxKind kind, SyntaxNode body, SemanticModel semanticModel)
        {
            if (body == null)
                return default;

            return new AnonymousFunctionAnalysis(kind, body, semanticModel.AnalyzeDataFlow(body).Captured);
        }

        public SyntaxKind Kind { get; }

        public SyntaxNode Body { get; }

        public ImmutableArray<ISymbol> Captured { get; }

        public ParameterSyntax Parameter
        {
            get
            {
                switch (Kind)
                {
                    case SyntaxKind.SimpleLambdaExpression:
                        return ((SimpleLambdaExpressionSyntax)Body.Parent).Parameter;
                    case SyntaxKind.ParenthesizedLambdaExpression:
                        return ((ParenthesizedLambdaExpressionSyntax)Body.Parent).ParameterList?.Parameters.FirstOrDefault();
                    case SyntaxKind.AnonymousMethodExpression:
                        return ((AnonymousMethodExpressionSyntax)Body.Parent).ParameterList?.Parameters.FirstOrDefault();
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }
}
