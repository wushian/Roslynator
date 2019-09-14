// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal class ListReferencesCommand : MSBuildWorkspaceCommand
    {
        public ListReferencesCommand(
            ListReferencesCommandLineOptions options,
            MetadataReferenceDisplay display,
            in ProjectFilter projectFilter) : base(projectFilter)
        {
            Options = options;
            Display = display;
        }

        public ListReferencesCommandLineOptions Options { get; }

        public MetadataReferenceDisplay Display { get; }

        public override async Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
        {
            AssemblyResolver.Register();

            ImmutableArray<Compilation> compilations = await GetCompilationsAsync(projectOrSolution, cancellationToken);

            int count = 0;

            foreach (string display in compilations
                .SelectMany(compilation => compilation.ExternalReferences.Select(reference => (compilation, reference)))
                .Select(f => GetDisplay(f.compilation, f.reference))
                .Distinct()
                .OrderBy(f => f, StringComparer.InvariantCulture))
            {
                WriteLine(display);
                count++;
            }

            if (ShouldWrite(Verbosity.Normal))
            {
                WriteLine(Verbosity.Normal);
                WriteLine($"{count} assembl{((count == 1) ? "y" : "ies")} found", ConsoleColor.Green, Verbosity.Normal);
                WriteLine(Verbosity.Normal);
            }

            return CommandResult.Success;

            string GetDisplay(Compilation compilation, MetadataReference reference)
            {
                switch (reference)
                {
                    case PortableExecutableReference portableReference:
                        {
                            string path = portableReference.FilePath;

                            switch (Display)
                            {
                                case MetadataReferenceDisplay.Path:
                                    {
                                        return path;
                                    }
                                case MetadataReferenceDisplay.FileName:
                                    {
                                        return Path.GetFileName(path);
                                    }
                                case MetadataReferenceDisplay.FileNameWithoutExtension:
                                    {
                                        return Path.GetFileNameWithoutExtension(path);
                                    }
                                case MetadataReferenceDisplay.AssemblyName:
                                    {
                                        var assembly = (IAssemblySymbol)compilation.GetAssemblyOrModuleSymbol(reference);

                                        return assembly.Identity.ToString();
                                    }
                                default:
                                    {
                                        throw new InvalidOperationException();
                                    }
                            }
                        }
                    case CompilationReference compilationReference:
                        {
                            return compilationReference.Display;
                        }
                    default:
                        {
#if DEBUG
                            WriteLine(reference.GetType().FullName, ConsoleColor.Yellow);
#endif
                            return reference.Display;
                        }
                }
            }
        }
    }
}
