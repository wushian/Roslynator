// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TokenCodeFixProvider))]
    [Shared]
    public class TokenCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.AddEmptyLineAfterClosingBraceOfBlock,
                    DiagnosticIdentifiers.PlaceConditionalOperatorBeforeExpression,
                    DiagnosticIdentifiers.PlaceConditionalOperatorAfterExpression);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindToken(root, context.Span.Start, out SyntaxToken token))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.AddEmptyLineAfterClosingBraceOfBlock:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Add empty line",
                            ct => AddEmptyLineAsync(document, token, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIdentifiers.PlaceConditionalOperatorBeforeExpression:
                    {
                        var conditionalExpression = (ConditionalExpressionSyntax)token.Parent;

                        string title = null;
                        if (token.IsKind(SyntaxKind.QuestionToken))
                        {
                            title = (SyntaxTriviaAnalysis.IsTokenPlacedAfterExpression(conditionalExpression.WhenTrue, conditionalExpression.ColonToken, conditionalExpression.WhenFalse))
                                ? "Place '?' and ':' before expression"
                                : "Place '?' before expression";
                        }
                        else
                        {
                            title = "Place ':' before expression";
                        }

                        CodeAction codeAction = CodeAction.Create(
                            title,
                            ct => PlaceConditionalOperatorBeforeExpressionAsync(document, conditionalExpression, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIdentifiers.PlaceConditionalOperatorAfterExpression:
                    {
                        var conditionalExpression = (ConditionalExpressionSyntax)token.Parent;

                        string title = null;
                        if (token.IsKind(SyntaxKind.QuestionToken))
                        {
                            title = (SyntaxTriviaAnalysis.IsTokenPlacedAfterExpression(conditionalExpression.WhenTrue, conditionalExpression.ColonToken, conditionalExpression.WhenFalse))
                                ? "Place '?' and ':' after expression"
                                : "Place '?' after expression";
                        }
                        else
                        {
                            title = "Place ':' after expression";
                        }

                        CodeAction codeAction = CodeAction.Create(
                            title,
                            ct => PlaceConditionalOperatorAfterExpressionAsync(document, conditionalExpression, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }

        private static Task<Document> AddEmptyLineAsync(
            Document document,
            SyntaxToken token,
            CancellationToken cancellationToken)
        {
            SyntaxToken newToken = token.AppendToTrailingTrivia(SyntaxTriviaAnalysis.FindEndOfLine(token));

            return document.ReplaceTokenAsync(token, newToken, cancellationToken);
        }

        private static Task<Document> PlaceConditionalOperatorBeforeExpressionAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax condition = conditionalExpression.Condition;
            ExpressionSyntax whenTrue = conditionalExpression.WhenTrue;
            ExpressionSyntax whenFalse = conditionalExpression.WhenFalse;
            SyntaxToken questionToken = conditionalExpression.QuestionToken;
            SyntaxToken colonToken = conditionalExpression.ColonToken;

            ExpressionSyntax newCondition = condition;
            ExpressionSyntax newWhenTrue = whenTrue;
            ExpressionSyntax newWhenFalse = whenFalse;
            SyntaxToken newQuestionToken = questionToken;
            SyntaxToken newColonToken = colonToken;

            if (SyntaxTriviaAnalysis.IsTokenPlacedAfterExpression(condition, questionToken, whenTrue))
            {
                var (left, token, right) = SyntaxTriviaManipulation.PlaceTokenBeforeExpression(condition, questionToken, whenTrue);

                newCondition = left;
                newQuestionToken = token;
                newWhenTrue = right;
            }

            if (SyntaxTriviaAnalysis.IsTokenPlacedAfterExpression(whenTrue, colonToken, whenFalse))
            {
                var (left, token, right) = SyntaxTriviaManipulation.PlaceTokenBeforeExpression(newWhenTrue, colonToken, whenFalse);

                newWhenTrue = left;
                newColonToken = token;
                newWhenFalse = right;
            }

            ConditionalExpressionSyntax newConditionalExpression = ConditionalExpression(
                newCondition,
                newQuestionToken,
                newWhenTrue,
                newColonToken,
                newWhenFalse);

            return document.ReplaceNodeAsync(conditionalExpression, newConditionalExpression, cancellationToken);
        }

        private static Task<Document> PlaceConditionalOperatorAfterExpressionAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax condition = conditionalExpression.Condition;
            ExpressionSyntax whenTrue = conditionalExpression.WhenTrue;
            ExpressionSyntax whenFalse = conditionalExpression.WhenFalse;
            SyntaxToken questionToken = conditionalExpression.QuestionToken;
            SyntaxToken colonToken = conditionalExpression.ColonToken;

            ExpressionSyntax newCondition = condition;
            ExpressionSyntax newWhenTrue = whenTrue;
            ExpressionSyntax newWhenFalse = whenFalse;
            SyntaxToken newQuestionToken = questionToken;
            SyntaxToken newColonToken = colonToken;

            if (SyntaxTriviaAnalysis.IsTokenPlacedBeforeExpression(condition, questionToken, whenTrue))
            {
                var (left, token, right) = SyntaxTriviaManipulation.PlaceTokenAfterExpression(condition, questionToken, whenTrue);

                newCondition = left;
                newQuestionToken = token;
                newWhenTrue = right;
            }

            if (SyntaxTriviaAnalysis.IsTokenPlacedBeforeExpression(whenTrue, colonToken, whenFalse))
            {
                var (left, token, right) = SyntaxTriviaManipulation.PlaceTokenAfterExpression(newWhenTrue, colonToken, whenFalse);

                newWhenTrue = left;
                newColonToken = token;
                newWhenFalse = right;
            }

            ConditionalExpressionSyntax newConditionalExpression = ConditionalExpression(
                newCondition,
                newQuestionToken,
                newWhenTrue,
                newColonToken,
                newWhenFalse);

            return document.ReplaceNodeAsync(conditionalExpression, newConditionalExpression, cancellationToken);
        }
    }
}
