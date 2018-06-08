// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;
using static Roslynator.CSharp.Refactorings.ExtractLinqToLocalFunction.ExtractLinqToLocalFunctionHelpers;

namespace Roslynator.CSharp.Refactorings.ExtractLinqToLocalFunction
{
    internal abstract class ExtractLinqToLocalFunctionRefactoring
    {
        public abstract string MethodName { get; }

        protected abstract ReturnStatementSyntax GetFirstReturnStatement(in ExtractLinqToLocalFunctionRefactoringContext context);

        protected abstract ReturnStatementSyntax GetLastReturnStatement(in ExtractLinqToLocalFunctionRefactoringContext context);

        protected abstract TypeSyntax GetReturnType(in ExtractLinqToLocalFunctionRefactoringContext context);

        protected virtual ExpressionSyntax GetCondition(in ExtractLinqToLocalFunctionRefactoringContext context, ExpressionSyntax expression)
        {
            return expression;
        }

        internal static void ComputeRefactorings(
            RefactoringContext context,
            in SimpleMemberInvocationExpressionInfo invocationInfo,
            SemanticModel semanticModel)
        {
            string methodName = invocationInfo.NameText;

            switch (methodName)
            {
                case "Any":
                case "All":
                case "FirstOrDefault":
                    break;
                default:
                    return;
            }

            if (invocationInfo.Arguments.Count != 1)
                return;

            ExpressionSyntax argumentExpression = invocationInfo.Arguments.SingleOrDefault(shouldThrow: false).Expression?.WalkDownParentheses();

            if (argumentExpression == null)
                return;

            InvocationExpressionSyntax invocationExpression = invocationInfo.InvocationExpression;

            SyntaxNode bodyOrExpressionBody = FindContainingBodyOrExpressionBody(invocationExpression);

            if (bodyOrExpressionBody == null)
                return;

            ExtensionMethodSymbolInfo extensionMethodSymbolInfo = semanticModel.GetReducedExtensionMethodInfo(invocationExpression, context.CancellationToken);

            if (extensionMethodSymbolInfo.Symbol == null)
                return;

            if (!SymbolUtility.IsLinqExtensionOfIEnumerableOfTWithPredicate(extensionMethodSymbolInfo.Symbol, methodName, allowImmutableArrayExtension: true))
                return;

            AnonymousFunctionAnalysis analysis = AnonymousFunctionAnalysis.Create(argumentExpression, semanticModel);

            if (analysis.Captured.IsDefaultOrEmpty)
                return;

            context.RegisterRefactoring(
                $"Extract '{methodName}' to local function",
                GetCreateChangedDocument(),
                RefactoringIdentifiers.ExtractLinqToLocalFunction);

            Func<CancellationToken, Task<Document>> GetCreateChangedDocument()
            {
                switch (methodName)
                {
                    case "Any":
                        return ct => ExtractLinqToLocalFunctionRefactorings.ExtractAny.RefactorAsync(context.Document, invocationExpression, bodyOrExpressionBody, extensionMethodSymbolInfo.ReducedSymbol, semanticModel, ct);
                    case "All":
                        return ct => ExtractLinqToLocalFunctionRefactorings.ExtractAll.RefactorAsync(context.Document, invocationExpression, bodyOrExpressionBody, extensionMethodSymbolInfo.ReducedSymbol, semanticModel, ct);
                    case "FirstOrDefault":
                        return ct => ExtractLinqToLocalFunctionRefactorings.ExtractFirstOrDefault.RefactorAsync(context.Document, invocationExpression, bodyOrExpressionBody, extensionMethodSymbolInfo.ReducedSymbol, semanticModel, ct);
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            SyntaxNode bodyOrExpressionBody,
            IMethodSymbol methodSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationExpression);

            ExpressionSyntax argumentExpression = invocationInfo
                .Arguments
                .Single()
                .Expression
                .WalkDownParentheses();

            AnonymousFunctionAnalysis analysis = AnonymousFunctionAnalysis.Create(argumentExpression, semanticModel);

            TypeSyntax elementType = methodSymbol
                .TypeArguments[0]
                .ToTypeSyntax()
                .WithSimplifierAnnotation();

            ParameterSyntax parameter = analysis.Parameter;

            string parameterName = parameter.Identifier.ValueText;

            string functionName = MethodName;

            LocalFunctionStatementSyntax childLocalFunction = null;

            SyntaxNode declaration = null;

            ArrowExpressionClauseSyntax expressionBody = null;

            int position = -1;

            if (bodyOrExpressionBody is BlockSyntax body)
            {
                position = body.Statements.Last().Span.End;
            }
            else
            {
                expressionBody = (ArrowExpressionClauseSyntax)bodyOrExpressionBody;

                position = expressionBody.Expression.Span.End;

                int offset = invocationExpression.FullSpan.Start - expressionBody.Expression.FullSpan.Start;

                (SyntaxNode node, BlockSyntax body) result = ExpandExpressionBodyRefactoring.Refactor(expressionBody, semanticModel, cancellationToken);

                declaration = result.node;
                body = result.body;

                var returnStatement = ((ReturnStatementSyntax)body.Statements.First());

                SyntaxToken returnKeyword = returnStatement.ReturnKeyword;

                ExpressionSyntax returnExpression = returnStatement.Expression;

                invocationExpression = (InvocationExpressionSyntax)returnStatement.FindNode(
                    new TextSpan(offset + returnKeyword.Span.Length + returnStatement.FullSpan.Start, invocationExpression.FullSpan.Length),
                    getInnermostNodeForTie: true);
            }

            var context = new ExtractLinqToLocalFunctionRefactoringContext(
                parameter,
                methodSymbol,
                elementType,
                semanticModel,
                cancellationToken);

            ExpressionSyntax condition = null;

            if (analysis.Body is BlockSyntax block)
            {
                functionName = NameGenerator.Default.EnsureUniqueLocalName(functionName, semanticModel, bodyOrExpressionBody.SpanStart, cancellationToken: cancellationToken);

                condition = InvocationExpression(
                    IdentifierName(functionName),
                    ArgumentList(Argument(IdentifierName(parameterName))));

                childLocalFunction = LocalFunctionStatement(
                    default,
                    GetReturnType(context),
                    Identifier(functionName),
                    ParameterList(Parameter(elementType, parameterName)),
                    block);

                childLocalFunction = childLocalFunction.WithFormatterAnnotation();

                functionName += "2";
            }
            else
            {
                condition = (ExpressionSyntax)analysis.Body;
            }

            ForEachStatementSyntax forEachStatement = ForEachStatement(
                elementType,
                Identifier(parameterName),
                invocationInfo.Expression,
                Block(
                    IfStatement(
                        GetCondition(context, condition),
                        Block(GetFirstReturnStatement(context)))));

            forEachStatement = forEachStatement.WithFormatterAnnotation();

            functionName = NameGenerator.Default.EnsureUniqueLocalName(functionName, semanticModel, position, cancellationToken: cancellationToken);

            (SeparatedSyntaxList<ParameterSyntax> parameters, SeparatedSyntaxList<ArgumentSyntax> arguments)
                = GetArgumentsAndParameters(analysis, semanticModel, position);

            LocalFunctionStatementSyntax newLocalFunction = LocalFunctionStatement(
                default,
                GetReturnType(context),
                Identifier(functionName).WithRenameAnnotation(),
                ParameterList(parameters),
                (childLocalFunction != null)
                    ? Block(forEachStatement, GetLastReturnStatement(context), childLocalFunction)
                    : Block(forEachStatement, GetLastReturnStatement(context)));

            newLocalFunction = newLocalFunction.WithFormatterAnnotation();

            BlockSyntax newBody = body
                .ReplaceNode(invocationExpression, InvocationExpression(IdentifierName(functionName), ArgumentList(arguments)))
                .AddStatements(newLocalFunction);

            if (declaration != null)
            {
                newBody = newBody.WithFormatterAnnotation();

                SyntaxNode newDeclaration = declaration.ReplaceNode(body, newBody);

                return document.ReplaceNodeAsync(bodyOrExpressionBody.Parent, newDeclaration, cancellationToken);
            }
            else
            {
                return document.ReplaceNodeAsync(body, newBody, cancellationToken);
            }
        }
    }

    //TODO: del
    //private static Task<Document> ReplaceAnyWithForEachAsync(
    //    Document document,
    //    InvocationExpressionSyntax invocationExpression,
    //    IMethodSymbol methodSymbol,
    //    CancellationToken cancellationToken)
    //{
    //    SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationExpression);

    //    ExpressionSyntax expression = invocationExpression.WalkUpParentheses();

    //    var variableDeclarator = (VariableDeclaratorSyntax)expression.Parent.Parent;

    //    var localDeclaration = (LocalDeclarationStatementSyntax)variableDeclarator.Parent.Parent;

    //    LocalDeclarationStatementSyntax newLocalDeclaration = localDeclaration
    //        .ReplaceNode(expression, FalseLiteralExpression())
    //        .WithTrailingTrivia(NewLine());

    //    string name = null;
    //    ExpressionSyntax condition = null;

    //    switch (invocationInfo.Arguments.Single().Expression.WalkDownParentheses())
    //    {
    //        case SimpleLambdaExpressionSyntax simpleLambda:
    //            {
    //                name = simpleLambda.Parameter.Identifier.ValueText;
    //                condition = (ExpressionSyntax)simpleLambda.Body;
    //                break;
    //            }
    //        case ParenthesizedLambdaExpressionSyntax parenthesizedLambda:
    //            {
    //                name = parenthesizedLambda.ParameterList.Parameters[0].Identifier.ValueText;
    //                condition = (ExpressionSyntax)parenthesizedLambda.Body;
    //                break;
    //            }
    //    }

    //    IfStatementSyntax ifStatement = IfStatement(
    //        condition,
    //        Block(
    //            SimpleAssignmentStatement(
    //                IdentifierName(variableDeclarator.Identifier.WithoutTrivia()),
    //                TrueLiteralExpression()),
    //            BreakStatement()));

    //    ForEachStatementSyntax forEachStatement = ForEachStatement(
    //        methodSymbol.TypeArguments[0].ToTypeSyntax().WithSimplifierAnnotation(),
    //        Identifier(name).WithRenameAnnotation(),
    //        invocationInfo.Expression,
    //        Block(ifStatement));

    //    forEachStatement = forEachStatement
    //        .WithTrailingTrivia(localDeclaration.GetTrailingTrivia())
    //        .WithFormatterAnnotation();

    //    return document.ReplaceNodeAsync(
    //        localDeclaration,
    //        new StatementSyntax[] { newLocalDeclaration, forEachStatement },
    //        cancellationToken);
    //}
}
