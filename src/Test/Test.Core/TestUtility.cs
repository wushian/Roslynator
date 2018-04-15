// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Test
{
    public static class TestUtility
    {
        public const string TestFileName = "Test";
        public const string TestProjectName = "TestProject";

        public const string CSharpFileExtension = "cs";
        public const string VisualBasicFileExtension = "vb";

        internal static readonly Comparison<Diagnostic> DiagnosticSpanStartComparison = new Comparison<Diagnostic>((x, y) => Comparer<int>.Default.Compare(x.Location.SourceSpan.Start, y.Location.SourceSpan.Start));

        public static Project EmptyCSharpProject { get; } = CreateProject();

        public static Document GetDocument(string source, string language = LanguageNames.CSharp)
        {
            if (language != LanguageNames.CSharp
                && language != LanguageNames.VisualBasic)
            {
                throw new ArgumentException("Unsupported language.", nameof(language));
            }

            return CreateProject(source, language).Documents.First();
        }

        public static IEnumerable<Document> GetDocuments(IEnumerable<string> sources, string language = LanguageNames.CSharp)
        {
            if (language != LanguageNames.CSharp
                && language != LanguageNames.VisualBasic)
            {
                throw new ArgumentException("Unsupported language.", nameof(language));
            }

            return CreateProject(sources, language).Documents;
        }

        public static Project CreateProject(string source, string language = LanguageNames.CSharp)
        {
            Project project = (language == LanguageNames.CSharp) ? EmptyCSharpProject : CreateProject(language);

            ProjectId projectId = project.Id;

            string newFileName = CreateFileName(language: language);

            DocumentId documentId = DocumentId.CreateNewId(projectId, debugName: newFileName);

            return project
                .Solution
                .AddDocument(documentId, newFileName, SourceText.From(source))
                .GetProject(projectId);
        }

        public static Project CreateProject(IEnumerable<string> sources, string language = LanguageNames.CSharp)
        {
            Project project = (language == LanguageNames.CSharp) ? EmptyCSharpProject : CreateProject(language);

            Solution solution = project.Solution;

            int count = 0;
            foreach (string source in sources)
            {
                string newFileName = CreateFileName(suffix: count, language: language);
                DocumentId documentId = DocumentId.CreateNewId(project.Id, debugName: newFileName);
                solution = solution.AddDocument(documentId, newFileName, SourceText.From(source));
                count++;
            }

            return solution.GetProject(project.Id);
        }

        public static Project CreateProject(string language = LanguageNames.CSharp)
        {
            ProjectId projectId = ProjectId.CreateNewId(debugName: TestProjectName);

            return new AdhocWorkspace()
                .CurrentSolution
                .AddProject(projectId, TestProjectName, TestProjectName, language)
                .AddMetadataReferences(
                    projectId,
                    new MetadataReference[]
                    {
                        RuntimeMetadataReferenceResolver.CorLibReference,
                        RuntimeMetadataReferenceResolver.SystemCoreReference,
                        RuntimeMetadataReferenceResolver.CSharpCodeAnalysisReference,
                        RuntimeMetadataReferenceResolver.CodeAnalysisReference,
                        MetadataReference.CreateFromFile(RuntimeMetadataReferenceResolver.GetAssemblyLocation("System.Runtime.dll"))
                    })
                .GetProject(projectId);
        }

        public static string CreateFileName(string fileName = TestFileName, int suffix = 0, string language = LanguageNames.CSharp)
        {
            string extension = ((language == LanguageNames.CSharp) ? CSharpFileExtension : VisualBasicFileExtension);

            return $"{fileName}{suffix}.{extension}";
        }

        public static Diagnostic[] GetSortedDiagnostics(
            DiagnosticAnalyzer analyzer,
            IEnumerable<string> sources,
            string language = LanguageNames.CSharp)
        {
            IEnumerable<Document> documents = GetDocuments(sources, language);

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

            diagnostics.Sort(DiagnosticSpanStartComparison);

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

            diagnostics.Sort(DiagnosticSpanStartComparison);

            return diagnostics.ToArray();
        }

        private static ImmutableArray<Diagnostic> GetDiagnostics(Project project, DiagnosticAnalyzer analyzer)
        {
            Compilation compilation = project
                .GetCompilationAsync()
                .Result;

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

        //TODO: del
        public static (string newText, List<LinePositionSpan> spans) GetTextAndSpans(string s)
        {
            var sb = new StringBuilder();

            var spans = new List<LinePositionSpan>();

            int lastPos = 0;

            int line = 0;
            int column = 0;

            int startLine = -1;
            int startColumn = -1;

            int length = s.Length;

            for (int i = 0; i < length; i++)
            {
                switch (s[i])
                {
                    case '\r':
                        {
                            if (i < length - 1
                                && s[i + 1] == '\n')
                            {
                                i++;
                            }

                            line++;
                            column = 0;
                            continue;
                        }
                    case '\n':
                        {
                            line++;
                            column = 0;
                            continue;
                        }
                    case '<':
                        {
                            if (i < length - 1
                                && s[i + 1] == '<'
                                && i < length - 2
                                && s[i + 2] == '<'
                                && i < length - 3
                                && s[i + 3] != '<')
                            {
                                sb.Append(s, lastPos, i - lastPos);

                                startLine = line;
                                startColumn = column;

                                i += 2;

                                lastPos = i + 1;

                                continue;
                            }

                            break;
                        }
                    case '>':
                        {
                            if (startColumn != -1
                                && i < length - 1
                                && s[i + 1] == '>'
                                && i < length - 2
                                && s[i + 2] == '>'
                                && i < length - 3
                                && s[i + 3] != '>')
                            {
                                sb.Append(s, lastPos, i - lastPos);

                                var span = new LinePositionSpan(
                                    new LinePosition(startLine, startColumn),
                                    new LinePosition(line, column));

                                spans.Add(span);

                                i += 2;

                                lastPos = i + 1;

                                startLine = -1;
                                startColumn = -1;

                                continue;
                            }

                            break;
                        }
                }

                column++;
            }

            sb.Append(s, lastPos, s.Length - lastPos);

            return (sb.ToString(), spans);
        }
    }
}
