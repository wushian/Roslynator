// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator
{
    public static class RuntimeMetadataReference
    {
        internal static readonly MetadataReference CorLibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        internal static readonly MetadataReference CSharpCodeAnalysisReference = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
        internal static readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);

        private static readonly ImmutableDictionary<string, string> _assemblyMap = GetTrustedPlatformAssemblies();

        private static ImmutableDictionary<string, string> GetTrustedPlatformAssemblies()
        {
            return AppContext
                        .GetData("TRUSTED_PLATFORM_ASSEMBLIES")
                        .ToString()
                        .Split(';')
                        .ToImmutableDictionary(Path.GetFileName);
        }

        public static PortableExecutableReference CreateFromAssemblyName(string assemblyName)
        {
            return MetadataReference.CreateFromFile(GetAssemblyLocation(assemblyName));
        }

        public static string GetAssemblyLocation(string assemblyName)
        {
            return _assemblyMap[assemblyName];
        }
    }
}
