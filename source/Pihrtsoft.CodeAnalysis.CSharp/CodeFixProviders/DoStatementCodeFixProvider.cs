﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DoStatementCodeFixProvider))]
    [Shared]
    public class DoStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.AvoidUsageOfDoStatementToCreateInfiniteLoop);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            DoStatementSyntax doStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<DoStatementSyntax>();

            if (doStatement == null)
                return;

            TextSpan span = TextSpan.FromBounds(
                doStatement.Statement.Span.End,
                doStatement.Span.End);

            if (root
                .DescendantTrivia(span)
                .All(f => f.IsWhitespaceOrEndOfLine()))
            {
                CodeAction codeAction = CodeAction.Create(
                    "Use while statement to create an infinite loop",
                    cancellationToken =>
                    {
                        return DoStatementRefactoring.ConvertToWhileStatementAsync(
                            context.Document,
                            doStatement,
                            cancellationToken);
                    },
                    DiagnosticIdentifiers.AvoidUsageOfDoStatementToCreateInfiniteLoop + EquivalenceKeySuffix);

                context.RegisterCodeFix(codeAction, context.Diagnostics);
            }
        }
    }
}
