// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
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
using static Roslynator.CSharp.Refactorings.ExtractLinqToLocalFunction.ExtractLinqToLocalFunctionHelpers;

namespace Roslynator.CSharp.Refactorings.ExtractLinqToLocalFunction
{
    internal abstract class ExtractLinqToLocalFunctionRefactoring
    {
        private TypeSyntax _elementType;

        protected ExtractLinqToLocalFunctionRefactoring(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            SyntaxNode body,
            SyntaxNode containingBody,
            ITypeSymbol elementTypeSymbol,
            ImmutableArray<ISymbol> capturedSymbols,
            SemanticModel semanticModel)
        {
            Document = document;
            InvocationExpression = invocationExpression;
            Body = body;
            ContainingBody = containingBody;
            ElementTypeSymbol = elementTypeSymbol;
            CapturedSymbols = capturedSymbols;
            SemanticModel = semanticModel;
        }

        public Document Document { get; }

        public InvocationExpressionSyntax InvocationExpression { get; private set; }

        public SyntaxNode Body { get; }

        public SyntaxNode ContainingBody { get; }

        public ITypeSymbol ElementTypeSymbol { get; }

        public ImmutableArray<ISymbol> CapturedSymbols { get; }

        public TypeSyntax ElementType
        {
            get
            {
                if (_elementType == null)
                {
                    if (ElementTypeSymbol.SupportsExplicitDeclaration())
                    {
                        _elementType = ElementTypeSymbol.ToTypeSyntax().WithSimplifierAnnotation();
                    }
                    else
                    {
                        _elementType = VarType();
                    }
                }

                return _elementType;
            }
        }

        public ParameterSyntax Parameter
        {
            get
            {
                switch (Body.Parent.Kind())
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

        public SemanticModel SemanticModel { get; }

        public abstract string MethodName { get; }

        protected abstract ReturnStatementSyntax GetFirstReturnStatement();

        protected abstract ReturnStatementSyntax GetLastReturnStatement();

        protected abstract TypeSyntax GetReturnType();

        protected virtual ExpressionSyntax GetCondition(ExpressionSyntax expression)
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

            SyntaxNode containingBody = FindContainingBodyOrExpressionBody(invocationExpression);

            if (containingBody == null)
                return;

            ExtensionMethodSymbolInfo extensionMethodSymbolInfo = semanticModel.GetReducedExtensionMethodInfo(invocationExpression, context.CancellationToken);

            IMethodSymbol reducedSymbol = extensionMethodSymbolInfo.ReducedSymbol;

            if (reducedSymbol == null)
                return;

            ITypeSymbol typeArgument = reducedSymbol.TypeArguments[0];

            bool? supportsExplicitDeclaration = null;

            if (methodName == "FirstOrDefault")
            {
                supportsExplicitDeclaration = typeArgument.SupportsExplicitDeclaration();

                if (supportsExplicitDeclaration == false)
                    return;
            }

            if (!SymbolUtility.IsLinqExtensionOfIEnumerableOfTWithPredicate(extensionMethodSymbolInfo.Symbol, methodName, allowImmutableArrayExtension: true))
                return;

            AnonymousFunctionAnalysis analysis = AnonymousFunctionAnalysis.Create(argumentExpression, semanticModel);

            if (analysis.Body.IsKind(SyntaxKind.Block))
            {
                supportsExplicitDeclaration = supportsExplicitDeclaration ?? typeArgument.SupportsExplicitDeclaration();

                if (supportsExplicitDeclaration == false)
                    return;
            }

            if (analysis.Captured.IsDefaultOrEmpty)
                return;

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceLinqWithForEach)
                && !analysis.Body.IsKind(SyntaxKind.Block)
                && analysis.IsLambda
                && invocationExpression.WalkUpParentheses().IsParentKind(SyntaxKind.ReturnStatement, SyntaxKind.ArrowExpressionClause))
            {
                context.RegisterRefactoring(
                    $"Replace '{methodName}' with foreach",
                    ct => ReplaceLinqWithForEachRefactoring.RefactorAsync(context.Document, invocationExpression, reducedSymbol, semanticModel, ct),
                    RefactoringIdentifiers.ReplaceLinqWithForEach);
            }
            else if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExtractLinqToLocalFunction))
            {
                context.RegisterRefactoring(
                    $"Extract '{methodName}' to local function",
                    ct => CreateRefactoring().RefactorAsync(ct),
                    RefactoringIdentifiers.ExtractLinqToLocalFunction);
            }

            ExtractLinqToLocalFunctionRefactoring CreateRefactoring()
            {
                switch (methodName)
                {
                    case "Any":
                        return new ExtractAnyToLocalFunctionRefactoring(context.Document, invocationExpression, analysis.Body, containingBody, typeArgument, analysis.Captured, semanticModel);
                    case "All":
                        return new ExtractAllToLocalFunctionRefactoring(context.Document, invocationExpression, analysis.Body, containingBody, typeArgument, analysis.Captured, semanticModel);
                    case "FirstOrDefault":
                        return new ExtractFirstOrDefaultToLocalFunctionRefactoring(context.Document, invocationExpression, analysis.Body, containingBody, typeArgument, analysis.Captured, semanticModel);
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public Task<Document> RefactorAsync(CancellationToken cancellationToken)
        {
            SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(InvocationExpression);

            ExpressionSyntax argumentExpression = invocationInfo
                .Arguments
                .Single()
                .Expression
                .WalkDownParentheses();

            AnonymousFunctionAnalysis analysis = AnonymousFunctionAnalysis.Create(argumentExpression, SemanticModel);

            TypeSyntax elementType;
            if (ElementTypeSymbol.SupportsExplicitDeclaration())
            {
                elementType = ElementTypeSymbol.ToTypeSyntax().WithSimplifierAnnotation();
            }
            else
            {
                elementType = VarType();
            }

            string parameterName = Parameter.Identifier.ValueText;

            string functionName = MethodName;

            LocalFunctionStatementSyntax childLocalFunction = null;

            SyntaxNode declaration = null;

            ArrowExpressionClauseSyntax expressionBody = null;

            int position = -1;

            if (ContainingBody is BlockSyntax body)
            {
                position = body.Statements.Last().Span.End;
            }
            else
            {
                expressionBody = (ArrowExpressionClauseSyntax)ContainingBody;

                position = expressionBody.Expression.Span.End;

                int offset = InvocationExpression.FullSpan.Start - expressionBody.Expression.FullSpan.Start;

                (SyntaxNode node, BlockSyntax body) result = ExpandExpressionBodyRefactoring.Refactor(expressionBody, SemanticModel, cancellationToken);

                declaration = result.node;
                body = result.body;

                var returnStatement = ((ReturnStatementSyntax)body.Statements.First());

                SyntaxToken returnKeyword = returnStatement.ReturnKeyword;

                ExpressionSyntax returnExpression = returnStatement.Expression;

                InvocationExpression = (InvocationExpressionSyntax)returnStatement.FindNode(
                    new TextSpan(offset + returnKeyword.Span.Length + returnStatement.FullSpan.Start, InvocationExpression.FullSpan.Length),
                    getInnermostNodeForTie: true);
            }

            ExpressionSyntax condition = null;

            if (analysis.Body is BlockSyntax block)
            {
                functionName = NameGenerator.Default.EnsureUniqueLocalName(functionName, SemanticModel, ContainingBody.SpanStart, cancellationToken: cancellationToken);

                condition = InvocationExpression(
                    IdentifierName(functionName),
                    ArgumentList(Argument(IdentifierName(parameterName))));

                childLocalFunction = LocalFunctionStatement(
                    default,
                    GetReturnType(),
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
                        GetCondition(condition),
                        Block(GetFirstReturnStatement()))));

            forEachStatement = forEachStatement.WithFormatterAnnotation();

            functionName = NameGenerator.Default.EnsureUniqueLocalName(functionName, SemanticModel, position, cancellationToken: cancellationToken);

            (SeparatedSyntaxList<ParameterSyntax> parameters, SeparatedSyntaxList<ArgumentSyntax> arguments)
                = GetArgumentsAndParameters(analysis, SemanticModel, position);

            LocalFunctionStatementSyntax newLocalFunction = LocalFunctionStatement(
                default,
                GetReturnType(),
                Identifier(functionName).WithRenameAnnotation(),
                ParameterList(parameters),
                (childLocalFunction != null)
                    ? Block(forEachStatement, GetLastReturnStatement(), childLocalFunction)
                    : Block(forEachStatement, GetLastReturnStatement()));

            newLocalFunction = newLocalFunction.WithFormatterAnnotation();

            BlockSyntax newBody = body
                .ReplaceNode(InvocationExpression, InvocationExpression(IdentifierName(functionName), ArgumentList(arguments)))
                .AddStatements(newLocalFunction);

            if (declaration != null)
            {
                newBody = newBody.WithFormatterAnnotation();

                SyntaxNode newDeclaration = declaration.ReplaceNode(body, newBody);

                return Document.ReplaceNodeAsync(ContainingBody.Parent, newDeclaration, cancellationToken);
            }
            else
            {
                return Document.ReplaceNodeAsync(body, newBody, cancellationToken);
            }
        }
    }
}
