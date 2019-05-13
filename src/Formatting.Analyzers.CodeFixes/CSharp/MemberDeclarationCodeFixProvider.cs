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
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MemberDeclarationCodeFixProvider))]
    [Shared]
    public class MemberDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.AddNewLineToEmptyTypeDeclaration); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out MemberDeclarationSyntax memberDeclaration))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.AddNewLineToEmptyTypeDeclaration:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Add newline",
                            ct => AddNewLineBeforeClosingBraceOfEmptyTypeDeclarationAsync(document, memberDeclaration, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }

        private static Task<Document> AddNewLineBeforeClosingBraceOfEmptyTypeDeclarationAsync(
            Document document,
            MemberDeclarationSyntax declaration,
            CancellationToken cancellationToken)
        {
            MemberDeclarationSyntax newNode = GetNewDeclaration().WithFormatterAnnotation();

            return document.ReplaceNodeAsync(declaration, newNode, cancellationToken);

            MemberDeclarationSyntax GetNewDeclaration()
            {
                switch (declaration.Kind())
                {
                    case SyntaxKind.ClassDeclaration:
                        {
                            var classDeclaration = (ClassDeclarationSyntax)declaration;

                            return classDeclaration
                                .WithOpenBraceToken(classDeclaration.OpenBraceToken.WithoutTrailingTrivia())
                                .WithCloseBraceToken(classDeclaration.CloseBraceToken.WithLeadingTrivia(CSharpFactory.NewLine()));
                        }
                    case SyntaxKind.StructDeclaration:
                        {
                            var structDeclaration = (StructDeclarationSyntax)declaration;

                            return structDeclaration
                                .WithOpenBraceToken(structDeclaration.OpenBraceToken.WithoutTrailingTrivia())
                                .WithCloseBraceToken(structDeclaration.CloseBraceToken.WithLeadingTrivia(CSharpFactory.NewLine()));
                        }
                    case SyntaxKind.InterfaceDeclaration:
                        {
                            var interfaceDeclaration = (InterfaceDeclarationSyntax)declaration;

                            return interfaceDeclaration
                                .WithOpenBraceToken(interfaceDeclaration.OpenBraceToken.WithoutTrailingTrivia())
                                .WithCloseBraceToken(interfaceDeclaration.CloseBraceToken.WithLeadingTrivia(CSharpFactory.NewLine()));
                        }
                }

                return declaration;
            }
        }
    }
}
