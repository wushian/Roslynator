// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.ExtractLinqToLocalFunction
{
    internal static class ExtractLinqToLocalFunctionHelpers
    {
        public static SyntaxNode FindContainingBodyOrExpressionBody(SyntaxNode node)
        {
            for (SyntaxNode parent = node.Parent; parent != null; parent = parent.Parent)
            {
                switch (parent.Kind())
                {
                    case SyntaxKind.Block:
                    case SyntaxKind.ArrowExpressionClause:
                        {
                            switch (parent.Parent.Kind())
                            {
                                case SyntaxKind.MethodDeclaration:
                                case SyntaxKind.OperatorDeclaration:
                                case SyntaxKind.ConversionOperatorDeclaration:
                                case SyntaxKind.ConstructorDeclaration:
                                case SyntaxKind.DestructorDeclaration:
                                case SyntaxKind.PropertyDeclaration:
                                case SyntaxKind.EventDeclaration:
                                case SyntaxKind.IndexerDeclaration:
                                case SyntaxKind.GetAccessorDeclaration:
                                case SyntaxKind.SetAccessorDeclaration:
                                case SyntaxKind.AddAccessorDeclaration:
                                case SyntaxKind.RemoveAccessorDeclaration:
                                case SyntaxKind.UnknownAccessorDeclaration:
                                case SyntaxKind.LocalFunctionStatement:
                                    return parent;
                            }

                            Debug.Assert(!parent.IsKind(SyntaxKind.ArrowExpressionClause));
                            Debug.Assert(!(parent.Parent is MemberDeclarationSyntax), parent.Parent.Kind().ToString());
                            break;
                        }
                    case SyntaxKind.FieldDeclaration:
                        {
                            return null;
                        }
                    default:
                        {
                            Debug.Assert(!(parent is MemberDeclarationSyntax), parent.Kind().ToString());
                            break;
                        }
                }
            }

            return null;
        }

        public static (SeparatedSyntaxList<ParameterSyntax> parameters, SeparatedSyntaxList<ArgumentSyntax> arguments) GetArgumentsAndParameters(
            in AnonymousFunctionAnalysis analysis,
            SemanticModel semanticModel,
            int position)
        {
            if (analysis.Captured.IsDefault)
                return default;

            SeparatedSyntaxList<ParameterSyntax> parameters = default;
            SeparatedSyntaxList<ArgumentSyntax> arguments = default;

            foreach (ISymbol symbol in analysis.Captured)
            {
                if (symbol is ILocalSymbol localSymbol)
                {
                    IdentifierNameSyntax identifierName = IdentifierName(symbol.Name);

                    if (semanticModel
                        .GetSpeculativeSymbolInfo(position, identifierName, SpeculativeBindingOption.BindAsExpression)
                        .Symbol?
                        .Equals(symbol) != true)
                    {
                        parameters = parameters.Add(Parameter(localSymbol.Type.ToTypeSyntax().WithSimplifierAnnotation(), localSymbol.Name));
                        arguments = arguments.Add(Argument(identifierName));
                    }
                }
                else if (symbol is IParameterSymbol parameterSymbol)
                {
                    if (!semanticModel.IsAccessible(position, parameterSymbol))
                    {
                        parameters = parameters.Add(Parameter(parameterSymbol.Type.ToTypeSyntax().WithSimplifierAnnotation(), parameterSymbol.Name));
                        arguments = arguments.Add(Argument(IdentifierName(parameterSymbol.Name)));
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            return (parameters, arguments);
        }
    }
}
