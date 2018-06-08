// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.ExtractLinqToLocalFunction
{
    internal static class ExtractLinqToLocalFunctionRefactoring
    {
        internal static void ComputeRefactorings(
            RefactoringContext context,
            in SimpleMemberInvocationExpressionInfo invocationInfo,
            SemanticModel semanticModel)
        {
            if (invocationInfo.NameText != "Any")
                return;

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

            if (!SymbolUtility.IsLinqExtensionOfIEnumerableOfTWithPredicate(extensionMethodSymbolInfo.Symbol, "Any", allowImmutableArrayExtension: true))
                return;

            AnonymousFunctionAnalysis analysis = AnonymousFunctionAnalysis.Create(argumentExpression, semanticModel);

            if (analysis.Captured.IsDefaultOrEmpty)
                return;

            context.RegisterRefactoring(
                "Extract 'Any' to local function",
                ct => RefactorAsync(context.Document, invocationExpression, bodyOrExpressionBody, extensionMethodSymbolInfo.ReducedSymbol, semanticModel, ct),
                RefactoringIdentifiers.ExtractLinqToLocalFunction);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            SyntaxNode bodyOrExpressionBody,
            IMethodSymbol methodSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationExpression);

            ExpressionSyntax argumentExpression = invocationInfo.Arguments.Single().Expression.WalkDownParentheses();

            AnonymousFunctionAnalysis analysis = AnonymousFunctionAnalysis.Create(argumentExpression, semanticModel);

            TypeSyntax elementType = methodSymbol.TypeArguments[0].ToTypeSyntax().WithSimplifierAnnotation();

            string parameterName = analysis.Parameter.Identifier.ValueText;

            string functionName = "Any";

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

                declaration = ExpandExpressionBodyRefactoring.Refactor(expressionBody, semanticModel, cancellationToken);

                body = GetBody(declaration);

                var returnStatement = ((ReturnStatementSyntax)body.Statements.First());

                SyntaxToken returnKeyword = returnStatement.ReturnKeyword;

                ExpressionSyntax returnExpression = returnStatement.Expression;

                invocationExpression = (InvocationExpressionSyntax)returnStatement.FindNode(
                    new TextSpan(offset + returnKeyword.Span.Length + returnStatement.FullSpan.Start, invocationExpression.FullSpan.Length),
                    getInnermostNodeForTie: true);
            }

            ExpressionSyntax condition = null;

            if (analysis.Body is BlockSyntax block)
            {
                functionName = NameGenerator.Default.EnsureUniqueLocalName(functionName, semanticModel, bodyOrExpressionBody.SpanStart, cancellationToken: cancellationToken);

                condition = InvocationExpression(
                    IdentifierName(functionName),
                    ArgumentList(Argument(IdentifierName(parameterName))));

                childLocalFunction = LocalFunctionStatement(
                    default,
                    CSharpTypeFactory.BoolType(),
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
                        condition,
                        Block(ReturnStatement(TrueLiteralExpression())))));

            forEachStatement = forEachStatement.WithFormatterAnnotation();

            functionName = NameGenerator.Default.EnsureUniqueLocalName(functionName, semanticModel, position, cancellationToken: cancellationToken);

            (SeparatedSyntaxList<ParameterSyntax> parameters, SeparatedSyntaxList<ArgumentSyntax> arguments)
                = analysis.GetArgumentsAndParameters(semanticModel, position);

            LocalFunctionStatementSyntax newLocalFunction = LocalFunctionStatement(
                default,
                CSharpTypeFactory.BoolType(),
                Identifier(functionName).WithRenameAnnotation(),
                ParameterList(parameters),
                (childLocalFunction != null)
                    ? Block(forEachStatement, ReturnStatement(FalseLiteralExpression()), childLocalFunction)
                    : Block(forEachStatement, ReturnStatement(FalseLiteralExpression())));

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

        private static SyntaxNode FindContainingBodyOrExpressionBody(InvocationExpressionSyntax invocationExpression)
        {
            for (SyntaxNode node = invocationExpression.Parent; node != null; node = node.Parent)
            {
                switch (node.Kind())
                {
                    case SyntaxKind.Block:
                    case SyntaxKind.ArrowExpressionClause:
                        {
                            SyntaxNode parent = node.Parent;

                            switch (parent.Kind())
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
                                    return node;
                            }

                            Debug.Assert(!node.IsKind(SyntaxKind.ArrowExpressionClause));
                            Debug.Assert(!(parent is MemberDeclarationSyntax), parent.Kind().ToString());
                            break;
                        }
                    case SyntaxKind.FieldDeclaration:
                        {
                            return null;
                        }
                    default:
                        {
                            Debug.Assert(!(node is MemberDeclarationSyntax), node.Kind().ToString());
                            break;
                        }
                }
            }

            return null;
        }

        private static BlockSyntax GetBody(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).Body;
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)node).Body;
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)node).Body;
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)node).Body;
                case SyntaxKind.DestructorDeclaration:
                    return ((DestructorDeclarationSyntax)node).Body;
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)node).AccessorList.Accessors.First().Body;
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)node).AccessorList.Accessors.First().Body;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)node).AccessorList.Accessors.First().Body;
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return ((AccessorDeclarationSyntax)node).Body;
                case SyntaxKind.LocalFunctionStatement:
                    return ((LocalFunctionStatementSyntax)node).Body;
            }

            throw new InvalidOperationException();
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

        private readonly struct AnonymousFunctionAnalysis
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
                            return null;
                    }
                }
            }

            public (SeparatedSyntaxList<ParameterSyntax> parameters, SeparatedSyntaxList<ArgumentSyntax> arguments) GetArgumentsAndParameters(
                SemanticModel semanticModel,
                int position)
            {
                if (Captured.IsDefault)
                    return default;

                SeparatedSyntaxList<ParameterSyntax> parameters = default;
                SeparatedSyntaxList<ArgumentSyntax> arguments = default;

                foreach (ISymbol symbol in Captured)
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
}
