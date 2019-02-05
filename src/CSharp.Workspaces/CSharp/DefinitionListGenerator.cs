// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp;

namespace Roslynator
{
    //TODO: del
    internal static class DefinitionListGenerator
    {
        public static string Generate(
            IEnumerable<IAssemblySymbol> assemblies,
            DefinitionListOptions options = null,
            IComparer<ISymbol> comparer = null)
        {
            options = options ?? DefinitionListOptions.Default;

            using (var writer = new StringWriter())
            {
                var builder = new DefinitionListWriter(
                    writer,
                    options: options,
                    comparer: comparer);

                builder.Write(assemblies);

                return builder.ToString();
            }
        }
    }
}
