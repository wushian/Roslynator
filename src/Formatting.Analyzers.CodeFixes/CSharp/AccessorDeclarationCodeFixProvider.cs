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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AccessorDeclarationCodeFixProvider))]
    [Shared]
    public class AccessorDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.RemoveNewLinesFromAccessorWithSinglelineExpression); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out AccessorDeclarationSyntax accessorDeclaration))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            CodeAction codeAction = CodeAction.Create(
                "Remove newlines from accessor",
                ct => RemoveNewLinesFromAccessorAsync(document, accessorDeclaration, ct),
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static Task<Document> RemoveNewLinesFromAccessorAsync(
            Document document,
            AccessorDeclarationSyntax accessorDeclaration,
            CancellationToken cancellationToken)
        {
            AccessorDeclarationSyntax newAccessorDeclaration = accessorDeclaration
                .RemoveWhitespace(accessorDeclaration.Span)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(accessorDeclaration, newAccessorDeclaration, cancellationToken);
        }
    }
}
