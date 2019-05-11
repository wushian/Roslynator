// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseSpacesInsteadOfTabCodeFixProvider))]
    [Shared]
    public class UseSpacesInsteadOfTabCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.UseSpacesInsteadOfTab); }
        }

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            CodeAction codeAction = CodeAction.Create(
                "Use spaces instead of tab",
                ct => UseSpacesInsteadOfTabAsync(document, context.Span, ct),
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);

            return Task.CompletedTask;
        }

        private static async Task<Document> UseSpacesInsteadOfTabAsync(
            Document document,
            TextSpan span,
            CancellationToken cancellationToken = default)
        {
            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            var textChange = new TextChange(span, new string(' ', span.Length * 4));

            SourceText newSourceText = sourceText.WithChanges(textChange);

            return document.WithText(newSourceText);
        }
    }
}
