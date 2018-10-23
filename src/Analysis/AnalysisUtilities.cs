// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Analysis;

namespace Roslynator
{
    internal static class AnalysisUtilities
    {
        public static ImmutableArray<DiagnosticAnalyzer>GetAnalyzers(
            Project project,
            AnalyzerAssemblyList analyzerAssemblies,
            AnalyzerAssemblyList analyzerReferences,
            bool ignoreAnalyzerReferences = false,
            DiagnosticSeverity minimalSeverity = DiagnosticSeverity.Info)
        {
            return GetAnalyzersAndFixers(
                project: project,
                analyzerAssemblies: analyzerAssemblies,
                analyzerReferences: analyzerReferences,
                ignoreAnalyzerReferences: ignoreAnalyzerReferences,
                minimalSeverity: minimalSeverity,
                loadFixers: false).analyzers;
        }

        public static (ImmutableArray<DiagnosticAnalyzer> analyzers, ImmutableArray<CodeFixProvider> fixers) GetAnalyzersAndFixers(
            Project project,
            AnalyzerAssemblyList analyzerAssemblies,
            AnalyzerAssemblyList analyzerReferences,
            bool ignoreAnalyzerReferences = false,
            DiagnosticSeverity minimalSeverity = DiagnosticSeverity.Info)
        {
            return GetAnalyzersAndFixers(
                project: project,
                analyzerAssemblies: analyzerAssemblies,
                analyzerReferences: analyzerReferences,
                ignoreAnalyzerReferences: ignoreAnalyzerReferences,
                minimalSeverity: minimalSeverity,
                loadFixers: true);
        }

        private static (ImmutableArray<DiagnosticAnalyzer> analyzers, ImmutableArray<CodeFixProvider> fixers) GetAnalyzersAndFixers(
            Project project,
            AnalyzerAssemblyList analyzerAssemblies,
            AnalyzerAssemblyList analyzerReferences,
            bool ignoreAnalyzerReferences = false,
            DiagnosticSeverity minimalSeverity = DiagnosticSeverity.Info,
            bool loadFixers = true)
        {
            string language = project.Language;

            ImmutableArray<Assembly> assemblies = (ignoreAnalyzerReferences) ? ImmutableArray<Assembly>.Empty : project.AnalyzerReferences
                .Distinct()
                .OfType<AnalyzerFileReference>()
                .Select(f => f.GetAssembly())
                .Where(f => !analyzerAssemblies.ContainsAssembly(f.FullName))
                .ToImmutableArray();

            ImmutableArray<DiagnosticAnalyzer> analyzers = analyzerAssemblies
                .GetAnalyzers(language)
                .Concat(analyzerReferences.GetOrAddAnalyzers(assemblies, language))
                .Where(a =>
                {
                    return a.SupportedDiagnostics
                        .Any(d =>
                        {
                            ReportDiagnostic reportDiagnostic = d.GetEffectiveSeverity(project.CompilationOptions);

                            return reportDiagnostic != ReportDiagnostic.Suppress
                                && reportDiagnostic.ToDiagnosticSeverity() >= minimalSeverity;
                        });
                })
                 .ToImmutableArray();

            ImmutableArray<CodeFixProvider> fixers = ImmutableArray<CodeFixProvider>.Empty;

            if (analyzers.Any()
                && loadFixers)
            {
                fixers = analyzerAssemblies
                    .GetFixers(language)
                    .Concat(analyzerReferences.GetOrAddFixers(assemblies, language))
                    .ToImmutableArray();
            }

            return (analyzers, fixers);
        }
    }
}
