// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Tests
{
    public abstract class CodeVerifier
    {
        public abstract CodeVerificationOptions Options { get; }

        public abstract string Language { get; }

        protected virtual Document CreateDocument(string source, params string[] additionalSources)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (additionalSources == null)
                throw new ArgumentNullException(nameof(additionalSources));

            ProjectBuilder projectBuilder = ProjectBuilder.Default;

            OnProjectCreating(projectBuilder);

            Project project = CreateProject(projectBuilder);

            ProjectId projectId = project.Id;

            string newFileName = projectBuilder.CreateFileName(projectBuilder.DocumentName, 0);

            DocumentId documentId = DocumentId.CreateNewId(projectId, debugName: newFileName);

            Document document = project.AddDocument(newFileName, SourceText.From(source));

            project = document.Project;

            if (additionalSources.Length > 0)
            {
                Solution solution = project.Solution;

                for (int i = 0; i < additionalSources.Length; i++)
                {
                    newFileName = projectBuilder.CreateFileName(projectBuilder.DocumentName, i + 1);
                    project = project.AddDocument(newFileName, SourceText.From(additionalSources[i])).Project;
                }
            }

            return project.GetDocument(document.Id);
        }

        protected virtual void OnDocumentCreating(DocumentBuilder documentBuilder)
        {
        }

        protected virtual void OnProjectCreating(ProjectBuilder projectBuilder)
        {
        }

        protected virtual void OnProjectCreated(Project project)
        {
        }

        protected virtual void OnSolutionCreating(SolutionBuilder projectBuilder)
        {
        }

        protected virtual void OnWorkspaceCreating(WorkspaceBuilder projectBuilder)
        {
        }

        protected virtual Project CreateProject(ProjectBuilder projectBuilder)
        {
            var workspaceBuilder = new WorkspaceBuilder();

            var workspace = new AdhocWorkspace(MefHostServices.DefaultHost, workspaceKind: "Test");

            Solution solution = workspace.CurrentSolution;

            Project project;

            ProjectInfo projectInfo = projectBuilder.ProjectInfo;

            if (projectInfo != null)
            {
                project = solution.AddProject(projectInfo).GetProject(projectInfo.Id);
            }
            else
            {
                project = solution
                    .AddProject(projectBuilder.ProjectName, projectBuilder.AssemblyName ?? projectBuilder.ProjectName, Language)
                    .WithMetadataReferences(projectBuilder.MetadataReferences);
            }

            if (Language == LanguageNames.CSharp)
            {
                var compilationOptions = (CSharpCompilationOptions)project.CompilationOptions;

                CSharpCompilationOptions newCompilationOptions = compilationOptions
                    .WithAllowUnsafe(true)
                    .WithOutputKind(OutputKind.DynamicallyLinkedLibrary);

                var parseOptions = (CSharpParseOptions)project.ParseOptions;

                CSharpParseOptions newParseOptions = parseOptions
                    .WithLanguageVersion(LanguageVersion.Latest);

                project = project
                    .WithCompilationOptions(newCompilationOptions)
                    .WithParseOptions(newParseOptions);
            }

            return project;
        }
    }
}
