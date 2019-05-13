// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DirectiveTriviaCodeFixProvider))]
    [Shared]
    public class DirectiveTriviaCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.AddEmptyLineAfterRegionAndBeforeEndRegion); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out DirectiveTriviaSyntax directiveTrivia, findInsideTrivia: true))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.AddEmptyLineAfterRegionAndBeforeEndRegion:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Add empty line",
                            ct => AddEmptyLineAfterRegionAndBeforeEndRegionAsync(document, directiveTrivia, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }

        private static Task<Document> AddEmptyLineAfterRegionAndBeforeEndRegionAsync(
            Document document,
            DirectiveTriviaSyntax directiveTrivia,
            CancellationToken cancellationToken)
        {
            if (directiveTrivia.IsKind(SyntaxKind.RegionDirectiveTrivia))
            {
                var regionDirective = (RegionDirectiveTriviaSyntax)directiveTrivia;
                SyntaxTrivia parentTrivia = regionDirective.ParentTrivia;
                SyntaxToken token = parentTrivia.Token;
                SyntaxTriviaList leadingTrivia = token.LeadingTrivia;

                int index = leadingTrivia.IndexOf(parentTrivia);

                SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(index + 1, SyntaxTriviaAnalysis.FindEndOfLine(token, CSharpFactory.NewLine()));

                SyntaxToken newToken = token.WithLeadingTrivia(newLeadingTrivia);

                return document.ReplaceTokenAsync(token, newToken, cancellationToken);
            }
            else if (directiveTrivia.IsKind(SyntaxKind.EndRegionDirectiveTrivia))
            {
                var regionDirective = (EndRegionDirectiveTriviaSyntax)directiveTrivia;
                SyntaxTrivia parentTrivia = regionDirective.ParentTrivia;
                SyntaxToken token = parentTrivia.Token;
                SyntaxTriviaList leadingTrivia = token.LeadingTrivia;

                int index = leadingTrivia.IndexOf(parentTrivia);

                if (index > 0
                    && leadingTrivia[index - 1].IsWhitespaceTrivia())
                {
                    index--;
                }

                SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(index, SyntaxTriviaAnalysis.FindEndOfLine(token, CSharpFactory.NewLine()));

                SyntaxToken newToken = token.WithLeadingTrivia(newLeadingTrivia);

                return document.ReplaceTokenAsync(token, newToken, cancellationToken);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
