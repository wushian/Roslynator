// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public class ProjectBuilder
    {
        public ProjectBuilder(
            string projectName,
            string assemblyName,
            ImmutableArray<MetadataReference> metadataReferences,
            Func<string, int, string> createFileName)
        {
            ProjectName = projectName;
            AssemblyName = assemblyName;
            MetadataReferences = metadataReferences;
            CreateFileName = createFileName;
        }

        public ProjectBuilder(
            ProjectInfo projectInfo,
            Func<string, int, string> createFileName)
        {
            ProjectInfo = projectInfo;
            CreateFileName = createFileName;
        }

        public static ProjectBuilder Default { get; } = new ProjectBuilder(
            FileUtility.TestProjectName,
            null,
            RuntimeMetadataReference.DefaultReferences,
            null);

        public string DocumentName { get; }

        public string ProjectName { get; }

        public string AssemblyName { get; }

        public ImmutableArray<MetadataReference> MetadataReferences { get; }

        public Func<string, int, string> CreateFileName { get; }

        public ProjectInfo ProjectInfo { get; }
    }
}