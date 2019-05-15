// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

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
            switch (directiveTrivia.Kind())
            {
                case SyntaxKind.RegionDirectiveTrivia:
                    return CodeFixHelpers.AddEmptyLineAfterDirectiveAsync(document, directiveTrivia, cancellationToken);
                case SyntaxKind.EndRegionDirectiveTrivia:
                    return CodeFixHelpers.AddEmptyLineBeforeDirectiveAsync(document, directiveTrivia, cancellationToken);
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
