// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceLinqWithForEachRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            IMethodSymbol methodSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationExpression);

            string name = null;
            ExpressionSyntax condition = null;

            switch (invocationInfo.Arguments.Single().Expression.WalkDownParentheses())
            {
                case SimpleLambdaExpressionSyntax simpleLambda:
                    {
                        name = simpleLambda.Parameter.Identifier.ValueText;
                        condition = (ExpressionSyntax)simpleLambda.Body;
                        break;
                    }
                case ParenthesizedLambdaExpressionSyntax parenthesizedLambda:
                    {
                        name = parenthesizedLambda.ParameterList.Parameters[0].Identifier.ValueText;
                        condition = (ExpressionSyntax)parenthesizedLambda.Body;
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException();
                    }
            }

            if (methodSymbol.Name == "All")
                condition = Negator.LogicallyNegate(condition, semanticModel, cancellationToken);

            IfStatementSyntax ifStatement = IfStatement(
                condition,
                Block(ReturnStatement(GetFirstReturnExpression())));

            ITypeSymbol typeArgument = methodSymbol.TypeArguments[0];

            TypeSyntax elementType = (typeArgument.SupportsExplicitDeclaration())
                    ? typeArgument.ToTypeSyntax().WithSimplifierAnnotation()
                    : VarType();

            ForEachStatementSyntax forEachStatement = ForEachStatement(
                elementType,
                Identifier(name).WithRenameAnnotation(),
                invocationInfo.Expression,
                Block(ifStatement));

            forEachStatement = forEachStatement.WithFormatterAnnotation();

            SyntaxNode parent = invocationExpression.WalkUpParentheses().Parent;

            ReturnStatementSyntax lastReturnStatement = ReturnStatement(GetLastReturnExpression());

            if (parent is ReturnStatementSyntax returnStatement)
            {
                forEachStatement = forEachStatement.WithLeadingTrivia(returnStatement.GetLeadingTrivia());
                lastReturnStatement = lastReturnStatement.WithTrailingTrivia(returnStatement.GetTrailingTrivia());

                return document.ReplaceNodeAsync(
                    returnStatement,
                    new StatementSyntax[] { forEachStatement, lastReturnStatement },
                    cancellationToken);
            }
            else
            {
                var expressionBody = (ArrowExpressionClauseSyntax)parent;

                (SyntaxNode node, BlockSyntax body) = ExpandExpressionBodyRefactoring.Refactor(expressionBody, semanticModel, cancellationToken);

                returnStatement = (ReturnStatementSyntax)body.Statements.Single();

                BlockSyntax newBody = body
                    .ReplaceNode(returnStatement, new StatementSyntax[] { forEachStatement, lastReturnStatement })
                    .WithFormatterAnnotation();

                SyntaxNode newNode = node.ReplaceNode(body, newBody);

                return document.ReplaceNodeAsync(expressionBody.Parent, newNode, cancellationToken);
            }

            ExpressionSyntax GetFirstReturnExpression()
            {
                switch (methodSymbol.Name)
                {
                    case "Any":
                        return TrueLiteralExpression();
                    case "All":
                        return FalseLiteralExpression();
                    case "FirstOrDefault":
                        return IdentifierName(name);
                    default:
                        throw new InvalidOperationException();
                }
            }

            ExpressionSyntax GetLastReturnExpression()
            {
                switch (methodSymbol.Name)
                {
                    case "Any":
                        return FalseLiteralExpression();
                    case "All":
                        return TrueLiteralExpression();
                    case "FirstOrDefault":
                        return typeArgument.GetDefaultValueSyntax(elementType);
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }
}
