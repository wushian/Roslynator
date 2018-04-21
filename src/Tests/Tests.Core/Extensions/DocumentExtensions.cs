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
        public static string GetSimplifiedAndFormattedText(this Document document)
        {
            return GetSimplifiedAndFormattedTextAsync(document).Result;
        }

        public static async Task<string> GetSimplifiedAndFormattedTextAsync(this Document document)
        {
            Document simplifiedDocument = await Simplifier.ReduceAsync(document, Simplifier.Annotation).ConfigureAwait(false);

            SyntaxNode root = await simplifiedDocument.GetSyntaxRootAsync().ConfigureAwait(false);

            root = Formatter.Format(root, Formatter.Annotation, simplifiedDocument.Project.Solution.Workspace);

            return root.ToFullString();
        }

        public static Document ApplyCodeAction(this Document document, CodeAction codeAction)
        {
            return codeAction
                .GetOperationsAsync(CancellationToken.None)
                .Result
                .OfType<ApplyChangesOperation>()
                .Single()
                .ChangedSolution
                .GetDocument(document.Id);
        }

        public static ImmutableArray<Diagnostic> GetCompilerDiagnostics(this Document document, CancellationToken cancellation = default(CancellationToken))
        {
            return document.GetSemanticModelAsync(cancellation).Result.GetDiagnostics();
        }
    }
}
