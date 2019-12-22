// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator.Testing
{
    public abstract class CodeVerificationOptions
    {
        private ImmutableArray<MetadataReference> _metadataReferences;

        protected CodeVerificationOptions(
            IEnumerable<string> assemblyNames,
            DiagnosticSeverity allowedCompilerDiagnosticSeverity = DiagnosticSeverity.Info,
            IEnumerable<string> allowedCompilerDiagnosticIds = null)
        {
            AssemblyNames = assemblyNames;
            AllowedCompilerDiagnosticSeverity = allowedCompilerDiagnosticSeverity;
            AllowedCompilerDiagnosticIds = allowedCompilerDiagnosticIds?.ToImmutableArray() ?? ImmutableArray<string>.Empty;
        }

        protected abstract ParseOptions CommonParseOptions { get; }

        protected abstract CompilationOptions CommonCompilationOptions { get; }

        public ParseOptions ParseOptions => CommonParseOptions;

        public CompilationOptions CompilationOptions => CommonCompilationOptions;

        public IEnumerable<string> AssemblyNames { get; }

        public DiagnosticSeverity AllowedCompilerDiagnosticSeverity { get; }

        public ImmutableArray<string> AllowedCompilerDiagnosticIds { get; }

        internal ImmutableArray<MetadataReference> MetadataReferences
        {
            get
            {
                if (_metadataReferences.IsDefault)
                    ImmutableInterlocked.InterlockedInitialize(ref _metadataReferences, CreateMetadataReferences());

                return _metadataReferences;

                ImmutableArray<MetadataReference> CreateMetadataReferences()
                {
                    ImmutableArray<MetadataReference>.Builder builder = ImmutableArray.CreateBuilder<MetadataReference>();

                    builder.Add(RuntimeMetadataReference.CorLibReference);
                    builder.AddRange(AssemblyNames.Select(f => RuntimeMetadataReference.CreateFromAssemblyName(f)));

                    return builder.ToImmutableArray();
                }
            }
        }

        public abstract CodeVerificationOptions AddAllowedCompilerDiagnosticId(string diagnosticId);

        public abstract CodeVerificationOptions AddAllowedCompilerDiagnosticIds(IEnumerable<string> diagnosticIds);
    }
}
