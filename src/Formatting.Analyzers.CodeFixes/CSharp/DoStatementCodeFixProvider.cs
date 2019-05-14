// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DoStatementCodeFixProvider))]
    [Shared]
    public class DoStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.RemoveNewLineBeforeWhileKeywordOfDoStatement); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out DoStatementSyntax doStatement))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.RemoveNewLineBeforeWhileKeywordOfDoStatement:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Remove newline",
                            ct => RemoveNewLineBeforeWhileKeywordOfDoStatementAsync(document, doStatement, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }

        private static Task<Document> RemoveNewLineBeforeWhileKeywordOfDoStatementAsync(
            Document document,
            DoStatementSyntax doStatement,
            CancellationToken cancellationToken)
        {
            DoStatementSyntax newDoStatement = doStatement
                .WithStatement(doStatement.Statement.WithTrailingTrivia(SyntaxFactory.Space))
                .WithWhileKeyword(doStatement.WhileKeyword.WithoutLeadingTrivia());

            return document.ReplaceNodeAsync(doStatement, newDoStatement, cancellationToken);
        }
    }
}
