// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Analysis;
using Roslynator.Formatting;
using static Roslynator.ConsoleHelpers;

namespace Roslynator.CodeFixes
{
    //TODO: fix compiler diagnostics
    public class CodeFixer
    {
        private readonly AnalyzerAssemblyCache _analyzerAssemblies;

        private readonly AnalyzerAssemblyCache _analyzerAssemblyCache = new AnalyzerAssemblyCache();

        private static readonly CompilationWithAnalyzersOptions _defaultCompilationWithAnalyzersOptions = new CompilationWithAnalyzersOptions(
            options: default(AnalyzerOptions),
            onAnalyzerException: null,
            concurrentAnalysis: true,
            logAnalyzerExecutionTime: false,
            reportSuppressedDiagnostics: false);

        public CodeFixer(Solution solution, IEnumerable<string> analyzerAssemblies = null, CodeFixerOptions options = null)
        {
            Workspace = solution.Workspace;
            Options = options ?? CodeFixerOptions.Default;

            _analyzerAssemblies = new AnalyzerAssemblyCache();

            if (analyzerAssemblies != null)
                _analyzerAssemblies.LoadFrom(analyzerAssemblies);
        }

        public Workspace Workspace { get; }

        public CodeFixerOptions Options { get; }

        private Solution CurrentSolution => Workspace.CurrentSolution;

        public async Task FixSolutionAsync(CancellationToken cancellationToken = default)
        {
            ImmutableArray<ProjectId> projects = CurrentSolution
                .GetProjectDependencyGraph()
                .GetTopologicallySortedProjects(cancellationToken)
                .ToImmutableArray();

            foreach (string id in Options.IgnoredDiagnosticIds.OrderBy(f => f))
                WriteLine($"Ignore diagnostic '{id}'");

            foreach (string id in Options.IgnoredCompilerDiagnosticIds.OrderBy(f => f))
                WriteLine($"Ignore compiler diagnostic '{id}'");

            var results = new List<ProjectFixResult>();

            Stopwatch stopwatch = Stopwatch.StartNew();

            TimeSpan lastElapsed = TimeSpan.Zero;

            for (int i = 0; i < projects.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Project project = CurrentSolution.GetProject(projects[i]);

                if (Options.IgnoredProjectNames.Contains(project.Name)
                    || (Options.Language != null && Options.Language != project.Language))
                {
                    WriteLine($"Skip project {$"{i + 1}/{projects.Length}"} '{project.Name}'", ConsoleColor.DarkGray);

                    results.Add(ProjectFixResult.Skipped);
                }
                else
                {
                    WriteLine($"Fix project {$"{i + 1}/{projects.Length}"} '{project.Name}'", ConsoleColor.Cyan);

                    ProjectFixResult result = await FixProjectAsync(project, cancellationToken).ConfigureAwait(false);

                    results.Add(result);

                    if (result.Result == FixResult.CompilerError)
                        break;

                    if (Options.Format)
                    {
                        project = CurrentSolution.GetProject(project.Id);

                        WriteLine($"  Format  '{project.Name}'");

                        Project newProject = await CodeFormatter.FormatProjectAsync(project, cancellationToken).ConfigureAwait(false);

                        bool success = Workspace.TryApplyChanges(newProject.Solution);

                        Debug.Assert(success, "Cannot apply changes to a solution.");
                    }
                }

                TimeSpan elapsed = stopwatch.Elapsed;

                WriteLine($"Done fixing project {$"{i + 1}/{projects.Length}"} {elapsed - lastElapsed:mm\\:ss\\.ff} '{project.Name}'", ConsoleColor.Green);

                lastElapsed = elapsed;
            }

            stopwatch.Stop();

            WriteLine();
            WriteLine("Fixed diagnostics:");

            IEnumerable<DiagnosticDescriptor> diagnosticDescriptors = results
                .SelectMany(f => f.Analyzers)
                .Distinct()
                .SelectMany(f => f.SupportedDiagnostics)
                .Distinct(DiagnosticDescriptorComparer.Id);

            foreach (DiagnosticDescriptor diagnosticDescriptor in results
                .SelectMany(f => f.FixedDiagnosticIds)
                .Distinct()
                .Join(diagnosticDescriptors, id => id, d => d.Id, (_, d) => d)
                .OrderBy(f => f.Id))
            {
                WriteLine($"  {diagnosticDescriptor.Id} '{diagnosticDescriptor.Title}'");
            }

            WriteLine();
            WriteLine($"Done fixing solution {stopwatch.Elapsed:mm\\:ss\\.ff} '{CurrentSolution.FilePath}'", ConsoleColor.Green);
        }

        public async Task<ProjectFixResult> FixProjectAsync(Project project, CancellationToken cancellationToken = default)
        {
            string language = project.Language;

            ImmutableArray<Assembly> assemblies = (Options.IgnoreAnalyzerReferences) ? ImmutableArray<Assembly>.Empty : project.AnalyzerReferences
                .Distinct()
                .OfType<AnalyzerFileReference>()
                .Select(f => f.GetAssembly())
                .Where(f => !_analyzerAssemblies.Contains(f.FullName))
                .ToImmutableArray();

            ImmutableArray<DiagnosticAnalyzer> analyzers = _analyzerAssemblies
                .GetAnalyzers(language)
                .AddRange(_analyzerAssemblyCache.GetAnalyzers(assemblies, language))
                .Where(a =>
                 {
                     return a.SupportedDiagnostics
                         .Any(d =>
                         {
                             ReportDiagnostic reportDiagnostic = d.GetEffectiveSeverity(project.CompilationOptions);

                             return reportDiagnostic != ReportDiagnostic.Suppress
                                 && reportDiagnostic.ToDiagnosticSeverity() >= Options.MinimalSeverity;
                         });
                 })
                 .ToImmutableArray();

            if (!analyzers.Any())
            {
                WriteLine($"  No analyzers found to analyze '{project.Name}'", ConsoleColor.DarkGray);
                return ProjectFixResult.NoAnalyzers;
            }

            ImmutableArray<CodeFixProvider> fixers = _analyzerAssemblies
                .GetFixers(language)
                .AddRange(_analyzerAssemblyCache.GetFixers(assemblies, language));

            if (!fixers.Any())
            {
                WriteLine($"  No fixers found to fix '{project.Name}'", ConsoleColor.DarkGray);
                return new ProjectFixResult(ImmutableArray<string>.Empty, analyzers, fixers, FixResult.NoFixers);
            }

            Dictionary<string, ImmutableArray<DiagnosticAnalyzer>> analyzersById = analyzers
                .SelectMany(f => f.SupportedDiagnostics.Select(d => (id: d.Id, analyzer: f)))
                .GroupBy(f => f.id, f => f.analyzer)
                .ToDictionary(g => g.Key, g => g.Select(analyzer => analyzer).Distinct().ToImmutableArray());

            Dictionary<string, ImmutableArray<CodeFixProvider>> fixersById = fixers
                .Where(f => f.GetFixAllProvider() != null)
                .SelectMany(f => f.FixableDiagnosticIds.Select(id => (id, fixer: f)))
                .GroupBy(f => f.id)
                .ToDictionary(f => f.Key, g => g.Select(f => f.fixer).ToImmutableArray());

            ImmutableHashSet<string>.Builder diagnosticIds = ImmutableHashSet.CreateBuilder<string>(StringComparer.Ordinal);

            ImmutableArray<Diagnostic> previousDiagnostics = ImmutableArray<Diagnostic>.Empty;

            int iterationCount = 1;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                project = CurrentSolution.GetProject(project.Id);

                WriteLine($"  Compile '{project.Name}'{((iterationCount > 1) ? $" iteration {iterationCount}" : "")}");

                Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

                if (!VerifyCompilerDiagnostics(compilation, cancellationToken))
                    return new ProjectFixResult(diagnosticIds.ToImmutableArray(), analyzers, fixers, FixResult.CompilerError);

                var compilationWithAnalyzers = new CompilationWithAnalyzers(compilation, analyzers, _defaultCompilationWithAnalyzersOptions);

                WriteLine($"  Analyze '{project.Name}' with {analyzers.Length} analyzers");

                ImmutableArray<Diagnostic> diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(cancellationToken).ConfigureAwait(false);

                foreach (string message in diagnostics
                    .Where(f => f.IsAnalyzerExceptionDiagnostic())
                    .Select(f => f.ToString())
                    .Distinct())
                {
                    WriteLine(message, ConsoleColor.Yellow);
                }

                diagnostics = diagnostics
                    .Where(f => f.Severity >= Options.MinimalSeverity
                        && analyzersById.ContainsKey(f.Id)
                        && fixersById.ContainsKey(f.Id)
                        && !Options.IgnoredDiagnosticIds.Contains(f.Id))
                    .ToImmutableArray();

                int length = diagnostics.Length;

                if (length == 0)
                    break;

                if (length == previousDiagnostics.Length
                    && !diagnostics.Except(previousDiagnostics, DiagnosticDeepEqualityComparer.Instance).Any())
                {
                    break;
                }

                WriteLine($"  Found {length} {((length == 1) ? "diagnostic" : "diagnostics")} in '{project.Name}'");

                foreach (string diagnosticId in diagnostics
                    .Select(f => f.Id)
                    .Distinct()
                    .OrderBy(f => f))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    FixResult result = await FixDiagnosticsAsync(
                        diagnosticId,
                        CurrentSolution.GetProject(project.Id),
                        analyzersById[diagnosticId],
                        fixersById[diagnosticId],
                        cancellationToken).ConfigureAwait(false);

                    diagnosticIds.Add(diagnosticId);

                    if (result == FixResult.CompilerError)
                        return new ProjectFixResult(diagnosticIds.ToImmutableArray(), analyzers, fixers, FixResult.CompilerError);
                }

                previousDiagnostics = diagnostics;
                iterationCount++;
            }

            return new ProjectFixResult(diagnosticIds.ToImmutableArray(), analyzers, fixers, FixResult.Success);
        }

        private async Task<FixResult> FixDiagnosticsAsync(
            string diagnosticId,
            Project project,
            ImmutableArray<DiagnosticAnalyzer> analyzers,
            ImmutableArray<CodeFixProvider> fixers,
            CancellationToken cancellationToken)
        {
            ImmutableArray<Diagnostic> previousDiagnostics = ImmutableArray<Diagnostic>.Empty;

            while (true)
            {
                Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

                if (!VerifyCompilerDiagnostics(compilation, cancellationToken))
                    return FixResult.CompilerError;

                var compilationWithAnalyzers = new CompilationWithAnalyzers(compilation, analyzers, _defaultCompilationWithAnalyzersOptions);

                ImmutableArray<Diagnostic> diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(cancellationToken).ConfigureAwait(false);

                diagnostics = diagnostics
                    .Where(f => f.Id == diagnosticId && f.Severity >= Options.MinimalSeverity)
                    .ToImmutableArray();

                int length = diagnostics.Length;

                if (length == 0)
                    return FixResult.Success;

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

                await FixDiagnosticsAsync(diagnosticId, project, diagnostics, fixers, cancellationToken).ConfigureAwait(false);

                if (Options.BatchSize <= 0
                    || length <= Options.BatchSize)
                {
                    break;
                }

                project = CurrentSolution.GetProject(project.Id);
            }

            return FixResult.Success;
        }

        private async Task FixDiagnosticsAsync(
            string diagnosticId,
            Project project,
            ImmutableArray<Diagnostic> diagnostics,
            ImmutableArray<CodeFixProvider> fixers,
            CancellationToken cancellationToken)
        {
            WriteLine($"  Fix {diagnostics.Length,4} {diagnosticId,10} '{diagnostics[0].Descriptor.Title}'");

            if (Options.BatchSize == 1)
                WriteLine($"  {diagnostics[0]}", ConsoleColor.DarkGray);

            CodeFixProvider fixer = null;
            CodeAction codeAction = null;

            for (int i = 0; i < fixers.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                CodeAction codeAction2 = await GetFixAsync(
                    diagnosticId,
                    project,
                    diagnostics,
                    fixers[i],
                    cancellationToken).ConfigureAwait(false);

                if (codeAction2 != null)
                {
                    if (codeAction == null)
                    {
                        codeAction = codeAction2;
                        fixer = fixers[i];
                    }
                    else
                    {
#if DEBUG
                        WriteLine($"Diagnostic '{diagnosticId}' is fixable by multiple fixers", ConsoleColor.DarkYellow);
                        WriteLine($"  {fixer.GetType().Name}", ConsoleColor.DarkYellow);
                        WriteLine($"  {fixers[i].GetType().Name}", ConsoleColor.DarkYellow);
#endif
                        codeAction = null;
                        break;
                    }
                }
            }

            if (codeAction != null)
            {
                ImmutableArray<CodeActionOperation> operations = await codeAction.GetOperationsAsync(cancellationToken).ConfigureAwait(false);

                WriteLineIf(operations.Length > 1, $@"Code action has multiple operations
Title: {codeAction.Title}
Equivalence key: {codeAction.EquivalenceKey}", ConsoleColor.Magenta);

                if (operations.Length == 1)
                    operations[0].Apply(Workspace, cancellationToken);
            }
        }

        private static async Task<CodeAction> GetFixAsync(
            string diagnosticId,
            Project project,
            ImmutableArray<Diagnostic> diagnostics,
            CodeFixProvider fixer,
            CancellationToken cancellationToken)
        {
            FixAllProvider fixAll = fixer.GetFixAllProvider();

            if (!fixAll.GetSupportedFixAllDiagnosticIds(fixer).Any(f => f == diagnosticId))
                return null;

            if (!fixAll.GetSupportedFixAllScopes().Any(f => f == FixAllScope.Project))
                return null;

            foreach (Diagnostic diagnostic in diagnostics)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!diagnostic.Location.IsInSource)
                    continue;

                Document document = project.GetDocument(diagnostic.Location.SourceTree);

                Debug.Assert(document != null, "");

                if (document == null)
                    continue;

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
                        else if (!string.Equals(a.EquivalenceKey, action.EquivalenceKey, StringComparison.Ordinal))
                        {
#if DEBUG
                            WriteLine($"'{fixer.GetType().Name}' registered multiple actions for diagnostic '{diagnosticId}'", ConsoleColor.DarkYellow);
                            WriteLine($"  {action.EquivalenceKey}", ConsoleColor.DarkYellow);
                            WriteLine($"  {a.EquivalenceKey}", ConsoleColor.DarkYellow);
#endif
                            action = null;
                        }
                    },
                    cancellationToken);

                await fixer.RegisterCodeFixesAsync(context).ConfigureAwait(false);

                if (action == null)
                    continue;

                var fixAllContext = new FixAllContext(
                    document,
                    fixer,
                    FixAllScope.Project,
                    action.EquivalenceKey,
                    new string[] { diagnosticId },
                    new FixAllDiagnosticProvider(diagnostics),
                    cancellationToken);

                CodeAction fixAllAction = await fixAll.GetFixAsync(fixAllContext).ConfigureAwait(false);

                if (fixAllAction == null && diagnosticId.StartsWith("RCS"))
                {
                    WriteLine($"'{fixer.GetType().FullName}' registered no action for diagnostics:", ConsoleColor.Magenta);
                    Write(diagnostics, 10, ConsoleColor.Magenta);
                }

                return fixAllAction;
            }

            return null;
        }

        private bool VerifyCompilerDiagnostics(Compilation compilation, CancellationToken cancellationToken)
        {
            ImmutableArray<Diagnostic> diagnostics = compilation.GetDiagnostics(cancellationToken);

            using (IEnumerator<Diagnostic> en = diagnostics
                .Where(f => f.Severity == DiagnosticSeverity.Error
                    && !Options.IgnoredCompilerDiagnosticIds.Contains(f.Id))
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    const int maxCount = 10;

                    int count = 0;

                    do
                    {
                        count++;

                        if (count <= maxCount)
                        {
                            WriteLine(en.Current.ToString(), ConsoleColor.Red);
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
                        WriteLine($"and {count}{((plus) ? "+" : "")} more diagnostics", ConsoleColor.Red);
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
    }
}
