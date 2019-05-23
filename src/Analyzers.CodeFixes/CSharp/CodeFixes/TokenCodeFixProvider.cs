// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TokenCodeFixProvider))]
    [Shared]
    public class TokenCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.RemoveUnnecessaryNewLine); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindToken(root, context.Span.Start, out SyntaxToken token))
                return;

            Diagnostic diagnostic = context.Diagnostics[0];
            Document document = context.Document;

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.RemoveUnnecessaryNewLine:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Remove new line",
                            ct => RemoveUnnecessaryNewLineAsync(document, token, ct),
                            GetEquivalenceKey(diagnostic.Id));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }

        private static Task<Document> RemoveUnnecessaryNewLineAsync(
            Document document,
            SyntaxToken token,
            CancellationToken cancellationToken)
        {
            SyntaxToken nextToken = token.GetNextToken();

            string newText = (nextToken.IsKind(SyntaxKind.SemicolonToken, SyntaxKind.ColonToken))
                ? ""
                : " ";

            var textChange = new TextChange(TextSpan.FromBounds(token.Span.End, nextToken.Span.Start), newText);

            return document.WithTextChangeAsync(textChange, cancellationToken);
        }
    }
}