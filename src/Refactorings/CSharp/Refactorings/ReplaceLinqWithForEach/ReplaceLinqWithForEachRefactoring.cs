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

                if (argumentExpression != null)
                {
                    InvocationExpressionSyntax invocationExpression = invocationInfo.InvocationExpression;

                    ExtensionMethodSymbolInfo extensionMethodSymbolInfo = semanticModel.GetReducedExtensionMethodInfo(invocationExpression, context.CancellationToken);

                    if (extensionMethodSymbolInfo.Symbol != null
                        && SymbolUtility.IsLinqExtensionOfIEnumerableOfTWithPredicate(extensionMethodSymbolInfo.Symbol, "Any", allowImmutableArrayExtension: true))
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

                                    AnonymousFunctionAnalysis analysis = AnonymousFunctionAnalysis.Create(argumentExpression, semanticModel);

                                    if (analysis.Captured.IsDefaultOrEmpty)
                                        break;

                                    if (!analysis.Body.IsKind(SyntaxKind.Block))
                                    {
                                        context.RegisterRefactoring(
                                            "Replace 'Any' with foreach",
                                            ct => ReplaceAnyWithForEachAsync(context.Document, invocationExpression, extensionMethodSymbolInfo.ReducedSymbol, ct),
                                            RefactoringIdentifiers.ReplaceLinqWithForEach);
                                    }

                                    context.RegisterRefactoring(
                                        "Replace 'Any' with local function",
                                        ct => ReplaceAnyWithLocalFunctionAsync(context.Document, invocationExpression, extensionMethodSymbolInfo.ReducedSymbol, semanticModel, ct),
                                        EquivalenceKey.Join(RefactoringIdentifiers.ReplaceLinqWithForEach, "LocalFunction"));

                                    break;
                                }
                        }
                    }
                }
            }
        }

        private static Task<Document> ReplaceAnyWithLocalFunctionAsync(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            IMethodSymbol methodSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationExpression);

            ExpressionSyntax expression = invocationExpression.WalkUpParentheses();

            var variableDeclarator = (VariableDeclaratorSyntax)expression.Parent.Parent;

            var localDeclaration = (LocalDeclarationStatementSyntax)variableDeclarator.Parent.Parent;

            ExpressionSyntax argumentExpression = invocationInfo.Arguments.Single().Expression.WalkDownParentheses();

            AnonymousFunctionAnalysis analysis = AnonymousFunctionAnalysis.Create(argumentExpression, semanticModel);

            TypeSyntax elementType = methodSymbol.TypeArguments[0].ToTypeSyntax().WithSimplifierAnnotation();

            string parameterName = analysis.Parameter.Identifier.ValueText;

            string functionName = "Any";

            LocalFunctionStatementSyntax localFunction2 = null;

            for (SyntaxNode parent = invocationExpression.Parent; parent != null; parent = parent.Parent)
            {
                switch (parent.Kind())
                {
                    case SyntaxKind.MethodDeclaration:
                        {
                            var methodDeclaration = (MethodDeclarationSyntax)parent;

                            ExpressionSyntax condition = null;

                            if (analysis.Body is BlockSyntax body)
                            {
                                functionName = NameGenerator.Default.EnsureUniqueLocalName(functionName, semanticModel, methodDeclaration.Body.SpanStart, cancellationToken: cancellationToken);

                                condition = InvocationExpression(
                                    IdentifierName(functionName),
                                    ArgumentList(Argument(IdentifierName(parameterName))));

                                localFunction2 = LocalFunctionStatement(
                                    default,
                                    CSharpTypeFactory.BoolType(),
                                    Identifier(functionName),
                                    ParameterList(Parameter(elementType, parameterName)),
                                    body);

                                localFunction2 = localFunction2.WithFormatterAnnotation();

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

                            functionName = NameGenerator.Default.EnsureUniqueLocalName(functionName, semanticModel, methodDeclaration.Body.SpanStart, cancellationToken: cancellationToken);

                            (SeparatedSyntaxList<ParameterSyntax> parameters, SeparatedSyntaxList<ArgumentSyntax> arguments)
                                = analysis.GetArgumentsAndParameters(semanticModel, methodDeclaration.Body.Statements.Last().FullSpan.End);

                            LocalFunctionStatementSyntax newLocalFunction = LocalFunctionStatement(
                                default,
                                CSharpTypeFactory.BoolType(),
                                Identifier(functionName).WithRenameAnnotation(),
                                ParameterList(parameters),
                                (localFunction2 != null)
                                    ? Block(forEachStatement, ReturnStatement(FalseLiteralExpression()), localFunction2)
                                    : Block(forEachStatement, ReturnStatement(FalseLiteralExpression())));

                            newLocalFunction = newLocalFunction
                                .WithTrailingTrivia(localDeclaration.GetTrailingTrivia())
                                .WithFormatterAnnotation();

                            LocalDeclarationStatementSyntax newLocalDeclaration = localDeclaration
                                .ReplaceNode(expression, InvocationExpression(IdentifierName(functionName), ArgumentList(arguments)));

                            MethodDeclarationSyntax newMethodDeclaration = methodDeclaration
                                .ReplaceNode(localDeclaration, newLocalDeclaration)
                                .AddBodyStatements(newLocalFunction);

                            return document.ReplaceNodeAsync(methodDeclaration, newMethodDeclaration, cancellationToken);
                        }
                    case SyntaxKind.LocalFunctionStatement:
                        {
                            throw new InvalidOperationException();
                        }
                }
            }

            throw new InvalidOperationException();
        }

        private static Task<Document> ReplaceAnyWithForEachAsync(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            IMethodSymbol methodSymbol,
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
            }

            IfStatementSyntax ifStatement = IfStatement(
                condition,
                Block(
                    SimpleAssignmentStatement(
                        IdentifierName(variableDeclarator.Identifier.WithoutTrivia()),
                        TrueLiteralExpression()),
                    BreakStatement()));

            ForEachStatementSyntax forEachStatement = ForEachStatement(
                methodSymbol.TypeArguments[0].ToTypeSyntax().WithSimplifierAnnotation(),
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
                {
                    return default;
                }

                SeparatedSyntaxList<ParameterSyntax> parameters = default;
                SeparatedSyntaxList<ArgumentSyntax> arguments = default;

                foreach (ISymbol symbol in Captured)
                {
                    IdentifierNameSyntax identifierName = IdentifierName(symbol.Name);

                    if (semanticModel
                        .GetSpeculativeSymbolInfo(position, identifierName, SpeculativeBindingOption.BindAsExpression)
                        .Symbol?
                        .Equals(symbol) != true)
                    {
                        if (symbol is ILocalSymbol localSymbol)
                        {
                            parameters = parameters.Add(Parameter(localSymbol.Type.ToTypeSyntax().WithSimplifierAnnotation(), localSymbol.Name));
                            arguments = arguments.Add(Argument(identifierName));
                        }
                        else if (symbol is IParameterSymbol parameterSymbol)
                        {
                            parameters = parameters.Add(Parameter(parameterSymbol.Type.ToTypeSyntax().WithSimplifierAnnotation(), parameterSymbol.Name));
                            arguments = arguments.Add(Argument(identifierName));
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }
                }

                return (parameters, arguments);
            }
        }
    }
}
