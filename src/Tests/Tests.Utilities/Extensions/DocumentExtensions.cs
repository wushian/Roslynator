// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;

namespace Roslynator
{
    public static class DocumentExtensions
    {
        public static async Task<string> ToSimplifiedAndFormattedFullStringAsync(this Document document)
        {
            document = await Simplifier.ReduceAsync(document, Simplifier.Annotation).ConfigureAwait(false);

            SyntaxNode root = await document.GetSyntaxRootAsync().ConfigureAwait(false);

            root = Formatter.Format(root, Formatter.Annotation, document.Project.Solution.Workspace);

            return root.ToFullString();
        }

        public static async Task<Document> ApplyCodeActionAsync(this Document document, CodeAction codeAction)
        {
            ImmutableArray<CodeActionOperation> operations = await codeAction.GetOperationsAsync(CancellationToken.None).ConfigureAwait(false);

            return operations
                .OfType<ApplyChangesOperation>()
                .Single()
                .ChangedSolution
                .GetDocument(document.Id);
        }
    }
}
