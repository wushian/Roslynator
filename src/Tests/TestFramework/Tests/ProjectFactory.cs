// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using static Roslynator.RuntimeMetadataReference;

namespace Roslynator.Tests
{
    public abstract class ProjectFactory
    {
        public abstract string Language { get; }

        public abstract string DefaultDocumentName { get; }

        public virtual string DefaultProjectName => "TestProject";

        public virtual Project CreateProject()
        {
            return new AdhocWorkspace()
            .CurrentSolution
            .AddProject(DefaultProjectName, "TestAssembly", Language)
            .WithMetadataReferences(ImmutableArray.Create(
                CorLibReference,
                CreateFromAssemblyName("System.Core.dll"),
                CreateFromAssemblyName("System.Linq.dll"),
                CreateFromAssemblyName("System.Linq.Expressions.dll"),
                CreateFromAssemblyName("System.Runtime.Serialization.Formatters.dll"),
                CreateFromAssemblyName("System.Runtime.dll"),
                CreateFromAssemblyName("System.Collections.dll"),
                CreateFromAssemblyName("System.Collections.Immutable.dll"),
                CreateFromAssemblyName("System.Text.RegularExpressions.dll"),
                CreateFromAssemblyName("Microsoft.CodeAnalysis.dll"),
                CreateFromAssemblyName("Microsoft.CodeAnalysis.CSharp.dll")));
        }

        public abstract Document CreateDocument(string source, params string[] additionalSources);

        protected virtual Document CreateDocumentCore(string name, string source, params string[] additionalSources)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (additionalSources == null)
                throw new ArgumentNullException(nameof(additionalSources));

            Document document = CreateProject().AddDocument(name, SourceText.From(source));

            Project project = document.Project;

            int length = additionalSources.Length;

            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    project = project
                        .AddDocument(PathHelpers.AddNumberToFileName(document.Name, i + 2), SourceText.From(additionalSources[i]))
                        .Project;
                }
            }

            return project.GetDocument(document.Id);
        }
    }
}
