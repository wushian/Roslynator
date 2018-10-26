// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

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

        public static bool BeginsWithAutoGeneratedComment(SyntaxNode root)
        {
            switch (root.Language)
            {
                case LanguageNames.CSharp:
                    {
                        return GeneratedCodeUtility.BeginsWithAutoGeneratedComment(
                            root,
                            f => f.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.SingleLineCommentTrivia, Microsoft.CodeAnalysis.CSharp.SyntaxKind.MultiLineCommentTrivia));
                    }
                case LanguageNames.VisualBasic:
                    {
                        return GeneratedCodeUtility.BeginsWithAutoGeneratedComment(
                            root,
                            f => f.IsKind(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommentTrivia));
                    }
            }

            return false;
        }

        public static bool BeginsWithBanner(SyntaxNode root, ImmutableArray<string> banner)
        {
            switch (root.Language)
            {
                case LanguageNames.CSharp:
                    {
                        return BeginsWithBanner(
                            root,
                            banner,
                            f => f.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.SingleLineCommentTrivia),
                            f => f.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.EndOfLineTrivia),
                            2);
                    }
                case LanguageNames.VisualBasic:
                    {
                        return BeginsWithBanner(
                            root,
                            banner,
                            f => f.IsKind(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommentTrivia),
                            f => f.IsKind(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOfLineTrivia),
                            1);
                    }
                default:
                    {
                        throw new NotSupportedException($"Language '{root.Language}' is not supported.");
                    }
            }
        }

        public static bool BeginsWithBanner(
            SyntaxNode root,
            ImmutableArray<string> banner,
            Func<SyntaxTrivia, bool> isSingleLineComment,
            Func<SyntaxTrivia, bool> isEndOfLine,
            int singleLineCommentStartLength)
        {
            SyntaxTriviaList leading = root.GetLeadingTrivia();

            if (banner.Length > leading.Count)
                return false;

            int i = 0;
            while (i < leading.Count)
            {
                SyntaxTrivia trivia = leading[i];

                if (isSingleLineComment(trivia))
                {
                    string comment = trivia.ToString();

                    if (string.Compare(
                        banner[i],
                        0,
                        comment,
                        singleLineCommentStartLength,
                        comment.Length - singleLineCommentStartLength,
                        StringComparison.Ordinal) != 0)
                    {
                        return false;
                    }

                    if (i == banner.Length - 1)
                        return true;

                    i++;

                    if (i == leading.Count
                        || !isEndOfLine(leading[i]))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                i++;
            }

            return i == banner.Length;
        }
    }
}
