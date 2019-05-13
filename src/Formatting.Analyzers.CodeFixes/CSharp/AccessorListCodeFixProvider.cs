// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AccessorListCodeFixProvider))]
    [Shared]
    public class AccessorListCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.RemoveNewLinesFromAccessorListOfAutoProperty,
                    DiagnosticIdentifiers.AddNewLinesToAccessorListOfFullProperty);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out AccessorListSyntax accessorList))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.AddNewLinesToAccessorListOfFullProperty:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Add newlines to accessor list",
                            ct => AddNewLinesToAccessorListAsync(document, accessorList, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIdentifiers.RemoveNewLinesFromAccessorListOfAutoProperty:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Remove newlines from accessor list",
                            ct => RemoveNewLinesFromAccessorListAsync(document, accessorList, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }

        private static Task<Document> AddNewLinesToAccessorListAsync(
            Document document,
            AccessorListSyntax accessorList,
            CancellationToken cancellationToken)
        {
            SyntaxTriviaList triviaList = accessorList
                .CloseBraceToken
                .LeadingTrivia
                .Add(NewLine());

            AccessorListSyntax newAccessorList = accessorList
                .RemoveWhitespace()
                .WithCloseBraceToken(accessorList.CloseBraceToken.WithLeadingTrivia(triviaList));

            SyntaxList<AccessorDeclarationSyntax> accessors = newAccessorList.Accessors;

            if (accessors.Count > 1)
            {
                AccessorDeclarationSyntax accessor = accessors[0];

                if (accessorList.SyntaxTree.IsSingleLineSpan(accessor.GetTrailingTrivia().Span, cancellationToken))
                    newAccessorList = newAccessorList.ReplaceNode(accessor, accessor.AppendToTrailingTrivia(NewLine()));
            }

            newAccessorList = newAccessorList.WithFormatterAnnotation();

            return document.ReplaceNodeAsync(accessorList, newAccessorList, cancellationToken);
        }

        private static Task<Document> RemoveNewLinesFromAccessorListAsync(
            Document document,
            AccessorListSyntax accessorList,
            CancellationToken cancellationToken)
        {
            switch (accessorList.Parent)
            {
                case PropertyDeclarationSyntax propertyDeclaration:
                    {
                        TextSpan span = TextSpan.FromBounds(
                            propertyDeclaration.Identifier.Span.End,
                            accessorList.CloseBraceToken.SpanStart);

                        PropertyDeclarationSyntax newNode = propertyDeclaration
                            .RemoveWhitespace(span)
                            .WithFormatterAnnotation();

                        return document.ReplaceNodeAsync(propertyDeclaration, newNode, cancellationToken);
                    }
                case IndexerDeclarationSyntax indexerDeclaration:
                    {
                        TextSpan span = TextSpan.FromBounds(
                            indexerDeclaration.ParameterList.CloseBracketToken.Span.End,
                            accessorList.CloseBraceToken.SpanStart);

                        IndexerDeclarationSyntax newNode = indexerDeclaration
                            .RemoveWhitespace(span)
                            .WithFormatterAnnotation();

                        return document.ReplaceNodeAsync(indexerDeclaration, newNode, cancellationToken);
                    }
                default:
                    {
                        throw new InvalidOperationException();
                    }
            }
        }
    }
}
