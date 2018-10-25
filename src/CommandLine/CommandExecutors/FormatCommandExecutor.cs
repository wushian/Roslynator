// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CodeFixes;
using Roslynator.Formatting;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal class FormatCommandExecutor : MSBuildWorkspaceCommandExecutor
    {
        public FormatCommandExecutor(FormatCommandLineOptions options)
        {
            Options = options;
        }

        public FormatCommandLineOptions Options { get; }

        public override async Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
        {
            ImmutableArray<string> supportedDiagnosticIds = Options
                .GetSupportedDiagnostics()
                .Select(f => f.Id)
                .ToImmutableArray();

            if (supportedDiagnosticIds.Any())
            {
                var codeFixerOptions = new CodeFixerOptions(
                    minimalSeverity: DiagnosticSeverity.Hidden,
                    ignoreCompilerErrors: true,
                    ignoreAnalyzerReferences: true,
                    supportedDiagnosticIds: supportedDiagnosticIds,
                    batchSize: 1000,
                    format: true);

                return await FixCommandExecutor.FixAsync(
                    projectOrSolution,
                    FixCommandExecutor.RoslynatorAnalyzerAssemblies,
                    codeFixerOptions,
                    default(IFormatProvider),
                    cancellationToken);
            }

            var options = new CodeFormatterOptions(includeGenerated: Options.IncludeGenerated);

            Workspace workspace = projectOrSolution.Workspace;

            if (projectOrSolution.IsProject)
            {
                Project project = projectOrSolution.AsProject();

                string solutionDirectory = Path.GetDirectoryName(project.Solution.FilePath);

                WriteLine($"  Analyze '{project.Name}'", Verbosity.Minimal);

                Project newProject = await CodeFormatter.FormatProjectAsync(project, options, cancellationToken);

                bool hasChanges = false;

                foreach (DocumentId documentId in newProject
                    .GetChanges(project)
                    .GetChangedDocuments(onlyGetDocumentsWithTextChanges: true))
                {
                    Document document = newProject.GetDocument(documentId);

                    IEnumerable<TextChange> textChanges = await document.GetTextChangesAsync(project.GetDocument(documentId));

                    if (textChanges.Any())
                    {
                        hasChanges = true;

                        WriteLine($"  Format '{PathUtilities.MakeRelativePath(document.FilePath, solutionDirectory)}'", ConsoleColor.DarkGray, Verbosity.Detailed);
#if DEBUG
                        await VerifySyntaxTreeEquivalence(document, project, cancellationToken);
#endif
                    }
                }

                if (hasChanges)
                {
                    Solution solution = newProject.Solution;

                    WriteLine($"Apply changes to solution '{solution.FilePath}'", Verbosity.Normal);

                    if (!workspace.TryApplyChanges(solution))
                    {
                        Debug.Fail($"Cannot apply changes to solution '{solution.FilePath}'");
                        WriteLine($"Cannot apply changes to solution '{solution.FilePath}'", ConsoleColor.Yellow, Verbosity.Detailed);
                    }
                }
            }
            else
            {
                Solution solution = projectOrSolution.AsSolution();

                string solutionDirectory = Path.GetDirectoryName(solution.FilePath);

                WriteLine($"Analyze solution '{projectOrSolution.FilePath}'", ConsoleColor.Cyan, Verbosity.Minimal);

                Stopwatch stopwatch = Stopwatch.StartNew();

                var changedDocumentIds = new ConcurrentBag<(DocumentId, SourceText)>();

                Parallel.ForEach(FilterProjects(solution, Options.IgnoredProjects, Options.Language), project =>
                {
                    WriteLine($"  Analyze '{project.Name}'", Verbosity.Minimal);

                    Project newProject = CodeFormatter.FormatProjectAsync(project, options, cancellationToken).Result;

                    foreach (DocumentId documentId in newProject
                        .GetChanges(project)
                        .GetChangedDocuments(onlyGetDocumentsWithTextChanges: true))
                    {
                        Document document = newProject.GetDocument(documentId);

                        IEnumerable<TextChange> textChanges = document.GetTextChangesAsync(project.GetDocument(documentId)).Result;

                        if (textChanges.Any())
                        {
                            WriteLine($"  Format '{PathUtilities.MakeRelativePath(document.FilePath, solutionDirectory)}'", ConsoleColor.DarkGray, Verbosity.Detailed);
#if DEBUG
                            VerifySyntaxTreeEquivalence(document, project, cancellationToken).Wait(cancellationToken);
#endif
                            SourceText sourceText = document.GetTextAsync(cancellationToken).Result;

                            changedDocumentIds.Add((document.Id, sourceText));
                        }
                    }

                    WriteLine($"  Done analyzing '{project.Name}'", Verbosity.Normal);
                });

                if (changedDocumentIds.Count > 0)
                {
                    foreach ((DocumentId documentId, SourceText sourceText) in changedDocumentIds)
                    {
                        solution = solution.WithDocumentText(documentId, sourceText);
                    }

                    WriteLine($"Apply changes to solution '{solution.FilePath}'", Verbosity.Normal);

                    if (!workspace.TryApplyChanges(solution))
                    {
                        Debug.Fail($"Cannot apply changes to solution '{solution.FilePath}'");
                        WriteLine($"Cannot apply changes to solution '{solution.FilePath}'", ConsoleColor.Yellow, Verbosity.Detailed);
                    }
                }

                WriteLine($"Done formatting solution '{solution.FilePath}' {stopwatch.Elapsed:mm\\:ss\\.ff}", ConsoleColor.Green, Verbosity.Minimal);
            }

            return new CommandResult(true);
        }

        protected override void OperationCanceled(OperationCanceledException ex)
        {
            WriteLine("Formatting was canceled.", Verbosity.Minimal);
        }

        private static async Task VerifySyntaxTreeEquivalence(
            Document newDocument,
            Project oldProject,
            CancellationToken cancellationToken)
        {
            if (!string.Equals(
                (await newDocument.GetSyntaxRootAsync(cancellationToken)).NormalizeWhitespace("", false).ToFullString(),
                (await oldProject.GetDocument(newDocument.Id).GetSyntaxRootAsync(cancellationToken)).NormalizeWhitespace("", false).ToFullString(),
                StringComparison.Ordinal))
            {
                WriteLine("Syntax roots with normalized white-space are not equivalent", ConsoleColor.Magenta);
            }

            switch (oldProject.Language)
            {
                case LanguageNames.CSharp:
                    {
                        if (!Microsoft.CodeAnalysis.CSharp.SyntaxFactory.AreEquivalent(
                            await newDocument.GetSyntaxTreeAsync(cancellationToken),
                            await oldProject.GetDocument(newDocument.Id).GetSyntaxTreeAsync(cancellationToken),
                            topLevel: false))
                        {
                            WriteLine("Syntax trees are not equivalent", ConsoleColor.Magenta);
                        }

                        break;
                    }
                case LanguageNames.VisualBasic:
                    {
                        if (!Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.AreEquivalent(
                            await newDocument.GetSyntaxTreeAsync(cancellationToken),
                            await oldProject.GetDocument(newDocument.Id).GetSyntaxTreeAsync(cancellationToken),
                            topLevel: false))
                        {
                            WriteLine("Syntax trees are not equivalent", ConsoleColor.Magenta);
                        }

                        break;
                    }
                default:
                    {
                        Debug.Fail(oldProject.Language);
                        break;
                    }
            }
        }
    }
}
