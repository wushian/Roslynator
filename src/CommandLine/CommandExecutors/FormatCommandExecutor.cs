// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Formatting;
using static Roslynator.ConsoleHelpers;

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
            var options = new CodeFormatterOptions(includeGenerated: Options.IncludeGenerated);

            Workspace workspace = projectOrSolution.Workspace;

            if (projectOrSolution.IsProject)
            {
                Project project = projectOrSolution.AsProject();

                WriteLine($"  Analyze '{project.Name}'");

                Project newProject = await CodeFormatter.FormatProjectAsync(project, options, cancellationToken);

                foreach (DocumentId documentId in newProject
                    .GetChanges(project)
                    .GetChangedDocuments(onlyGetDocumentsWithTextChanges: true))
                {
                    Document document = newProject.GetDocument(documentId);

                    IEnumerable<TextChange> textChanges = await document.GetTextChangesAsync(project.GetDocument(documentId));

                    if (textChanges.Any())
                        WriteLine($"  Format '{PathUtilities.MakeRelativePath(document, newProject)}'");
                }

                bool success = workspace.TryApplyChanges(newProject.Solution);

                Debug.Assert(success, "Cannot apply changes to a solution.");
            }
            else
            {
                Solution solution = projectOrSolution.AsSolution();

                WriteLine($"Analyze solution '{projectOrSolution.FilePath}'", ConsoleColor.Cyan);

                Stopwatch stopwatch = Stopwatch.StartNew();

                var changedDocumentIds = new ConcurrentBag<(DocumentId, SourceText)>();

                Parallel.ForEach(FilterProjects(solution, Options.IgnoredProjects, Options.Language), project =>
                {
                    WriteLine($"  Analyze '{project.Name}'");

                    Project newProject = CodeFormatter.FormatProjectAsync(project, options, cancellationToken).Result;

                    foreach (DocumentId documentId in newProject
                        .GetChanges(project)
                        .GetChangedDocuments(onlyGetDocumentsWithTextChanges: true))
                    {
                        Document document = newProject.GetDocument(documentId);

                        IEnumerable<TextChange> textChanges = document.GetTextChangesAsync(project.GetDocument(documentId)).Result;

                        if (textChanges.Any())
                        {
                            WriteLine($"  Format '{PathUtilities.MakeRelativePath(document, newProject)}'");

                            SourceText sourceText = document.GetTextAsync(cancellationToken).Result;

                            changedDocumentIds.Add((document.Id, sourceText));
                        }
                    }

                    WriteLine($"  Done analyzing '{project.Name}'");
                });

                foreach ((DocumentId documentId, SourceText sourceText) in changedDocumentIds)
                {
                    solution = solution.WithDocumentText(documentId, sourceText);
                }

                WriteLine($"Apply changes to solution '{solution.FilePath}'");

                bool success = workspace.TryApplyChanges(solution);

                Debug.Assert(success, $"Cannot apply changes to solution '{solution.FilePath}'");

                WriteLine($"Done formatting solution '{solution.FilePath}' {stopwatch.Elapsed:mm\\:ss\\.ff}", ConsoleColor.Green);
            }

            return new CommandResult(true);
        }

        protected override void OperationCanceled(OperationCanceledException ex)
        {
            WriteLine("Formatting was canceled.");
        }
    }
}
