// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    internal static class CodeFixHelpers
    {
        public static Task<Document> AppendEndOfLineAsync(
            Document document,
            SyntaxToken token,
            CancellationToken cancellationToken)
        {
            SyntaxToken newToken = token.AppendEndOfLineToTrailingTrivia();

            return document.ReplaceTokenAsync(token, newToken, cancellationToken);
        }

        public static Task<Document> AddNewLineBeforeAsync(
            Document document,
            SyntaxToken token,
            CancellationToken cancellationToken = default)
        {
            SyntaxTrivia indentation = token.Parent.GetIndentation(cancellationToken);

            return AddNewLineBeforeAsync(
                document,
                token,
                indentation.ToString(),
                cancellationToken);
        }

        public static Task<Document> AddNewLineBeforeAndIncreaseIndentationAsync(
            Document document,
            SyntaxToken token,
            CancellationToken cancellationToken = default)
        {
            return AddNewLineBeforeAndIncreaseIndentationAsync(
                document,
                token,
                token.Parent.GetIndentation(cancellationToken),
                cancellationToken);
        }

        public static Task<Document> AddNewLineBeforeAndIncreaseIndentationAsync(
            Document document,
            SyntaxToken token,
            SyntaxTrivia indentation,
            CancellationToken cancellationToken = default)
        {
            SyntaxTrivia singleIndentation = token.SyntaxTree.GetFirstIndentation(cancellationToken);

            return AddNewLineBeforeAsync(
                document,
                token,
                indentation.ToString() + singleIndentation.ToString(),
                cancellationToken);
        }

        public static Task<Document> AddNewLineBeforeAsync(
            Document document,
            SyntaxToken token,
            string indentation,
            CancellationToken cancellationToken = default)
        {
            var textChange = new TextChange(
                TextSpan.FromBounds(token.GetPreviousToken().Span.End, token.SpanStart),
                SyntaxTriviaAnalysis.GetEndOfLine(token).ToString() + indentation);

            return document.WithTextChangeAsync(textChange, cancellationToken);
        }

        public static (ExpressionSyntax left, SyntaxToken token, ExpressionSyntax right) PlaceTokenBeforeExpression(
            ExpressionSyntax left,
            SyntaxToken token,
            ExpressionSyntax right)
        {
            return (
                left.WithTrailingTrivia(token.TrailingTrivia),
                Token(
                    right.GetLeadingTrivia(),
                    token.Kind(),
                    TriviaList(Space)),
                right.WithoutLeadingTrivia());
        }

        public static (ExpressionSyntax left, SyntaxToken token, ExpressionSyntax right) PlaceTokenAfterExpression(
            ExpressionSyntax left,
            SyntaxToken token,
            ExpressionSyntax right)
        {
            return (
                left.WithTrailingTrivia(Space),
                Token(
                    SyntaxTriviaList.Empty,
                    token.Kind(),
                    left.GetTrailingTrivia()),
                right.WithLeadingTrivia(token.LeadingTrivia));
        }

        public static Task<Document> AddEmptyLineBeforeDirectiveAsync(
            Document document,
            DirectiveTriviaSyntax directiveTrivia,
            CancellationToken cancellationToken)
        {
            SyntaxTrivia parentTrivia = directiveTrivia.ParentTrivia;
            SyntaxToken token = parentTrivia.Token;
            SyntaxTriviaList leadingTrivia = token.LeadingTrivia;

            int index = leadingTrivia.IndexOf(parentTrivia);

            if (index > 0
                && leadingTrivia[index - 1].IsWhitespaceTrivia())
            {
                index--;
            }

            SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(index, SyntaxTriviaAnalysis.GetEndOfLine(token));

            SyntaxToken newToken = token.WithLeadingTrivia(newLeadingTrivia);

            return document.ReplaceTokenAsync(token, newToken, cancellationToken);
        }

        public static Task<Document> AddEmptyLineAfterDirectiveAsync(
            Document document,
            DirectiveTriviaSyntax directiveTrivia,
            CancellationToken cancellationToken)
        {
            SyntaxTrivia parentTrivia = directiveTrivia.ParentTrivia;
            SyntaxToken token = parentTrivia.Token;
            SyntaxTriviaList leadingTrivia = token.LeadingTrivia;

            int index = leadingTrivia.IndexOf(parentTrivia);

            SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(index + 1, SyntaxTriviaAnalysis.GetEndOfLine(token));

            SyntaxToken newToken = token.WithLeadingTrivia(newLeadingTrivia);

            return document.ReplaceTokenAsync(token, newToken, cancellationToken);
        }
    }
}
