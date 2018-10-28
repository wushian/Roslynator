// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using static Roslynator.Logger;

namespace Roslynator
{
    internal static class WorkspacesUtilities
    {
        public static ImmutableArray<DiagnosticAnalyzer> GetAnalyzers(
            Project project,
            AnalyzerAssemblyList analyzerAssemblies,
            AnalyzerAssemblyList analyzerReferences,
            ImmutableHashSet<string> supportedDiagnosticIds,
            ImmutableHashSet<string> ignoredDiagnosticIds,
            bool ignoreAnalyzerReferences = false,
            DiagnosticSeverity minimalSeverity = DiagnosticSeverity.Info)
        {
            return GetAnalyzersAndFixers(
                project: project,
                analyzerAssemblies: analyzerAssemblies,
                analyzerReferences: analyzerReferences,
                supportedDiagnosticIds: supportedDiagnosticIds,
                ignoredDiagnosticIds: ignoredDiagnosticIds,
                ignoreAnalyzerReferences: ignoreAnalyzerReferences,
                minimalSeverity: minimalSeverity,
                loadFixers: false).analyzers;
        }

        public static (ImmutableArray<DiagnosticAnalyzer> analyzers, ImmutableArray<CodeFixProvider> fixers) GetAnalyzersAndFixers(
            Project project,
            AnalyzerAssemblyList analyzerAssemblies,
            AnalyzerAssemblyList analyzerReferences,
            ImmutableHashSet<string> supportedDiagnosticIds,
            ImmutableHashSet<string> ignoredDiagnosticIds,
            bool ignoreAnalyzerReferences = false,
            DiagnosticSeverity minimalSeverity = DiagnosticSeverity.Info)
        {
            return GetAnalyzersAndFixers(
                project: project,
                analyzerAssemblies: analyzerAssemblies,
                analyzerReferences: analyzerReferences,
                supportedDiagnosticIds: supportedDiagnosticIds,
                ignoredDiagnosticIds: ignoredDiagnosticIds,
                ignoreAnalyzerReferences: ignoreAnalyzerReferences,
                minimalSeverity: minimalSeverity,
                loadFixers: true);
        }

        private static (ImmutableArray<DiagnosticAnalyzer> analyzers, ImmutableArray<CodeFixProvider> fixers) GetAnalyzersAndFixers(
            Project project,
            AnalyzerAssemblyList analyzerAssemblies,
            AnalyzerAssemblyList analyzerReferences,
            ImmutableHashSet<string> supportedDiagnosticIds,
            ImmutableHashSet<string> ignoredDiagnosticIds,
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
                .Where(analyzer =>
                {
                    ImmutableArray<DiagnosticDescriptor> supportedDiagnostics = analyzer.SupportedDiagnostics;

                    if (supportedDiagnosticIds.Count > 0)
                    {
                        bool success = false;

                        foreach (DiagnosticDescriptor supportedDiagnostic in supportedDiagnostics)
                        {
                            if (supportedDiagnosticIds.Contains(supportedDiagnostic.Id))
                            {
                                success = true;
                                break;
                            }
                        }

                        if (!success)
                            return false;
                    }
                    else if (ignoredDiagnosticIds.Count > 0)
                    {
                        bool success = false;

                        foreach (DiagnosticDescriptor supportedDiagnostic in supportedDiagnostics)
                        {
                            if (!ignoredDiagnosticIds.Contains(supportedDiagnostic.Id))
                            {
                                success = true;
                                break;
                            }
                        }

                        if (!success)
                            return false;
                    }

                    foreach (DiagnosticDescriptor supportedDiagnostic in supportedDiagnostics)
                    {
                        ReportDiagnostic reportDiagnostic = supportedDiagnostic.GetEffectiveSeverity(project.CompilationOptions);

                        if (reportDiagnostic != ReportDiagnostic.Suppress
                            && reportDiagnostic.ToDiagnosticSeverity() >= minimalSeverity)
                        {
                            return true;
                        }
                    }

                    return false;
                })
                 .ToImmutableArray();

            ImmutableArray<CodeFixProvider> fixers = ImmutableArray<CodeFixProvider>.Empty;

            if (analyzers.Any()
                && loadFixers)
            {
                HashSet<string> diagnosticIds = analyzers
                    .SelectMany(f => f.SupportedDiagnostics)
                    .Select(f => f.Id)
                    .ToHashSet();

                fixers = analyzerAssemblies
                    .GetFixers(language)
                    .Concat(analyzerReferences.GetOrAddFixers(assemblies, language))
                    .Where(f => f.FixableDiagnosticIds.Any(id => diagnosticIds.Contains(id)))
                    .ToImmutableArray();
            }

            return (analyzers, fixers);
        }

        public static async Task<bool> VerifySyntaxEquivalenceAsync(
            Document oldDocument,
            Document newDocument,
            CancellationToken cancellationToken = default)
        {
            if (!string.Equals(
                (await newDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false)).NormalizeWhitespace("", false).ToFullString(),
                (await oldDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false)).NormalizeWhitespace("", false).ToFullString(),
                StringComparison.Ordinal))
            {
                WriteLine("Syntax roots with normalized white-space are not equivalent", ConsoleColor.Magenta);
                return false;
            }

            if (!SyntaxFactsService.GetService(oldDocument.Project.Language).AreEquivalent(
                await newDocument.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false),
                await oldDocument.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false)))
            {
                WriteLine("Syntax trees are not equivalent", ConsoleColor.Magenta);
                return false;
            }

            return true;
        }
    }
}
