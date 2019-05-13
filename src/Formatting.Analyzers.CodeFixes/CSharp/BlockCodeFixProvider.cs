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

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BlockCodeFixProvider))]
    [Shared]
    public class BlockCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.AddNewLineToEmptyBlock,
                    DiagnosticIdentifiers.AddNewLinesToBlock);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out BlockSyntax block))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.AddNewLineToEmptyBlock:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Add newline",
                            ct => AddNewLineBeforeClosingBraceOfEmptyBlockAsync(document, block, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIdentifiers.AddNewLinesToBlock:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Add newlines",
                            ct => AddNewLinesToSinglelineBlockAsync(document, block, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }

        private static Task<Document> AddNewLineBeforeClosingBraceOfEmptyBlockAsync(
            Document document,
            BlockSyntax block,
            CancellationToken cancellationToken)
        {
            BlockSyntax newBlock = block
                .WithOpenBraceToken(block.OpenBraceToken.WithoutTrailingTrivia())
                .WithCloseBraceToken(block.CloseBraceToken.WithLeadingTrivia(SyntaxTriviaAnalysis.FindEndOfLine(block, CSharpFactory.NewLine())))
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(block, newBlock, cancellationToken);
        }

        private static Task<Document> AddNewLinesToSinglelineBlockAsync(
            Document document,
            BlockSyntax block,
            CancellationToken cancellationToken)
        {
            SyntaxToken closeBrace = block.CloseBraceToken;

            BlockSyntax newBlock = block
                .WithCloseBraceToken(closeBrace.AppendEndOfLineToLeadingTrivia())
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(block, newBlock, cancellationToken);
        }
    }
}
