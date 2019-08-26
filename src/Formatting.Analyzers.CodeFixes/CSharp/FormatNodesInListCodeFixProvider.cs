// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
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
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FormatNodesInListCodeFixProvider))]
    [Shared]
    public class FormatNodesInListCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.FormatNodesInList); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(
                root,
                context.Span,
                out SyntaxNode node,
                predicate: f =>
                {
                    switch (f.Kind())
                    {
                        case SyntaxKind.TypeArgumentList:
                        case SyntaxKind.ArgumentList:
                        case SyntaxKind.BracketedArgumentList:
                        case SyntaxKind.AttributeList:
                        case SyntaxKind.AttributeArgumentList:
                        case SyntaxKind.BaseList:
                        case SyntaxKind.ParameterList:
                        case SyntaxKind.BracketedParameterList:
                        case SyntaxKind.TypeParameterList:
                            return true;
                    }

                    return false;
                }))
            {
                return;
            }

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            CodeAction codeAction = CodeAction.Create(
                "Format arguments on separate lines",
                ct => FormatArgumentsOnSeparateLinesAsync(document, node, ct),
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static Task<Document> FormatArgumentsOnSeparateLinesAsync(
            Document document,
            SyntaxNode node,
            CancellationToken cancellationToken)
        {
            switch (node)
            {
                case TypeArgumentListSyntax typeArgumentList:
                    return FormatArgumentsOnSeparateLinesAsync(document, typeArgumentList.Arguments, cancellationToken);
                case BaseArgumentListSyntax argumentList:
                    return FormatArgumentsOnSeparateLinesAsync(document, argumentList.Arguments, cancellationToken);
                case AttributeListSyntax attributeList:
                    return FormatArgumentsOnSeparateLinesAsync(document, attributeList.Attributes, cancellationToken);
                case AttributeArgumentListSyntax attributeArgumentList:
                    return FormatArgumentsOnSeparateLinesAsync(document, attributeArgumentList.Arguments, cancellationToken);
                case BaseListSyntax baseList:
                    return FormatArgumentsOnSeparateLinesAsync(document, baseList.Types, cancellationToken);
                case BaseParameterListSyntax parameterList:
                    return FormatArgumentsOnSeparateLinesAsync(document, parameterList.Parameters, cancellationToken);
                case TypeParameterListSyntax typeParameterList:
                    return FormatArgumentsOnSeparateLinesAsync(document, typeParameterList.Parameters, cancellationToken);
                default:
                    throw new InvalidOperationException();
            }
        }

        private static Task<Document> FormatArgumentsOnSeparateLinesAsync<TNode>(
            Document document,
            SeparatedSyntaxList<TNode> nodes,
            CancellationToken cancellationToken) where TNode : SyntaxNode
        {
            return document.WithTextChangesAsync(GetTextChanges().Where(f => f != default), cancellationToken);

            IEnumerable<TextChange> GetTextChanges()
            {
                SyntaxTree syntaxTree = nodes[0].SyntaxTree;

                string newText = SyntaxTriviaAnalysis.GetEndOfLine(nodes[0]).ToString() + GetIndentation().ToString();

                yield return GetTextChange(nodes[0].GetFirstToken().GetPreviousToken(), nodes[0]);

                for (int i = 1; i < nodes.Count; i++)
                {
                    yield return GetTextChange(nodes.GetSeparator(i - 1), nodes[i]);
                }

                TextChange GetTextChange(SyntaxToken token, SyntaxNode node)
                {
                    TextSpan span = TextSpan.FromBounds(token.Span.End, node.SpanStart);

                    return (syntaxTree.IsSingleLineSpan(span))
                        ? new TextChange(span, newText)
                        : default;
                }
            }

            SyntaxTrivia GetIndentation()
            {
                foreach (TNode node in nodes)
                {
                    SyntaxTriviaList.Reversed.Enumerator en = node.GetLeadingTrivia().Reverse().GetEnumerator();

                    if (en.MoveNext()
                        && en.Current.IsWhitespaceTrivia())
                    {
                        SyntaxTrivia whitespaceTrivia = en.Current;

                        if (!en.MoveNext()
                            || en.Current.IsEndOfLineTrivia())
                        {
                            return whitespaceTrivia;
                        }
                    }
                }

                return nodes[0].GetIndentation(cancellationToken);
            }
        }
    }
}
