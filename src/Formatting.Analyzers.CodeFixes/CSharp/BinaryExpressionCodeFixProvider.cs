// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BinaryExpressionCodeFixProvider))]
    [Shared]
    public class BinaryExpressionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.PlaceBinaryOperatorBeforeOperand,
                    DiagnosticIdentifiers.PlaceBinaryOperatorAfterOperand);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out BinaryExpressionSyntax binaryExpression))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.PlaceBinaryOperatorBeforeOperand:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            $"Place '{binaryExpression.OperatorToken.ToString()}' before operand",
                            ct => PlaceBinaryOperatorBeforeOperandAsync(document, binaryExpression, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIdentifiers.PlaceBinaryOperatorAfterOperand:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            $"Place '{binaryExpression.OperatorToken.ToString()}' after operand",
                            ct => PlaceBinaryOperatorAfterOperandAsync(document, binaryExpression, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }

        private static Task<Document> PlaceBinaryOperatorBeforeOperandAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            var (left, token, right) = SyntaxTriviaManipulation.PlaceTokenBeforeExpression(binaryExpression.Left, binaryExpression.OperatorToken, binaryExpression.Right);

            BinaryExpressionSyntax newBinaryExpression = BinaryExpression(
                binaryExpression.Kind(),
                left,
                token,
                right);

            return document.ReplaceNodeAsync(binaryExpression, newBinaryExpression, cancellationToken);
        }

        private static Task<Document> PlaceBinaryOperatorAfterOperandAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            var (left, token, right) = SyntaxTriviaManipulation.PlaceTokenAfterExpression(binaryExpression.Left, binaryExpression.OperatorToken, binaryExpression.Right);

            BinaryExpressionSyntax newBinaryExpression = BinaryExpression(
                binaryExpression.Kind(),
                left,
                token,
                right);

            return document.ReplaceNodeAsync(binaryExpression, newBinaryExpression, cancellationToken);
        }
    }
}