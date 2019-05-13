// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp
{
    internal static class SyntaxTriviaAnalysis
    {
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

        private static bool IsOptionalWhitespaceTriviaFollowedWithEndOfLineTrivia(SyntaxTriviaList triviaList)
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
    }
}
