// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator
{
    public static class DiagnosticUtility
    {
        public static Diagnostic[] GetSortedDiagnostics(
            DiagnosticAnalyzer analyzer,
            IEnumerable<string> sources,
            string language = LanguageNames.CSharp)
        {
            IEnumerable<Document> documents = WorkspaceUtility.GetDocuments(sources, language);

            return GetSortedDiagnostics(analyzer, documents);
        }

        public static Diagnostic[] GetSortedDiagnostics(
            DiagnosticAnalyzer analyzer,
            Document document)
        {
            Project project = document.Project;

            var diagnostics = new List<Diagnostic>();

            SyntaxTree tree = document.GetSyntaxTreeAsync().Result;

            foreach (Diagnostic diagnostic in GetDiagnostics(project, analyzer))
            {
                Location location = diagnostic.Location;

                if (location == Location.None
                    || location.IsInMetadata
                    || tree == location.SourceTree)
                {
                    diagnostics.Add(diagnostic);
                }
                else
                {
                    Debug.Fail(location.ToString());
                }
            }

            diagnostics.Sort(DiagnosticComparer.SpanStart);

            return diagnostics.ToArray();
        }

        public static Diagnostic[] GetSortedDiagnostics(
            DiagnosticAnalyzer analyzer,
            IEnumerable<Document> documents)
        {
            Project project = documents.First().Project;

            var diagnostics = new List<Diagnostic>();

            foreach (Diagnostic diagnostic in GetDiagnostics(project, analyzer))
            {
                Location location = diagnostic.Location;

                if (location == Location.None
                    || location.IsInMetadata)
                {
                    diagnostics.Add(diagnostic);
                }
                else
                {
                    foreach (Document document in documents)
                    {
                        SyntaxTree tree = document.GetSyntaxTreeAsync().Result;

                        if (tree == location.SourceTree)
                            diagnostics.Add(diagnostic);
                    }
                }
            }

            diagnostics.Sort(DiagnosticComparer.SpanStart);

            return diagnostics.ToArray();
        }

        private static ImmutableArray<Diagnostic> GetDiagnostics(Project project, DiagnosticAnalyzer analyzer)
        {
            Compilation compilation = project
                .GetCompilationAsync()
                .Result;

            //foreach (Diagnostic diagnostic in compilation.GetDiagnostics())
            //{
            //    if (diagnostic.Descriptor.DefaultSeverity == DiagnosticSeverity.Error
            //        && diagnostic.Descriptor.CustomTags.Contains(WellKnownDiagnosticTags.Compiler))
            //    {
            //        Debug.WriteLine(diagnostic.ToString());
            //    }
            //}

            foreach (DiagnosticDescriptor descriptor in analyzer.SupportedDiagnostics)
            {
                if (!descriptor.IsEnabledByDefault)
                {
                    CompilationOptions compilationOptions = compilation.Options;
                    ImmutableDictionary<string, ReportDiagnostic> specificDiagnosticOptions = compilationOptions.SpecificDiagnosticOptions;

                    specificDiagnosticOptions = specificDiagnosticOptions.Add(descriptor.Id, descriptor.DefaultSeverity.ToReportDiagnostic());
                    CompilationOptions options = compilationOptions.WithSpecificDiagnosticOptions(specificDiagnosticOptions);

                    compilation = compilation.WithOptions(options);
                }
            }

            CompilationWithAnalyzers compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create(analyzer));

            return compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result;
        }

        internal static IEnumerable<Diagnostic> GetNewDiagnostics(
            IEnumerable<Diagnostic> diagnostics,
            IEnumerable<Diagnostic> newDiagnostics)
        {
            using (IEnumerator<Diagnostic> enNew = newDiagnostics.OrderBy(f => f.Location.SourceSpan.Start).GetEnumerator())
            using (IEnumerator<Diagnostic> en = diagnostics.OrderBy(f => f.Location.SourceSpan.Start).GetEnumerator())
            {
                while (enNew.MoveNext())
                {
                    if (en.MoveNext())
                    {
                        if (en.Current.Id != enNew.Current.Id)
                            yield return enNew.Current;
                    }
                    else
                    {
                        yield return enNew.Current;

                        while (enNew.MoveNext())
                            yield return enNew.Current;

                        yield break;
                    }
                }
            }
        }

        public static ImmutableArray<Diagnostic> GetCompilerDiagnostics(Document document)
        {
            return document.GetSemanticModelAsync().Result.GetDiagnostics();
        }
    }
}
