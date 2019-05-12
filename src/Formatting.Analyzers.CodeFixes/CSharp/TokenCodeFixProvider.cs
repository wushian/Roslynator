// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;
using Roslynator.Text;

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
                    DiagnosticIdentifiers.PlaceConditionalOperatorBeforeExpression);
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
                            ct => AddEmptyLineAfterClosingBraceOfBlockAsync(document, token, ct),
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
                            title = (PlaceConditionalOperatorBeforeExpressionAnalyzer.IsFixable(conditionalExpression.WhenTrue, conditionalExpression.ColonToken))
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
            }
        }

        private static Task<Document> AddEmptyLineAfterClosingBraceOfBlockAsync(
            Document document,
            SyntaxToken token,
            CancellationToken cancellationToken)
        {
            SyntaxToken newToken = token.AppendToTrailingTrivia(CSharpFactory.NewLine());

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

            StringBuilder sb = StringBuilderCache.GetInstance();

            var builder = new SyntaxNodeTextBuilder(conditionalExpression, sb);

            builder.AppendLeadingTrivia();
            builder.AppendSpan(condition);

            Write(condition, whenTrue, questionToken, "? ");

            Write(whenTrue, whenFalse, colonToken, ": ");

            builder.AppendTrailingTrivia();

            ExpressionSyntax newNode = SyntaxFactory.ParseExpression(StringBuilderCache.GetStringAndFree(sb));

            return document.ReplaceNodeAsync(conditionalExpression, newNode, cancellationToken);

            void Write(
                ExpressionSyntax expression,
                ExpressionSyntax nextExpression,
                SyntaxToken token,
                string newText)
            {
                if (PlaceConditionalOperatorBeforeExpressionAnalyzer.IsFixable(expression, token))
                {
                    if (!expression.GetTrailingTrivia().IsEmptyOrWhitespace()
                        || !token.LeadingTrivia.IsEmptyOrWhitespace())
                    {
                        builder.AppendTrailingTrivia(expression);
                        builder.AppendLeadingTrivia(token);
                    }

                    builder.AppendTrailingTrivia(token);
                    builder.AppendLeadingTrivia(nextExpression);
                    builder.Append(newText);
                    builder.AppendSpan(nextExpression);
                }
                else
                {
                    builder.AppendTrailingTrivia(expression);
                    builder.AppendFullSpan(token);
                    builder.AppendLeadingTriviaAndSpan(nextExpression);
                }
            }
        }
    }
}
