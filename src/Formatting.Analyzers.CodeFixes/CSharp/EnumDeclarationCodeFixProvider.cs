// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumDeclarationCodeFixProvider))]
    [Shared]
    public class EnumDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.AddNewlineBeforeEnumMember); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out EnumDeclarationSyntax enumDeclaration))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            CodeAction codeAction = CodeAction.Create(
                "Add newline",
                ct => AddNewlineBeforeEnumMemberAsync(document, enumDeclaration, ct),
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static Task<Document> AddNewlineBeforeEnumMemberAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            CancellationToken cancellationToken)
        {
            var rewriter = new AddNewlineBeforeEnumMemberAsyncRewriter(enumDeclaration);

            SyntaxNode newNode = rewriter.Visit(enumDeclaration).WithFormatterAnnotation();

            return document.ReplaceNodeAsync(enumDeclaration, newNode, cancellationToken);
        }

        private class AddNewlineBeforeEnumMemberAsyncRewriter : CSharpSyntaxRewriter
        {
            private readonly SyntaxToken[] _separators;

            public AddNewlineBeforeEnumMemberAsyncRewriter(EnumDeclarationSyntax enumDeclaration)
            {
                _separators = enumDeclaration.Members.GetSeparators().ToArray();
            }

            public override SyntaxToken VisitToken(SyntaxToken token)
            {
                if (_separators.Contains(token)
                    && !token.TrailingTrivia.Contains(SyntaxKind.EndOfLineTrivia))
                {
                    return token.TrimTrailingTrivia().AppendToTrailingTrivia(CSharpFactory.NewLine());
                }

                return base.VisitToken(token);
            }
        }
    }
}
