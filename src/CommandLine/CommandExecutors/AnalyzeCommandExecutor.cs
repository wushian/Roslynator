// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Diagnostics;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal class AnalyzeCommandExecutor : MSBuildWorkspaceCommandExecutor
    {
        private static ImmutableArray<string> _roslynatorAnalyzerAssemblies;

        public AnalyzeCommandExecutor(AnalyzeCommandLineOptions options, DiagnosticSeverity minimalSeverity, string language) : base(language)
        {
            Options = options;
            MinimalSeverity = minimalSeverity;
        }

        public AnalyzeCommandLineOptions Options { get; }

        public DiagnosticSeverity MinimalSeverity { get; }

        public static ImmutableArray<string> RoslynatorAnalyzerAssemblies
        {
            get
            {
                if (_roslynatorAnalyzerAssemblies.IsDefault)
                {
                    _roslynatorAnalyzerAssemblies = ImmutableArray.Create(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Roslynator.CSharp.Analyzers.dll"));
                }

                return _roslynatorAnalyzerAssemblies;
            }
        }

        public override async Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
        {
            AssemblyResolver.Register();

            var codeAnalyzerOptions = new CodeAnalyzerOptions(
                ignoreAnalyzerReferences: Options.IgnoreAnalyzerReferences,
                ignoreCompilerDiagnostics: Options.IgnoreCompilerDiagnostics,
                reportFadeDiagnostics: Options.ReportFadeDiagnostics,
                reportSuppressedDiagnostics: Options.ReportSuppressedDiagnostics,
                executionTime: Options.ExecutionTime,
                minimalSeverity: MinimalSeverity,
                supportedDiagnosticIds: Options.SupportedDiagnostics,
                ignoredDiagnosticIds: Options.IgnoredDiagnostics,
                projectNames: Options.Projects,
                ignoredProjectNames: Options.IgnoredProjects,
                language: Language);

            IEnumerable<string> analyzerAssemblies = Options.AnalyzerAssemblies;

            if (Options.UseRoslynatorAnalyzers)
                analyzerAssemblies = analyzerAssemblies.Concat(RoslynatorAnalyzerAssemblies);

            CultureInfo culture = (Options.CultureName != null) ? CultureInfo.GetCultureInfo(Options.CultureName) : null;

            var codeAnalyzer = new CodeAnalyzer(analyzerAssemblies: analyzerAssemblies, formatProvider: culture, options: codeAnalyzerOptions);

            if (projectOrSolution.IsProject)
            {
                Project project = projectOrSolution.AsProject();

                WriteLine($"Analyze project '{project.Name}'", Verbosity.Minimal);

                ProjectAnalysisResult result = await codeAnalyzer.AnalyzeProjectAsync(project, cancellationToken);

                if (Options.XmlFileLog != null
                    && result.Diagnostics.Any())
                {
                    DiagnosticXmlSerializer.Serialize(result, project, Options.XmlFileLog, culture);
                }
            }
            else
            {
                Solution solution = projectOrSolution.AsSolution();

                ImmutableArray<ProjectAnalysisResult> results = await codeAnalyzer.AnalyzeSolutionAsync(solution, cancellationToken);

                if (Options.XmlFileLog != null
                    && results.Any(f => f.Diagnostics.Any()))
                {
                    DiagnosticXmlSerializer.Serialize(results, solution, Options.XmlFileLog, culture);
                }
            }

            return new CommandResult(true);
        }

        protected override void OperationCanceled(OperationCanceledException ex)
        {
            WriteLine("Analysis was canceled.", Verbosity.Quiet);
        }
    }
}
