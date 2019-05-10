// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddEmptyLineBetweenDeclarationsCodeFixProvider))]
    [Shared]
    public class AddEmptyLineBetweenDeclarationsCodeFixProvider : BaseCodeFixProvider
    {
        private const string Title = "Add empty line";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.AddEmptyLineBetweenDeclarations); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            SyntaxToken token = root.FindToken(context.Span.Start);

            if (token.IsKind(SyntaxKind.CommaToken))
            {
                CodeAction codeAction = CodeAction.Create(
                    Title,
                    ct =>
                    {
                        SyntaxToken newToken = token.AppendToTrailingTrivia(CSharpFactory.NewLine());

                        return document.ReplaceTokenAsync(token, newToken, ct);
                    },
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
            else
            {
                if (!TryFindFirstAncestorOrSelf(root, context.Span, out MemberDeclarationSyntax memberDeclaration))
                    return;

                CodeAction codeAction = CodeAction.Create(
                    Title,
                    ct =>
                    {
                        MemberDeclarationSyntax newMemberDeclaration = memberDeclaration.AppendToTrailingTrivia(CSharpFactory.NewLine());

                        return document.ReplaceNodeAsync(memberDeclaration, newMemberDeclaration, ct);
                    },
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }
    }
}
