// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Metrics
{
    public abstract class CodeMetricsCounter
    {
        public abstract bool IsComment(SyntaxTrivia trivia);

        public abstract bool IsEndOfLine(SyntaxTrivia trivia);

        public static CodeMetricsCounter GetPhysicalLinesCounter(string language)
        {
            switch (language)
            {
                case LanguageNames.CSharp:
                    return CSharp.CSharpPhysicalLinesCounter.Instance;
                case LanguageNames.VisualBasic:
                    return VisualBasic.VisualBasicPhysicalLinesCounter.Instance;
            }

            Debug.Assert(language == LanguageNames.FSharp, language);

            return null;
        }

        public static CodeMetricsCounter GetLogicalLinesCounter(string language)
        {
            switch (language)
            {
                case LanguageNames.CSharp:
                    return CSharp.CSharpLogicalLinesCounter.Instance;
                case LanguageNames.VisualBasic:
                    {
                        //TODO: VisualBasicLogicalLinesCounter
                        return null;
                    }
            }

            Debug.Assert(language == LanguageNames.FSharp, language);

            return null;
        }

        protected abstract CodeMetrics CountLines(SyntaxNode node, SourceText sourceText, CodeMetricsOptions options, CancellationToken cancellationToken);

        public async Task<CodeMetrics> CountLinesAsync(
            Project project,
            CodeMetricsOptions options = null,
            CancellationToken cancellationToken = default)
        {
            int totalLineCount = 0;
            int codeLineCount = 0;
            int preprocessDirectiveLineCount = 0;
            int commentLineCount = 0;
            int whiteSpaceLineCount = 0;
            int blockBoundaryCount = 0;

            foreach (Document document in project.Documents)
            {
                if (!document.SupportsSyntaxTree)
                    continue;

                CodeMetrics metrics = await CountLinesAsync(document, options, cancellationToken).ConfigureAwait(false);

                totalLineCount += metrics.TotalLineCount;
                codeLineCount += metrics.CodeLineCount;
                preprocessDirectiveLineCount += metrics.PreprocessorDirectiveLineCount;
                commentLineCount += metrics.CommentLineCount;
                whiteSpaceLineCount += metrics.WhiteSpaceLineCount;
                blockBoundaryCount += metrics.BlockBoundaryLineCount;
            }

            return new CodeMetrics(
                totalLineCount: totalLineCount,
                codeLineCount: codeLineCount,
                whiteSpaceLineCount: whiteSpaceLineCount,
                commentLineCount: commentLineCount,
                preprocessorDirectiveLineCount: preprocessDirectiveLineCount,
                blockBoundaryLineCount: blockBoundaryCount);
        }

        public async Task<CodeMetrics> CountLinesAsync(
            Document document,
            CodeMetricsOptions options = null,
            CancellationToken cancellationToken = default)
        {
            SyntaxTree tree = await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);

            if (tree == null)
                return default;

            if (GeneratedCodeUtility.IsGeneratedCode(tree, IsComment, cancellationToken)
                && !options.IncludeGenerated)
            {
                return default;
            }

            SyntaxNode root = tree.GetRoot(cancellationToken);

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            TextLineCollection lines = sourceText.Lines;

            return CountLines(root, sourceText, options, cancellationToken);
        }

        private protected int CountWhiteSpaceLines(SyntaxNode node, SourceText sourceText, CodeMetricsOptions options)
        {
            int whiteSpaceLineCount = 0;

            if (!options.IncludeWhiteSpace)
            {
                foreach (TextLine line in sourceText.Lines)
                {
                    if (line.IsEmptyOrWhiteSpace())
                    {
                        if (line.End == sourceText.Length
                            || IsEndOfLine(node.FindTrivia(line.End)))
                        {
                            whiteSpaceLineCount++;
                        }
                    }
                }
            }

            return whiteSpaceLineCount;
        }
    }
}
