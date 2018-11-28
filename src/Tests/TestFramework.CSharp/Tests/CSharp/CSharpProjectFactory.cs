// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.Tests.CSharp
{
    public class CSharpProjectFactory : ProjectFactory
    {
        internal static CSharpProjectFactory Instance { get; } = new CSharpProjectFactory();

        public override string Language => LanguageNames.CSharp;

        public override string DefaultDocumentName => "Test.cs";

        public override Project CreateProject()
        {
            Project project = base.CreateProject();

            var compilationOptions = (CSharpCompilationOptions)project.CompilationOptions;

            CSharpCompilationOptions newCompilationOptions = compilationOptions
                .WithAllowUnsafe(true)
                .WithOutputKind(OutputKind.DynamicallyLinkedLibrary);

            var parseOptions = (CSharpParseOptions)project.ParseOptions;

            CSharpParseOptions newParseOptions = parseOptions
                .WithLanguageVersion(LanguageVersion.Latest);

            return project
                .WithCompilationOptions(newCompilationOptions)
                .WithParseOptions(newParseOptions);
        }

        public override Document CreateDocument(string source, params string[] additionalSources)
        {
            return CreateDocumentCore(DefaultDocumentName, source, additionalSources);
        }
    }
}
