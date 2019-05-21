// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddEmptyLineBetweenDeclarationsCodeFixProvider))]
    [Shared]
    public class AddEmptyLineBetweenDeclarationsCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.AddEmptyLineBetweenDeclarations,
                    DiagnosticIdentifiers.AddEmptyLineBetweenSinglelineDeclarations,
                    DiagnosticIdentifiers.AddEmptyLineBetweenDeclarationAndDocumentationComment);
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
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Add empty line",
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
            }
        }
    }
}
