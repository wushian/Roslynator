// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Telemetry;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.Text;
using static Roslynator.ConsoleHelpers;

namespace Roslynator.Analysis
{
    public class CodeAnalyzer
    {
        private readonly AnalyzerAssemblyCache _analyzerAssemblies;

        private readonly AnalyzerAssemblyCache _analyzerAssemblyCache = new AnalyzerAssemblyCache();

        private static readonly TimeSpan _minimalExecutionTime = TimeSpan.FromMilliseconds(1);

        private static readonly CompilationWithAnalyzersOptions _ignoreSuppressedDiagnosticsCompilationWithAnalyzersOptions = new CompilationWithAnalyzersOptions(
            options: default(AnalyzerOptions),
            onAnalyzerException: default(Action<Exception, DiagnosticAnalyzer, Diagnostic>),
            concurrentAnalysis: true,
            logAnalyzerExecutionTime: true,
            reportSuppressedDiagnostics: false);

        private static readonly CompilationWithAnalyzersOptions _reportSuppressedDiagnosticsCompilationWithAnalyzersOptions = new CompilationWithAnalyzersOptions(
            options: default(AnalyzerOptions),
            onAnalyzerException: default(Action<Exception, DiagnosticAnalyzer, Diagnostic>),
            concurrentAnalysis: true,
            logAnalyzerExecutionTime: true,
            reportSuppressedDiagnostics: true);

        public CodeAnalyzer(IEnumerable<string> analyzerAssemblies = null, IFormatProvider formatProvider = null, CodeAnalyzerOptions options = null)
        {
            Options = options ?? CodeAnalyzerOptions.Default;

            _analyzerAssemblies = new AnalyzerAssemblyCache();

            if (analyzerAssemblies != null)
                _analyzerAssemblies.LoadFrom(analyzerAssemblies, loadFixers: false);

            FormatProvider = formatProvider;
        }

        public CodeAnalyzerOptions Options { get; }

        public IFormatProvider FormatProvider { get; }

        public async Task AnalyzeSolutionAsync(Solution solution, CancellationToken cancellationToken = default)
        {
            ImmutableArray<ProjectId> projectIds = solution
                .GetProjectDependencyGraph()
                .GetTopologicallySortedProjects(cancellationToken)
                .ToImmutableArray();

            foreach (string id in Options.IgnoredDiagnosticIds.OrderBy(f => f))
                WriteLine($"Ignore diagnostic '{id}'");

            WriteLine($"Analyze solution '{solution.FilePath}'");

            var results = new List<ProjectAnalysisResult>();

            Stopwatch stopwatch = Stopwatch.StartNew();

            TimeSpan lastElapsed = TimeSpan.Zero;

            for (int i = 0; i < projectIds.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Project project = solution.GetProject(projectIds[i]);

                if (Options.IgnoredProjectNames.Contains(project.Name)
                    || (Options.Language != null && Options.Language != project.Language))
                {
                    WriteLine($"Skip project {$"{i + 1}/{projectIds.Length}"} '{project.Name}' ({project.FilePath})", ConsoleColor.DarkGray);
                }
                else
                {
                    WriteLine($"Analyze project {$"{i + 1}/{projectIds.Length}"} '{project.Name}' ({project.FilePath})");

                    ProjectAnalysisResult result = await AnalyzeProjectAsync(project, cancellationToken).ConfigureAwait(false);

                    if (result != null)
                        results.Add(result);
                }

                lastElapsed = stopwatch.Elapsed;
            }

            stopwatch.Stop();

            if (results.Count > 0)
            {
                if (Options.ExecutionTime)
                {
                    var telemetryInfos = new Dictionary<DiagnosticAnalyzer, AnalyzerTelemetryInfo>();

                    foreach (ProjectAnalysisResult result in results)
                    {
                        foreach (KeyValuePair<DiagnosticAnalyzer, AnalyzerTelemetryInfo> kvp in result.Telemetry)
                        {
                            DiagnosticAnalyzer analyzer = kvp.Key;

                            if (!telemetryInfos.TryGetValue(analyzer, out AnalyzerTelemetryInfo telemetryInfo))
                                telemetryInfo = new AnalyzerTelemetryInfo();

                            telemetryInfo.Add(kvp.Value);

                            telemetryInfos[analyzer] = telemetryInfo;
                        }
                    }

                    WriteLine();

                    foreach (KeyValuePair<DiagnosticAnalyzer, AnalyzerTelemetryInfo> kvp in telemetryInfos
                        .Where(f => f.Value.ExecutionTime >= _minimalExecutionTime)
                        .OrderByDescending(f => f.Value.ExecutionTime))
                    {
                        WriteLine($"{kvp.Value.ExecutionTime:mm\\:ss\\.fff} '{kvp.Key.GetType().FullName}'");
                    }
                }

                int totalCount = 0;

                foreach (ProjectAnalysisResult result in results)
                {
                    IEnumerable<Diagnostic> diagnostics = result.Diagnostics
                        .Where(f => !f.IsAnalyzerExceptionDiagnostic())
                        .Concat(result.CompilerDiagnostics);

                    totalCount += FilterDiagnostics(diagnostics, cancellationToken).Count();
                }

                if (totalCount > 0)
                {
                    WriteLine();
                    WriteLine($"{totalCount} {((totalCount == 1) ? "diagnostic" : "diagnostics")} found in solution '{solution.FilePath}'.");

                    Dictionary<DiagnosticDescriptor, int> diagnosticsByDescriptor = results
                        .SelectMany(f => FilterDiagnostics(f.Diagnostics.Concat(f.CompilerDiagnostics), cancellationToken))
                        .GroupBy(f => f.Descriptor, DiagnosticDescriptorComparer.Id)
                        .ToDictionary(f => f.Key, f => f.Count());

                    int maxCountLength = Math.Max(totalCount.ToString().Length, diagnosticsByDescriptor.Max(f => f.Value.ToString().Length));
                    int maxIdLength = diagnosticsByDescriptor.Max(f => f.Key.Id.Length);

                    foreach (KeyValuePair<DiagnosticDescriptor, int> kvp in diagnosticsByDescriptor.OrderBy(f => f.Key.Id))
                    {
                        WriteLine($"{kvp.Value.ToString().PadLeft(maxCountLength)} {kvp.Key.Id.PadRight(maxIdLength)} {kvp.Key.Title}");
                    }

                    WriteLine();
                }
            }

            WriteLine($"Done analyzing solution {stopwatch.Elapsed:mm\\:ss\\.ff} '{solution.FilePath}'", ConsoleColor.Green);
        }

        public async Task<ProjectAnalysisResult> AnalyzeProjectAsync(Project project, CancellationToken cancellationToken = default)
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
                .AddRange(_analyzerAssemblyCache.GetAnalyzers(assemblies, language));

            if (!analyzers.Any())
            {
                WriteLine($"  No analyzers found to analyze '{project.Name}'", ConsoleColor.DarkGray);
                return default;
            }

            cancellationToken.ThrowIfCancellationRequested();

            Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = (Options.IgnoreCompilerDiagnostics)
                ? ImmutableArray<Diagnostic>.Empty
                : compilation.GetDiagnostics(cancellationToken);

            CompilationWithAnalyzersOptions compilationWithAnalyzersOptions = (Options.ReportSuppressedDiagnostics)
                ? _reportSuppressedDiagnosticsCompilationWithAnalyzersOptions
                : _ignoreSuppressedDiagnosticsCompilationWithAnalyzersOptions;

            var compilationWithAnalyzers = new CompilationWithAnalyzers(compilation, analyzers, compilationWithAnalyzersOptions);

            ImmutableArray<Diagnostic> diagnostics = default;
            ImmutableDictionary<DiagnosticAnalyzer, AnalyzerTelemetryInfo> telemetry = ImmutableDictionary<DiagnosticAnalyzer, AnalyzerTelemetryInfo>.Empty;

            if (Options.ExecutionTime)
            {
                AnalysisResult analysisResult = await compilationWithAnalyzers.GetAnalysisResultAsync(cancellationToken).ConfigureAwait(false);

                diagnostics = analysisResult.GetAllDiagnostics();
                telemetry = analysisResult.AnalyzerTelemetryInfo;
            }
            else
            {
                diagnostics = await compilationWithAnalyzers.GetAllDiagnosticsAsync(cancellationToken).ConfigureAwait(false);
            }

            WriteDiagnostics(FilterDiagnostics(diagnostics.Where(f => f.IsAnalyzerExceptionDiagnostic()), cancellationToken), project);

            IEnumerable<Diagnostic> allDiagnostics = diagnostics
                .Where(f => !f.IsAnalyzerExceptionDiagnostic())
                .Concat(compilerDiagnostics);

            WriteDiagnostics(FilterDiagnostics(allDiagnostics, cancellationToken), project);

            return new ProjectAnalysisResult(project, analyzers, diagnostics, compilerDiagnostics, telemetry);
        }

        private void WriteDiagnostics(IEnumerable<Diagnostic> diagnostics, Project project)
        {
            string projectDirectory = Path.GetDirectoryName(project.FilePath);

            foreach (IGrouping<SyntaxTree, Diagnostic> grouping in diagnostics
                .OrderBy(f => f.Id)
                .GroupBy(f => f.Location.SourceTree)
                .OrderBy(f => f.Key.FilePath))
            {
                foreach (Diagnostic diagnostic in grouping.OrderBy(f => f.Location.SourceSpan.Start))
                {
                    string message = FormatDiagnostic(diagnostic, baseDirectoryPath: projectDirectory);

                    WriteLine($"  {message}", GetColor(diagnostic.Severity));
                }
            }

            ConsoleColor GetColor(DiagnosticSeverity diagnosticSeverity)
            {
                switch (diagnosticSeverity)
                {
                    case DiagnosticSeverity.Hidden:
                        return ConsoleColor.DarkGray;
                    case DiagnosticSeverity.Info:
                        return ConsoleColor.Cyan;
                    case DiagnosticSeverity.Warning:
                        return ConsoleColor.Yellow;
                    case DiagnosticSeverity.Error:
                        return ConsoleColor.Red;
                    default:
                        throw new InvalidOperationException($"Unknown diagnostic severity '{diagnosticSeverity}'.");
                }
            }
        }

        private IEnumerable<Diagnostic> FilterDiagnostics(IEnumerable<Diagnostic> diagnostics, CancellationToken cancellationToken = default)
        {
            foreach (Diagnostic diagnostic in diagnostics)
            {
                if (diagnostic.Severity >= Options.MinimalSeverity
                    && !Options.IgnoredDiagnosticIds.Contains(diagnostic.Id)
                    && (Options.ReportFadeDiagnostics || !diagnostic.IsFadeDiagnostic()))
                {
                    if (diagnostic.Descriptor.CustomTags.Contains(WellKnownDiagnosticTags.Compiler))
                    {
                        Debug.Assert(diagnostic.Id.StartsWith("CS", StringComparison.Ordinal)
                            || diagnostic.Id.StartsWith("VB", StringComparison.Ordinal), diagnostic.Id);

                        SyntaxTree tree = diagnostic.Location.SourceTree;

                        if (tree == null
                            || !GeneratedCodeUtility.IsGeneratedCode(tree, f => IsComment(f), cancellationToken))
                        {
                            yield return diagnostic;
                        }
                    }
                    else
                    {
                        yield return diagnostic;
                    }
                }
            }

            bool IsComment(in SyntaxTrivia trivia)
            {
                switch (trivia.Language)
                {
                    case LanguageNames.CSharp:
                        return trivia.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.SingleLineCommentTrivia, Microsoft.CodeAnalysis.CSharp.SyntaxKind.MultiLineCommentTrivia);
                    case LanguageNames.VisualBasic:
                        return trivia.IsKind(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommentTrivia);
                    default:
                        return false;
                }
            }
        }

        private string FormatDiagnostic(
           Diagnostic diagnostic,
           string baseDirectoryPath = null)
        {
            StringBuilder sb = StringBuilderCache.GetInstance();

            sb.Append(diagnostic.Id);
            sb.Append(" ");
            sb.Append(diagnostic.GetMessage(FormatProvider));
            sb.Append(" ");
            sb.Append(GetSeverity(diagnostic.Severity));

            switch (diagnostic.Location.Kind)
            {
                case LocationKind.SourceFile:
                case LocationKind.XmlFile:
                case LocationKind.ExternalFile:
                    {
                        FileLinePositionSpan span = diagnostic.Location.GetMappedLineSpan();

                        if (span.IsValid)
                        {
                            string path = span.Path;

                            if (baseDirectoryPath != null
                                && path.StartsWith(baseDirectoryPath))
                            {
                                path = path.Remove(0, baseDirectoryPath.Length).TrimStart(Path.DirectorySeparatorChar);
                            }

                            sb.Append(" ");
                            sb.Append(path);

                            LinePosition linePosition = span.Span.Start;

                            sb.Append("(");
                            sb.Append(linePosition.Line + 1);
                            sb.Append(",");
                            sb.Append(linePosition.Character + 1);
                            sb.Append(")");
                        }

                        break;
                    }
            }

            return StringBuilderCache.GetStringAndFree(sb);

            string GetSeverity(DiagnosticSeverity severity)
            {
                switch (severity)
                {
                    case DiagnosticSeverity.Hidden:
                        return "hidden";
                    case DiagnosticSeverity.Info:
                        return "info";
                    case DiagnosticSeverity.Warning:
                        return "warning";
                    case DiagnosticSeverity.Error:
                        return "error";
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }
}
