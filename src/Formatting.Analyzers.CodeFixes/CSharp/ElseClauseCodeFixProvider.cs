// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Formatting.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ElseClauseCodeFixProvider))]
    [Shared]
    public class ElseClauseCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.RemoveNewLineBetweenIfKeywordAndElseKeyword); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out ElseClauseSyntax elseClause))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.RemoveNewLineBetweenIfKeywordAndElseKeyword:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            CodeFixTitles.RemoveNewLine,
                            ct => RemoveNewLineBetweenIfKeywordAndElseKeywordAsync(document, elseClause, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }

        private static Task<Document> RemoveNewLineBetweenIfKeywordAndElseKeywordAsync(
            Document document,
            ElseClauseSyntax elseClause,
            CancellationToken cancellationToken)
        {
            var ifStatement = (IfStatementSyntax)elseClause.Statement;

            ElseClauseSyntax newElseCaluse = elseClause.Update(
                elseClause.ElseKeyword.WithTrailingTrivia(Space),
                elseClause.Statement.WithoutLeadingTrivia());

            return document.ReplaceNodeAsync(elseClause, newElseCaluse, cancellationToken);
        }
    }
}