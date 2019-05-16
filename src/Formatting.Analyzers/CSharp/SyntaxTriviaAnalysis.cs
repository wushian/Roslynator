// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp
{
    internal static class SyntaxTriviaAnalysis
    {
        public static bool IsEmptyOrSingleWhitespaceTrivia(SyntaxTriviaList triviaList)
        {
            int count = triviaList.Count;

            return count == 0
                || (count == 1 && triviaList[0].IsWhitespaceTrivia());
        }

        public static SyntaxTrivia GetEndOfLine(SyntaxNode node)
        {
            return GetEndOfLine(node.GetFirstToken());
        }

        public static SyntaxTrivia GetEndOfLine(SyntaxToken token)
        {
            SyntaxTrivia trivia = FindEndOfLine(token);

            return (trivia.IsEndOfLineTrivia()) ? trivia : CSharpFactory.NewLine();
        }

        public static SyntaxTrivia FindEndOfLine(SyntaxNode node)
        {
            return FindEndOfLine(node.GetFirstToken());
        }

        public static SyntaxTrivia FindEndOfLine(SyntaxToken token)
        {
            SyntaxToken t = token;

            do
            {
                foreach (SyntaxTrivia trivia in t.LeadingTrivia)
                {
                    if (trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                        return trivia;
                }

                foreach (SyntaxTrivia trivia in t.TrailingTrivia)
                {
                    if (trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                        return trivia;
                }

                t = t.GetNextToken();
            }
            while (!t.IsKind(SyntaxKind.None));

            t = token;

            while (true)
            {
                t = t.GetPreviousToken();

                if (t.IsKind(SyntaxKind.None))
                    break;

                foreach (SyntaxTrivia trivia in t.LeadingTrivia)
                {
                    if (trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                        return trivia;
                }

                foreach (SyntaxTrivia trivia in t.TrailingTrivia)
                {
                    if (trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                        return trivia;
                }
            }

            return default;
        }

        public static bool IsTokenPlacedBeforeExpression(
            ExpressionSyntax left,
            SyntaxToken token,
            ExpressionSyntax right)
        {
            if (!IsOptionalWhitespaceTriviaFollowedWithEndOfLineTrivia(left.GetTrailingTrivia()))
                return false;

            if (!token.LeadingTrivia.IsEmptyOrWhitespace())
                return false;

            if (!token.TrailingTrivia.SingleOrDefault(shouldThrow: false).IsKind(SyntaxKind.WhitespaceTrivia))
                return false;

            if (right.GetLeadingTrivia().Any())
                return false;

            return true;
        }

        public static bool IsTokenPlacedAfterExpression(
            ExpressionSyntax left,
            SyntaxToken token,
            ExpressionSyntax right)
        {
            if (!left.GetTrailingTrivia().SingleOrDefault(shouldThrow: false).IsKind(SyntaxKind.WhitespaceTrivia))
                return false;

            if (token.LeadingTrivia.Any())
                return false;

            if (!IsOptionalWhitespaceTriviaFollowedWithEndOfLineTrivia(token.TrailingTrivia))
                return false;

            if (!right.GetLeadingTrivia().IsEmptyOrWhitespace())
                return false;

            return true;
        }

        public static bool IsOptionalWhitespaceTriviaFollowedWithEndOfLineTrivia(SyntaxTriviaList triviaList)
        {
            SyntaxTriviaList.Enumerator en = triviaList.GetEnumerator();

            if (!en.MoveNext())
                return false;

            SyntaxKind kind = en.Current.Kind();

            if (kind == SyntaxKind.WhitespaceTrivia)
            {
                if (!en.MoveNext())
                    return false;

                kind = en.Current.Kind();
            }

            return kind == SyntaxKind.EndOfLineTrivia
                && !en.MoveNext();
        }

        public static bool IsOptionalWhitespaceThenOptionalSingleLineCommentThenEndOfLineTrivia(SyntaxTriviaList triviaList)
        {
            SyntaxTriviaList.Enumerator en = triviaList.GetEnumerator();

            if (!en.MoveNext())
                return false;

            SyntaxKind kind = en.Current.Kind();

            if (kind == SyntaxKind.WhitespaceTrivia)
            {
                if (!en.MoveNext())
                    return false;

                kind = en.Current.Kind();
            }

            if (kind == SyntaxKind.SingleLineCommentTrivia)
            {
                if (!en.MoveNext())
                    return false;

                kind = en.Current.Kind();
            }

            return kind == SyntaxKind.EndOfLineTrivia
                && !en.MoveNext();
        }
    }
}
