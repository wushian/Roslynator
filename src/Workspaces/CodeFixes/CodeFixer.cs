// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Formatting;
using static Roslynator.Logger;

namespace Roslynator.CodeFixes
{
    public class CodeFixer
    {
        private readonly AnalyzerAssemblyList _analyzerAssemblies = new AnalyzerAssemblyList();

        private readonly AnalyzerAssemblyList _analyzerReferences = new AnalyzerAssemblyList();

        private static readonly CompilationWithAnalyzersOptions _defaultCompilationWithAnalyzersOptions = new CompilationWithAnalyzersOptions(
            options: default(AnalyzerOptions),
            onAnalyzerException: default(Action<Exception, DiagnosticAnalyzer, Diagnostic>),
            concurrentAnalysis: true,
            logAnalyzerExecutionTime: false,
            reportSuppressedDiagnostics: false);

        public CodeFixer(Solution solution, IEnumerable<string> analyzerAssemblies = null, IFormatProvider formatProvider = null, CodeFixerOptions options = null)
        {
            Workspace = solution.Workspace;
            Options = options ?? CodeFixerOptions.Default;

            if (analyzerAssemblies != null)
                _analyzerAssemblies.LoadFrom(analyzerAssemblies);

            FormatProvider = formatProvider;
        }

        public Workspace Workspace { get; }

        public CodeFixerOptions Options { get; }

        public IFormatProvider FormatProvider { get; }

        private Solution CurrentSolution => Workspace.CurrentSolution;

        public async Task FixSolutionAsync(CancellationToken cancellationToken = default)
        {
            foreach (string id in Options.IgnoredCompilerDiagnosticIds.OrderBy(f => f))
                WriteLine($"Ignore compiler diagnostic '{id}'", Verbosity.Diagnostic);

            foreach (string id in Options.IgnoredDiagnosticIds.OrderBy(f => f))
                WriteLine($"Ignore diagnostic '{id}'", Verbosity.Diagnostic);

            ImmutableArray<ProjectId> projects = CurrentSolution
                .GetProjectDependencyGraph()
                .GetTopologicallySortedProjects(cancellationToken)
                .ToImmutableArray();

            var results = new List<ProjectFixResult>();

            Stopwatch stopwatch = Stopwatch.StartNew();

            TimeSpan lastElapsed = TimeSpan.Zero;

            for (int i = 0; i < projects.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Project project = CurrentSolution.GetProject(projects[i]);

                string language = project.Language;

                if (Options.IsSupportedProject(project))
                {
                    WriteLine($"Fix '{project.Name}' {$"{i + 1}/{projects.Length}"}", ConsoleColor.Cyan, Verbosity.Minimal);

                    ProjectFixResult result = await FixProjectAsync(project, cancellationToken).ConfigureAwait(false);

                    results.Add(result);

                    WriteFixSummary(
                        result.FixedDiagnostics,
                        result.UnfixedDiagnostics,
                        result.UnfixableDiagnostics,
                        indent: "  ",
                        verbosity: Verbosity.Detailed);

                    if (result.Kind == ProjectFixKind.CompilerError)
                        break;
                }
                else
                {
                    WriteLine($"Skip '{project.Name}' {$"{i + 1}/{projects.Length}"}", ConsoleColor.DarkGray, Verbosity.Minimal);

                    results.Add(ProjectFixResult.Skipped);
                }

                TimeSpan elapsed = stopwatch.Elapsed;

                WriteLine($"Done fixing '{project.Name}' in {elapsed - lastElapsed:mm\\:ss\\.ff}", Verbosity.Normal);

                lastElapsed = elapsed;
            }

            stopwatch.Stop();

            if (Options.Format)
            {
                int count = results.Sum(f => f.FormattedDocumentCount);
                WriteLine();
                WriteLine($"{count} {((count == 1) ? "document" : "documents")} formatted", ConsoleColor.Green, Verbosity.Normal);
            }

            WriteFixSummary(
                results.SelectMany(f => f.FixedDiagnostics),
                results.SelectMany(f => f.UnfixedDiagnostics),
                results.SelectMany(f => f.UnfixableDiagnostics),
                ConsoleColor.Green,
                addEmptyLine: true,
                verbosity: Verbosity.Normal);

            WriteLine(Verbosity.Minimal);
            WriteLine($"Done fixing solution '{CurrentSolution.FilePath}' in {stopwatch.Elapsed:mm\\:ss\\.ff}", ConsoleColor.Green, Verbosity.Minimal);
        }

        public async Task<ProjectFixResult> FixProjectAsync(Project project, CancellationToken cancellationToken = default)
        {
            (ImmutableArray<DiagnosticAnalyzer> analyzers, ImmutableArray<CodeFixProvider> fixers) = Utilities.GetAnalyzersAndFixers(
                project: project,
                analyzerAssemblies: _analyzerAssemblies,
                analyzerReferences: _analyzerReferences,
                options: Options);

            ProjectFixResult fixResult = await FixProjectAsync(project, analyzers, fixers, cancellationToken).ConfigureAwait(false);

            Compilation compilation = await CurrentSolution.GetProject(project.Id).GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            Dictionary<string, ImmutableArray<CodeFixProvider>> fixersById = fixers
                .SelectMany(f => f.FixableDiagnosticIds.Select(id => (id, fixer: f)))
                .GroupBy(f => f.id)
                .ToDictionary(f => f.Key, g => g.Select(f => f.fixer).Distinct().ToImmutableArray());

            ImmutableArray<DiagnosticDescriptor> unfixableDiagnostics = await GetDiagnosticsAsync(
                analyzers,
                fixResult.FixedDiagnostics,
                compilation,
                f => !fixersById.TryGetValue(f.id, out ImmutableArray<CodeFixProvider> fixers2),
                cancellationToken).ConfigureAwait(false);

            ImmutableArray<DiagnosticDescriptor> unfixedDiagnostics = await GetDiagnosticsAsync(
                analyzers,
                fixResult.FixedDiagnostics.Concat(unfixableDiagnostics),
                compilation,
                f => fixersById.TryGetValue(f.id, out ImmutableArray<CodeFixProvider> fixers2),
                cancellationToken).ConfigureAwait(false);

            if (Options.FileBannerLines.Length > 0)
                await AddFileBannerAsync(CurrentSolution.GetProject(project.Id), Options.FileBannerLines, cancellationToken).ConfigureAwait(false);

            int formattedDocumentCount = 0;

            if (Options.Format)
                formattedDocumentCount = await FormatProjectAsync(CurrentSolution.GetProject(project.Id), cancellationToken).ConfigureAwait(false);

            return new ProjectFixResult(
                kind: fixResult.Kind,
                fixedDiagnostics: fixResult.FixedDiagnostics,
                unfixedDiagnostics: unfixedDiagnostics,
                unfixableDiagnostics: unfixableDiagnostics,
                analyzers: fixResult.Analyzers,
                fixers: fixResult.Fixers,
                formattedDocumentCount: formattedDocumentCount);
        }

        private async Task<ProjectFixResult> FixProjectAsync(
            Project project,
            ImmutableArray<DiagnosticAnalyzer> analyzers,
            ImmutableArray<CodeFixProvider> fixers,
            CancellationToken cancellationToken)
        {
            if (!analyzers.Any())
            {
                WriteLine($"  No analyzers found to analyze '{project.Name}'", ConsoleColor.DarkGray, Verbosity.Normal);
                return ProjectFixResult.NoAnalyzers;
            }

            if (!fixers.Any())
            {
                WriteLine($"  No fixers found to fix '{project.Name}'", ConsoleColor.DarkGray, Verbosity.Normal);
                return new ProjectFixResult(ProjectFixKind.NoFixers, analyzers: analyzers, fixers: fixers);
            }

            Dictionary<string, ImmutableArray<CodeFixProvider>> fixersById = fixers
                .Where(f =>
                {
                    if (f.HasFixAllProvider(FixAllScope.Project))
                        return true;

                    if (Options.FixableOneByOneDiagnosticIds.Count > 0)
                    {
                        foreach (string diagnosticId in f.FixableDiagnosticIds)
                        {
                            if (Options.FixableOneByOneDiagnosticIds.Contains(diagnosticId))
                                return true;
                        }
                    }

                    return false;
                })
                .SelectMany(f => f.FixableDiagnosticIds.Select(id => (id, fixer: f)))
                .GroupBy(f => f.id)
                .ToDictionary(f => f.Key, g => g.Select(f => f.fixer).Distinct().ToImmutableArray());

            analyzers = analyzers
                .Where(analyzer => analyzer.SupportedDiagnostics.Any(descriptor => fixersById.ContainsKey(descriptor.Id)))
                .ToImmutableArray();

            if (!analyzers.Any())
                return new ProjectFixResult(ProjectFixKind.Success, analyzers: analyzers, fixers: fixers);

            Dictionary<string, ImmutableArray<DiagnosticAnalyzer>> analyzersById = analyzers
                .SelectMany(f => f.SupportedDiagnostics.Select(d => (id: d.Id, analyzer: f)))
                .GroupBy(f => f.id, f => f.analyzer)
                .ToDictionary(g => g.Key, g => g.Select(analyzer => analyzer).Distinct().ToImmutableArray());

            WriteAnalyzers(analyzers, ConsoleColor.DarkGray, Verbosity.Diagnostic);
            WriteFixers(fixers, ConsoleColor.DarkGray, Verbosity.Diagnostic);

            ImmutableHashSet<DiagnosticDescriptor>.Builder fixedDiagnostics = ImmutableHashSet.CreateBuilder(DiagnosticDescriptorComparer.Id);

            ImmutableArray<Diagnostic> previousDiagnostics = ImmutableArray<Diagnostic>.Empty;
            ImmutableArray<Diagnostic> previousPreviousDiagnostics = ImmutableArray<Diagnostic>.Empty;

            var fixKind = ProjectFixKind.Success;

            int iterationCount = 1;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                project = CurrentSolution.GetProject(project.Id);

                WriteLine($"  Compile '{project.Name}'{((iterationCount > 1) ? $" iteration {iterationCount}" : "")}", Verbosity.Normal);

                Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

                ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

                if (!VerifyCompilerDiagnostics(compilerDiagnostics, project))
                    return new ProjectFixResult(ProjectFixKind.CompilerError, fixedDiagnostics, analyzers: analyzers, fixers: fixers);

                WriteLine($"  Analyze '{project.Name}'", Verbosity.Normal);

                ImmutableArray<Diagnostic> diagnostics = await compilation.GetAnalyzerDiagnosticsAsync(analyzers, _defaultCompilationWithAnalyzersOptions, cancellationToken).ConfigureAwait(false);

                foreach (string message in diagnostics
                    .Where(f => f.IsAnalyzerExceptionDiagnostic())
                    .Select(f => f.ToString())
                    .Distinct())
                {
                    WriteLine(message, ConsoleColor.Yellow, Verbosity.Diagnostic);
                }

                IEnumerable<Diagnostic> fixableCompilerDiagnostics = compilerDiagnostics
                    .Where(f => f.Severity != DiagnosticSeverity.Error
                        && !Options.IgnoredCompilerDiagnosticIds.Contains(f.Id)
                        && fixersById.ContainsKey(f.Id));

                diagnostics = diagnostics
                    .Where(f => Options.IsSupportedDiagnostic(f)
                        && analyzersById.ContainsKey(f.Id)
                        && fixersById.ContainsKey(f.Id))
                    .Concat(fixableCompilerDiagnostics)
                    .ToImmutableArray();

                int length = diagnostics.Length;

                if (length == 0)
                    break;

                if (length == previousDiagnostics.Length
                    && !diagnostics.Except(previousDiagnostics, DiagnosticDeepEqualityComparer.Instance).Any())
                {
                    break;
                }

                if (length == previousPreviousDiagnostics.Length
                    && !diagnostics.Except(previousPreviousDiagnostics, DiagnosticDeepEqualityComparer.Instance).Any())
                {
                    WriteInfiniteFixLoop(diagnostics, previousDiagnostics, project, FormatProvider);

                    fixKind = ProjectFixKind.InfiniteLoop;
                    break;
                }

                WriteLine($"  Found {length} {((length == 1) ? "diagnostic" : "diagnostics")} in '{project.Name}'", Verbosity.Normal);

                Dictionary<DiagnosticDescriptor, int> diagnosticCountByDescriptor = diagnostics
                    .GroupBy(f => f.Descriptor, DiagnosticDescriptorComparer.Id)
                    .ToDictionary(f => f.Key, f => f.Count());

                //TODO: sort diagnostic ids
                foreach (DiagnosticDescriptor diagnosticDescriptor in diagnosticCountByDescriptor
                    .Select(f => f.Key)
                    .OrderBy(f => f, new DiagnosticDescriptorFixComparer(diagnosticCountByDescriptor, fixersById)))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string diagnosticId = diagnosticDescriptor.Id;

                    DiagnosticFixKind diagnosticFixKind = await FixDiagnosticsAsync(
                        diagnosticDescriptor,
                        CurrentSolution.GetProject(project.Id),
                        (diagnosticDescriptor.CustomTags.Contains(WellKnownDiagnosticTags.Compiler))
                            ? default(ImmutableArray<DiagnosticAnalyzer>)
                            : analyzersById[diagnosticId],
                        fixersById[diagnosticId],
                        cancellationToken).ConfigureAwait(false);

                    if (diagnosticFixKind == DiagnosticFixKind.Fixed)
                    {
                        fixedDiagnostics.Add(diagnosticDescriptor);
                    }
                    else if (diagnosticFixKind == DiagnosticFixKind.CompilerError)
                    {
                        return new ProjectFixResult(ProjectFixKind.CompilerError, fixedDiagnostics, analyzers: analyzers, fixers: fixers);
                    }
                }

                if (iterationCount == Options.MaxIterations)
                    break;

                previousPreviousDiagnostics = previousDiagnostics;
                previousDiagnostics = diagnostics;
                iterationCount++;
            }

            return new ProjectFixResult(fixKind, fixedDiagnostics, analyzers: analyzers, fixers: fixers);
        }

        private async Task<DiagnosticFixKind> FixDiagnosticsAsync(
            DiagnosticDescriptor diagnosticDescriptor,
            Project project,
            ImmutableArray<DiagnosticAnalyzer> analyzers,
            ImmutableArray<CodeFixProvider> fixers,
            CancellationToken cancellationToken)
        {
            ImmutableArray<Diagnostic> previousDiagnostics = ImmutableArray<Diagnostic>.Empty;

            var fixKind = DiagnosticFixKind.NotFixed;

            while (true)
            {
                Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

                ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

                if (!VerifyCompilerDiagnostics(compilerDiagnostics, project))
                    return DiagnosticFixKind.CompilerError;

                ImmutableArray<Diagnostic> diagnostics = default;

                if (analyzers.IsDefault)
                {
                    diagnostics = compilerDiagnostics;
                }
                else
                {
                    diagnostics = await compilation.GetAnalyzerDiagnosticsAsync(analyzers, _defaultCompilationWithAnalyzersOptions, cancellationToken).ConfigureAwait(false);
                }

                diagnostics = diagnostics
                    .Where(f => f.Id == diagnosticDescriptor.Id && f.Severity >= Options.MinimalSeverity)
                    .ToImmutableArray();

                int length = diagnostics.Length;

                if (length == 0)
                    break;

                if (length == previousDiagnostics.Length
                    && !diagnostics.Except(previousDiagnostics, DiagnosticDeepEqualityComparer.Instance).Any())
                {
                    break;
                }

                previousDiagnostics = diagnostics;

                if (Options.BatchSize > 0
                    && length > Options.BatchSize)
                {
                    diagnostics = ImmutableArray.CreateRange(diagnostics, 0, Options.BatchSize, f => f);
                }

                fixKind = await FixDiagnosticsAsync(diagnostics, diagnosticDescriptor, project, fixers, cancellationToken).ConfigureAwait(false);

                if (fixKind == DiagnosticFixKind.Fixed)
                {
                    if (Options.BatchSize <= 0
                        || length <= Options.BatchSize)
                    {
                        break;
                    }
                }
                else if (fixKind != DiagnosticFixKind.PartiallyFixed)
                {
                    break;
                }

                project = CurrentSolution.GetProject(project.Id);
            }

            return fixKind;
        }

        private async Task<DiagnosticFixKind> FixDiagnosticsAsync(
            ImmutableArray<Diagnostic> diagnostics,
            DiagnosticDescriptor diagnosticDescriptor,
            Project project,
            ImmutableArray<CodeFixProvider> fixers,
            CancellationToken cancellationToken)
        {
            string diagnosticId = diagnosticDescriptor.Id;

            WriteLine($"  Fix {diagnostics.Length} {diagnosticId} '{diagnosticDescriptor.Title}'", diagnostics[0].Severity.GetColor(), Verbosity.Normal);

            WriteDiagnostics(diagnostics, baseDirectoryPath: Path.GetDirectoryName(project.FilePath), formatProvider: FormatProvider, indentation: "    ", verbosity: Verbosity.Detailed);

            CodeFixProvider fixer = null;
            CodeAction codeAction = null;

            for (int i = 0; i < fixers.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                CodeAction codeActionCandidate = await GetFixAsync(
                    diagnostics,
                    diagnosticId,
                    project,
                    fixers[i],
                    cancellationToken).ConfigureAwait(false);

                if (codeActionCandidate != null)
                {
                    if (codeAction == null)
                    {
                        if (Options.DiagnosticFixerMap.IsEmpty
                            || !Options.DiagnosticFixerMap.TryGetValue(diagnosticId, out string fullTypeName)
                            || string.Equals(fixers[i].GetType().FullName, fullTypeName, StringComparison.Ordinal))
                        {
                            codeAction = codeActionCandidate;
                            fixer = fixers[i];
                        }
                    }
                    else if (Options.DiagnosticFixerMap.IsEmpty
                        || !Options.DiagnosticFixerMap.ContainsKey(diagnosticId))
                    {
                        WriteLine($"  Diagnostic '{diagnosticId}' is fixable with multiple fixers", ConsoleColor.Yellow, Verbosity.Diagnostic);
                        WriteLine($"    Fixer 1: '{fixer.GetType().FullName}'", ConsoleColor.Yellow, Verbosity.Diagnostic);
                        WriteLine($"    Fixer 2: '{fixers[i].GetType().FullName}'", ConsoleColor.Yellow, Verbosity.Diagnostic);
                        return DiagnosticFixKind.MultipleFixers;
                    }
                }
            }

            if (codeAction != null)
            {
                ImmutableArray<CodeActionOperation> operations = await codeAction.GetOperationsAsync(cancellationToken).ConfigureAwait(false);

                if (operations.Length == 1)
                {
                    operations[0].Apply(Workspace, cancellationToken);

                    return (diagnostics.Length != 1 && fixer.GetFixAllProvider() == null)
                        ? DiagnosticFixKind.PartiallyFixed
                        : DiagnosticFixKind.Fixed;
                }
                else if (operations.Length > 1)
                {
                    WriteLine($@"Code action has multiple operations
  Title:           {codeAction.Title}
  EquivalenceKey: {codeAction.EquivalenceKey}", ConsoleColor.Yellow, Verbosity.Diagnostic);
                }
            }

            return DiagnosticFixKind.NotFixed;
        }

        private async Task<CodeAction> GetFixAsync(
            ImmutableArray<Diagnostic> diagnostics,
            string diagnosticId,
            Project project,
            CodeFixProvider fixer,
            CancellationToken cancellationToken)
        {
            if (diagnostics.Length == 1)
                return await GetFixAsync(diagnostics[0], project, fixer, cancellationToken).ConfigureAwait(false);

            FixAllProvider fixAllProvider = fixer.GetFixAllProvider();

            if (fixAllProvider == null)
            {
                if (Options.FixableOneByOneDiagnosticIds.Contains(diagnosticId))
                    return await GetFixAsync(diagnostics[0], project, fixer, cancellationToken).ConfigureAwait(false);

                WriteLine($"  '{fixer.GetType().FullName}' does not have FixAllProvider", ConsoleColor.Yellow, Verbosity.Diagnostic);
                return null;
            }

            if (!fixAllProvider.GetSupportedFixAllDiagnosticIds(fixer).Any(f => f == diagnosticId))
            {
                WriteLine($"  '{fixAllProvider.GetType().FullName}' does not support diagnostic '{diagnosticId}'", ConsoleColor.Yellow, Verbosity.Diagnostic);
                return null;
            }

            if (!fixAllProvider.GetSupportedFixAllScopes().Any(f => f == FixAllScope.Project))
            {
                WriteLine($"  '{fixAllProvider.GetType().FullName}' does not support scope '{FixAllScope.Project}'", ConsoleColor.Yellow, Verbosity.Diagnostic);
                return null;
            }

            var multipleActionsInfos = new HashSet<MultipleActionsInfo>();

            CodeAction action = null;

            Options.DiagnosticFixMap.TryGetValue(diagnosticId, out string equivalenceKey);

            foreach (Diagnostic diagnostic in diagnostics)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!diagnostic.Location.IsInSource)
                    continue;

                Document document = project.GetDocument(diagnostic.Location.SourceTree);

                if (document == null)
                    continue;

                CodeAction action2 = await GetRegisteredFixAsync(diagnostic, document, fixer, multipleActionsInfos, cancellationToken).ConfigureAwait(false);

                if (action2 == null)
                    continue;

                if (equivalenceKey != null
                    && equivalenceKey != action2.EquivalenceKey)
                {
                    break;
                }

                action = action2;

                var fixAllContext = new FixAllContext(
                    document,
                    fixer,
                    FixAllScope.Project,
                    action.EquivalenceKey,
                    new string[] { diagnosticId },
                    new FixAllDiagnosticProvider(diagnostics),
                    cancellationToken);

                CodeAction fixAllAction = await fixAllProvider.GetFixAsync(fixAllContext).ConfigureAwait(false);

                if (fixAllAction != null)
                {
                    WriteLine($"  CodeFixProvider: '{fixer.GetType().FullName}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                    if (!string.IsNullOrEmpty(action.EquivalenceKey))
                        WriteLine($"  EquivalenceKey:  '{action.EquivalenceKey}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                    WriteLine($"  FixAllProvider:  '{fixAllProvider.GetType().FullName}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                    return fixAllAction;
                }

                WriteLine($"  Fixer '{fixer.GetType().FullName}' registered no action for diagnostic '{diagnosticId}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);
                WriteDiagnostics(diagnostics, baseDirectoryPath: Path.GetDirectoryName(project.FilePath), formatProvider: FormatProvider, indentation: "    ", maxCount: 10, verbosity: Verbosity.Diagnostic);
            }

            return null;
        }

        private async Task<CodeAction> GetFixAsync(
            Diagnostic diagnostic,
            Project project,
            CodeFixProvider fixer,
            CancellationToken cancellationToken)
        {
            if (!diagnostic.Location.IsInSource)
                return null;

            Document document = project.GetDocument(diagnostic.Location.SourceTree);

            Debug.Assert(document != null, "");

            if (document == null)
                return null;

            CodeAction action = await GetRegisteredFixAsync(diagnostic, document, fixer, default, cancellationToken).ConfigureAwait(false);

            if (action == null)
            {
                WriteLine($"  Fixer '{fixer.GetType().FullName}' registered no action for diagnostic '{diagnostic.Id}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);
                WriteDiagnostic(diagnostic, baseDirectoryPath: Path.GetDirectoryName(project.FilePath), formatProvider: FormatProvider, indentation: "    ", verbosity: Verbosity.Diagnostic);
            }

            return action;
        }

        private async Task<CodeAction> GetRegisteredFixAsync(
            Diagnostic diagnostic,
            Document document,
            CodeFixProvider fixer,
            HashSet<MultipleActionsInfo> multipleActionsInfos,
            CancellationToken cancellationToken)
        {
            CodeAction action = null;

            var context = new CodeFixContext(
                document,
                diagnostic,
                (a, _) =>
                {
                    if (action == null)
                    {
                        action = a;
                    }
                    else if (!string.Equals(a.EquivalenceKey, action.EquivalenceKey, StringComparison.Ordinal)
                        && (Options.DiagnosticFixMap.IsEmpty || !Options.DiagnosticFixMap.ContainsKey(diagnostic.Id)))
                    {
                        var multipleActionsInfo = new MultipleActionsInfo(diagnostic.Id, fixer, action.EquivalenceKey, a.EquivalenceKey);

                        if (multipleActionsInfos == null)
                            multipleActionsInfos = new HashSet<MultipleActionsInfo>();

                        if (multipleActionsInfos.Add(multipleActionsInfo))
                        {
                            WriteLine($"  '{fixer.GetType().FullName}' registered multiple actions to fix diagnostic '{diagnostic.Id}'", ConsoleColor.Yellow, Verbosity.Diagnostic);
                            WriteLine($"    EquivalenceKey 1: '{action.EquivalenceKey}'", ConsoleColor.Yellow, Verbosity.Diagnostic);
                            WriteLine($"    EquivalenceKey 2: '{a.EquivalenceKey}'", ConsoleColor.Yellow, Verbosity.Diagnostic);
                        }

                        action = null;
                    }
                },
                cancellationToken);

            await fixer.RegisterCodeFixesAsync(context).ConfigureAwait(false);

            return action;
        }

        private bool VerifyCompilerDiagnostics(ImmutableArray<Diagnostic> diagnostics, Project project)
        {
            const string indentation = "    ";

            using (IEnumerator<Diagnostic> en = diagnostics
                .Where(f => f.Severity == DiagnosticSeverity.Error
                    && !Options.IgnoredCompilerDiagnosticIds.Contains(f.Id))
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    Write(indentation);
                    WriteLine("Compilation errors:");

                    string baseDirectoryPath = Path.GetDirectoryName(project.FilePath);

                    const int maxCount = 10;

                    int count = 0;

                    do
                    {
                        count++;

                        if (count <= maxCount)
                        {
                            WriteDiagnostic(
                                en.Current,
                                baseDirectoryPath: baseDirectoryPath,
                                formatProvider: FormatProvider,
                                indentation: indentation,
                                verbosity: Verbosity.Normal);
                        }
                        else
                        {
                            break;
                        }
                    }
                    while (en.MoveNext());

                    count = 0;

                    bool plus = false;

                    while (en.MoveNext())
                    {
                        count++;

                        if (count == 1000)
                        {
                            plus = true;
                            break;
                        }
                    }

                    if (count > maxCount)
                    {
                        Write(indentation);
                        WriteLine($"and {count}{((plus) ? "+" : "")} more errors", verbosity: Verbosity.Normal);
                    }

                    if (!Options.IgnoreCompilerErrors)
                    {
#if DEBUG
                        Console.Write("Stop (Y/N)? ");

                        if (char.ToUpperInvariant((char)Console.Read()) == 'Y')
                            return false;
#else
                        return false;
#endif
                    }
                }
            }

            return true;
        }

        private async Task<ImmutableArray<DiagnosticDescriptor>> GetDiagnosticsAsync(
            ImmutableArray<DiagnosticAnalyzer> analyzers,
            IEnumerable<DiagnosticDescriptor> except,
            Compilation compilation,
            Func<(string id, DiagnosticAnalyzer analyzer), bool> predicate,
            CancellationToken cancellationToken)
        {
            Dictionary<string, ImmutableArray<DiagnosticAnalyzer>> analyzersById = analyzers
                .SelectMany(f => f.SupportedDiagnostics.Select(d => (id: d.Id, analyzer: f)))
                .Where(predicate)
                .GroupBy(f => f.id, f => f.analyzer)
                .ToDictionary(g => g.Key, g => g.Select(analyzer => analyzer).Distinct().ToImmutableArray());

            analyzers = analyzersById
                .SelectMany(f => f.Value)
                .Distinct()
                .ToImmutableArray();

            if (!analyzers.Any())
                return ImmutableArray<DiagnosticDescriptor>.Empty;

            ImmutableArray<Diagnostic> diagnostics = await compilation.GetAnalyzerDiagnosticsAsync(analyzers, _defaultCompilationWithAnalyzersOptions, cancellationToken).ConfigureAwait(false);

            return diagnostics
                .Where(f => Options.IsSupportedDiagnostic(f)
                    && analyzersById.ContainsKey(f.Id))
                .Select(f => f.Descriptor)
                .Except(except, DiagnosticDescriptorComparer.Id)
                .ToImmutableArray();
        }

        private async Task AddFileBannerAsync(
            Project project,
            ImmutableArray<string> banner,
            CancellationToken cancellationToken)
        {
            bool hasChanges = false;

            string solutionDirectory = Path.GetDirectoryName(project.Solution.FilePath);

            foreach (DocumentId documentId in project.DocumentIds)
            {
                Document document = project.GetDocument(documentId);

                if (GeneratedCodeUtility.IsGeneratedCodeFile(document.FilePath))
                    continue;

                SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                SyntaxFactsService syntaxFacts = SyntaxFactsService.GetService(project.Language);

                if (syntaxFacts.BeginsWithAutoGeneratedComment(root))
                    continue;

                if (syntaxFacts.BeginsWithBanner(root, banner))
                    continue;

                SyntaxTriviaList leading = root.GetLeadingTrivia();

                SyntaxTriviaList newLeading = leading.InsertRange(0, banner.SelectMany(f => syntaxFacts.ParseLeadingTrivia(syntaxFacts.SingleLineCommentStart + f + Environment.NewLine)));

                if (!syntaxFacts.IsEndOfLineTrivia(leading.LastOrDefault()))
                    newLeading = newLeading.AddRange(syntaxFacts.ParseLeadingTrivia(Environment.NewLine));

                SyntaxNode newRoot = root.WithLeadingTrivia(newLeading);

                Document newDocument = document.WithSyntaxRoot(newRoot);

                WriteLine($"  Add banner to '{PathUtilities.TrimStart(document.FilePath, solutionDirectory)}'", ConsoleColor.DarkGray, Verbosity.Detailed);

                project = newDocument.Project;

                hasChanges = true;
            }

            if (hasChanges
                && !Workspace.TryApplyChanges(project.Solution))
            {
                Debug.Fail($"Cannot apply changes to solution '{project.Solution.FilePath}'");
                WriteLine($"Cannot apply changes to solution '{project.Solution.FilePath}'", ConsoleColor.Yellow, Verbosity.Diagnostic);
            }
        }

        private async Task<int> FormatProjectAsync(Project project, CancellationToken cancellationToken)
        {
            WriteLine($"  Format  '{project.Name}'", Verbosity.Normal);

            Project newProject = await CodeFormatter.FormatProjectAsync(project, cancellationToken).ConfigureAwait(false);

            string solutionDirectory = Path.GetDirectoryName(project.Solution.FilePath);

            ImmutableArray<DocumentId> formattedDocuments = await CodeFormatter.GetFormattedDocumentsAsync(project, newProject).ConfigureAwait(false);

            WriteFormattedDocuments(formattedDocuments, project, solutionDirectory);

            if (formattedDocuments.Length > 0
                && !Workspace.TryApplyChanges(newProject.Solution))
            {
                Debug.Fail($"Cannot apply changes to solution '{newProject.Solution.FilePath}'");
                WriteLine($"Cannot apply changes to solution '{newProject.Solution.FilePath}'", ConsoleColor.Yellow, Verbosity.Diagnostic);
            }

            return formattedDocuments.Length;
        }

        private readonly struct MultipleActionsInfo : IEquatable<MultipleActionsInfo>
        {
            public MultipleActionsInfo(string diagnosticId, CodeFixProvider fixer, string equivalenceKey1, string equivalenceKey2)
            {
                DiagnosticId = diagnosticId;
                Fixer = fixer;
                EquivalenceKey1 = equivalenceKey1;
                EquivalenceKey2 = equivalenceKey2;
            }

            public string DiagnosticId { get; }
            public CodeFixProvider Fixer { get; }
            public string EquivalenceKey1 { get; }
            public string EquivalenceKey2 { get; }

            public override bool Equals(object obj)
            {
                return obj is MultipleActionsInfo other && Equals(other);
            }

            public bool Equals(MultipleActionsInfo other)
            {
                return DiagnosticId == other.DiagnosticId
                    && Fixer == other.Fixer
                    && EquivalenceKey1 == other.EquivalenceKey1
                    && EquivalenceKey2 == other.EquivalenceKey2;
            }

            public override int GetHashCode()
            {
                return Hash.Combine(DiagnosticId,
                    Hash.Combine(Fixer,
                    Hash.Combine(EquivalenceKey1,
                    Hash.Create(EquivalenceKey2))));
            }

            public static bool operator ==(in MultipleActionsInfo info1, in MultipleActionsInfo info2)
            {
                return info1.Equals(info2);
            }

            public static bool operator !=(in MultipleActionsInfo info1, in MultipleActionsInfo info2)
            {
                return !(info1 == info2);
            }
        }
    }
}
