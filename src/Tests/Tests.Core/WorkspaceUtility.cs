// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    public static class WorkspaceUtility
    {
        public const string TestFileName = "Test";
        public const string TestProjectName = "TestProject";

        public const string CSharpFileExtension = "cs";
        public const string VisualBasicFileExtension = "vb";

        private const int FileNumberingBase = 0;

        public const string DefaultCSharpFileName = TestFileName + "0." + CSharpFileExtension;
        public const string DefaultVisualBasicFileName = TestFileName + "0." + VisualBasicFileExtension;

        public static Project EmptyCSharpProject { get; } = CreateProject(LanguageNames.CSharp);

        public static Document CreateDocument(string source, string language)
        {
            return CreateProject(source, language).Documents.First();
        }

        public static IEnumerable<Document> CreateDocuments(IEnumerable<string> sources, string language)
        {
            return CreateProject(sources, language).Documents;
        }

        public static Project CreateProject(string source, string language)
        {
            Project project = (language == LanguageNames.CSharp) ? EmptyCSharpProject : CreateProject(language);

            ProjectId projectId = project.Id;

            string newFileName = CreateDefaultFileName(language: language);

            DocumentId documentId = DocumentId.CreateNewId(projectId, debugName: newFileName);

            return project
                .Solution
                .AddDocument(documentId, newFileName, SourceText.From(source))
                .GetProject(projectId);
        }

        public static Project CreateProject(IEnumerable<string> sources, string language)
        {
            Project project = (language == LanguageNames.CSharp) ? EmptyCSharpProject : CreateProject(language);

            Solution solution = project.Solution;

            int count = FileNumberingBase;
            foreach (string source in sources)
            {
                string newFileName = CreateFileName(suffix: count, language: language);
                DocumentId documentId = DocumentId.CreateNewId(project.Id, debugName: newFileName);
                solution = solution.AddDocument(documentId, newFileName, SourceText.From(source));
                count++;
            }

            return solution.GetProject(project.Id);
        }

        public static Project CreateProject(string language)
        {
            ProjectId projectId = ProjectId.CreateNewId(debugName: TestProjectName);

            Project project = new AdhocWorkspace()
                .CurrentSolution
                .AddProject(projectId, TestProjectName, TestProjectName, language)
                .AddMetadataReferences(
                    projectId,
                    new MetadataReference[]
                    {
                        RuntimeMetadataReference.CorLibReference,
                        RuntimeMetadataReference.CreateFromAssemblyName("System.Core.dll"),
                        RuntimeMetadataReference.CreateFromAssemblyName("System.Linq.dll"),
                        RuntimeMetadataReference.CreateFromAssemblyName("System.Linq.Expressions.dll"),
                        RuntimeMetadataReference.CreateFromAssemblyName("System.Runtime.dll"),
                        RuntimeMetadataReference.CreateFromAssemblyName("System.Collections.Immutable.dll"),
                        RuntimeMetadataReference.CreateFromAssemblyName("Microsoft.CodeAnalysis.dll"),
                        RuntimeMetadataReference.CreateFromAssemblyName("Microsoft.CodeAnalysis.CSharp.dll"),
                    })
                .GetProject(projectId);

            if (language == LanguageNames.CSharp)
            {
                var compilationOptions = (CSharpCompilationOptions)project.CompilationOptions;

                var parseOptions = (CSharpParseOptions)project.ParseOptions;

                project = project
                    .WithCompilationOptions(compilationOptions.WithAllowUnsafe(true))
                    .WithParseOptions(parseOptions.WithLanguageVersion(LanguageVersion.Latest));
            }

            return project;
        }

        public static string CreateDefaultFileName(string language)
        {
            return (language == LanguageNames.CSharp) ? DefaultCSharpFileName : DefaultVisualBasicFileName;
        }

        public static string CreateFileName(string fileName = TestFileName, int suffix = FileNumberingBase, string language = LanguageNames.CSharp)
        {
            string extension = ((language == LanguageNames.CSharp) ? CSharpFileExtension : VisualBasicFileExtension);

            return $"{fileName}{suffix}.{extension}";
        }
    }
}
