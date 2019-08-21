// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EmptyLineBetweenDeclarationsCodeFixProvider))]
    [Shared]
    public class EmptyLineBetweenDeclarationsCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.AddEmptyLineBetweenDeclarations,
                    DiagnosticIdentifiers.AddEmptyLineBetweenSinglelineDeclarations,
                    DiagnosticIdentifiers.AddEmptyLineBetweenDeclarationAndDocumentationComment,
                    DiagnosticIdentifiers.AddEmptyLineBetweenSinglelineDeclarationsOfDifferentKind,
                    DiagnosticIdentifiers.RemoveEmptyLineBetweenSinglelineDeclarationsOfSameKind);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            if (!TryFindTrivia(root, context.Span.Start, out SyntaxTrivia trivia, findInsideTrivia: false))
                return;

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.AddEmptyLineBetweenDeclarations:
                case DiagnosticIdentifiers.AddEmptyLineBetweenSinglelineDeclarations:
                case DiagnosticIdentifiers.AddEmptyLineBetweenDeclarationAndDocumentationComment:
                case DiagnosticIdentifiers.AddEmptyLineBetweenSinglelineDeclarationsOfDifferentKind:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            CodeFixTitles.AddEmptyLine,
                            ct =>
                            {
                                SyntaxToken token = trivia.Token;
                                SyntaxToken newToken = token.AppendEndOfLineToTrailingTrivia();

                                return document.ReplaceTokenAsync(token, newToken, ct);
                            },
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIdentifiers.RemoveEmptyLineBetweenSinglelineDeclarationsOfSameKind:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            CodeFixTitles.RemoveEmptyLine,
                            ct => RemoveEmptyLineAsync(document, trivia, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }

        private static Task<Document> RemoveEmptyLineAsync(
            Document document,
            SyntaxTrivia trivia,
            CancellationToken cancellationToken)
        {
            SyntaxToken token = trivia.Token;
            SyntaxTriviaList leadingTrivia = token.LeadingTrivia;

            int count = 0;

            SyntaxTriviaList.Enumerator en = leadingTrivia.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Current.IsWhitespaceTrivia())
                {
                    if (!en.MoveNext())
                        break;

                    if (!en.Current.IsEndOfLineTrivia())
                        break;

                    count += 2;
                }
                else if (en.Current.IsEndOfLineTrivia())
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            SyntaxToken newToken = token.WithLeadingTrivia(leadingTrivia.RemoveRange(0, count));

            return document.ReplaceTokenAsync(token, newToken, cancellationToken);
        }
    }
}
