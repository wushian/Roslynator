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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConditionalExpressionCodeFixProvider))]
    [Shared]
    public class ConditionalExpressionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.AddNewLineBeforeOperatorOfMultilineConditionalExpression); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out ConditionalExpressionSyntax conditionalExpression))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.AddNewLineBeforeOperatorOfMultilineConditionalExpression:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Add new line before ?:",
                            ct => AddNewLineBeforeOperatorOfMultilineConditionalExpressionAsync(document, conditionalExpression, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }

        private static Task<Document> AddNewLineBeforeOperatorOfMultilineConditionalExpressionAsync(
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

            Write(condition, whenTrue, questionToken, "? ", builder);

            Write(whenTrue, whenFalse, colonToken, ": ", builder);

            builder.AppendTrailingTrivia();

            ExpressionSyntax newNode = SyntaxFactory.ParseExpression(StringBuilderCache.GetStringAndFree(sb));

            return document.ReplaceNodeAsync(conditionalExpression, newNode, cancellationToken);
        }

        private static void Write(
            ExpressionSyntax expression,
            ExpressionSyntax nextExpression,
            SyntaxToken token,
            string newText,
            SyntaxNodeTextBuilder builder)
        {
            if (AddNewLineBeforeOperatorOfMultilineConditionalExpressionAnalyzer.IsFixable(expression, token))
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
