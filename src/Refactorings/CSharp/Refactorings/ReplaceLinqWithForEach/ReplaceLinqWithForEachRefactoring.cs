// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.ReplaceLinqWithForEach
{
    internal static class ReplaceLinqWithForEachRefactoring
    {
        internal static void ComputeRefactorings(
            RefactoringContext context,
            in SimpleMemberInvocationExpressionInfo invocationInfo,
            SemanticModel semanticModel)
        {
            if (invocationInfo.NameText == "Any"
                && invocationInfo.Arguments.Count == 1)
            {
                ExpressionSyntax argumentExpression = invocationInfo.Arguments.SingleOrDefault(shouldThrow: false).Expression?.WalkDownParentheses();

                if (argumentExpression != null
                    && IsFixableArgumentExpression(argumentExpression))
                {
                    InvocationExpressionSyntax invocationExpression = invocationInfo.InvocationExpression;

                    IMethodSymbol methodSymbol = semanticModel.GetReducedExtensionMethodInfo(invocationExpression, context.CancellationToken).Symbol;

                    if (methodSymbol != null
                        && SymbolUtility.IsLinqExtensionOfIEnumerableOfTWithPredicate(methodSymbol, "Any", allowImmutableArrayExtension: true))
                    {
                        ExpressionSyntax expression = invocationExpression.WalkUpParentheses();

                        SyntaxNode parent = expression.Parent;

                        switch (parent.Kind())
                        {
                            case SyntaxKind.EqualsValueClause:
                                {
                                    parent = parent.Parent;

                                    if (!parent.IsKind(SyntaxKind.VariableDeclarator))
                                        break;

                                    parent = parent.Parent;

                                    if (!parent.IsKind(SyntaxKind.VariableDeclaration))
                                        break;

                                    parent = parent.Parent;

                                    if (!parent.IsKind(SyntaxKind.LocalDeclarationStatement))
                                        break;

                                    var localDeclaration = (LocalDeclarationStatementSyntax)parent;

                                    if (localDeclaration.IsEmbedded())
                                        break;

                                    context.RegisterRefactoring(
                                        "Replace 'Any' with foreach",
                                        ct => ReplaceAnyWithForEachAsync(context.Document, invocationExpression, semanticModel, ct),
                                        RefactoringIdentifiers.ReplaceLinqWithForEach);

                                    break;
                                }
                        }
                    }
                }
            }

            bool IsFixableArgumentExpression(ExpressionSyntax argumentExpression)
            {
                switch (argumentExpression.Kind())
                {
                    case SyntaxKind.SimpleLambdaExpression:
                        return ((SimpleLambdaExpressionSyntax)argumentExpression).Body is ExpressionSyntax;
                    case SyntaxKind.ParenthesizedLambdaExpression:
                        return ((ParenthesizedLambdaExpressionSyntax)argumentExpression).Body is ExpressionSyntax;
                    case SyntaxKind.IdentifierName:
                    case SyntaxKind.GenericName:
                    case SyntaxKind.SimpleMemberAccessExpression:
                        return true;
                }

                return false;
            }
        }

        private static Task<Document> ReplaceAnyWithForEachAsync(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationExpression);

            ExpressionSyntax expression = invocationExpression.WalkUpParentheses();

            var variableDeclarator = (VariableDeclaratorSyntax)expression.Parent.Parent;

            var localDeclaration = (LocalDeclarationStatementSyntax)variableDeclarator.Parent.Parent;

            LocalDeclarationStatementSyntax newLocalDeclaration = localDeclaration
                .ReplaceNode(expression, FalseLiteralExpression())
                .WithTrailingTrivia(NewLine());

            string name = null;
            ExpressionSyntax condition = null;

            ExpressionSyntax argumentExpression = invocationInfo.Arguments.Single().Expression.WalkDownParentheses();

            switch (argumentExpression)
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
                        name = NameGenerator.Default.EnsureUniqueLocalName(DefaultNames.ForEachVariable, semanticModel, localDeclaration.SpanStart, cancellationToken: cancellationToken);
                        condition = ParseExpression(argumentExpression.WithoutTrivia() + "()");
                        break;
                    }
            }

            IfStatementSyntax ifStatement = IfStatement(
                condition,
                Block(
                    SimpleAssignmentStatement(
                        IdentifierName(variableDeclarator.Identifier.WithoutTrivia()),
                        TrueLiteralExpression()),
                    BreakStatement()));

            ForEachStatementSyntax forEachStatement = ForEachStatement(
                VarType(),
                Identifier(name).WithRenameAnnotation(),
                invocationInfo.Expression,
                Block(ifStatement));

            forEachStatement = forEachStatement
                .WithTrailingTrivia(localDeclaration.GetTrailingTrivia())
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(
                localDeclaration,
                new StatementSyntax[] { newLocalDeclaration, forEachStatement },
                cancellationToken);
        }
    }
}
