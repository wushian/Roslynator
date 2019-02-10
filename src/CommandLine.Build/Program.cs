// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.Documentation;
using Roslynator.Documentation.Markdown;
using static Roslynator.CommandLine.DocumentationHelpers;
using static Roslynator.CommandLine.ParseHelpers;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal static class Program
    {
        private static readonly Encoding _defaultEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        private static int Main(string[] args)
        {
            WriteLine($"Roslynator Command Line Tool version {typeof(Program).GetTypeInfo().Assembly.GetName().Version}", Verbosity.Quiet);
            WriteLine("Copyright (c) Josef Pihrt. All rights reserved.", Verbosity.Quiet);
            WriteLine(Verbosity.Quiet);

            try
            {
                ParserResult<object> parserResult = Parser.Default.ParseArguments<GenerateDocCommandLineOptions,
                    GenerateDeclarationsCommandLineOptions,
                    GenerateDocRootCommandLineOptions>(args);

                bool verbosityParsed = false;

                parserResult.WithParsed<AbstractCommandLineOptions>(options =>
                {
                    var defaultVerbosity = Verbosity.Normal;

                    if (options.Verbosity == null
                        || TryParseVerbosity(options.Verbosity, out defaultVerbosity))
                    {
                        ConsoleOut.Verbosity = defaultVerbosity;

                        Verbosity fileLogVerbosity = defaultVerbosity;

                        if (options.FileLogVerbosity == null
                            || TryParseVerbosity(options.FileLogVerbosity, out fileLogVerbosity))
                        {
                            if (options.FileLog != null)
                            {
                                var fs = new FileStream(options.FileLog, FileMode.Create, FileAccess.Write, FileShare.Read);
                                var sw = new StreamWriter(fs, Encoding.UTF8, bufferSize: 4096, leaveOpen: false);
                                Out = new TextWriterWithVerbosity(sw) { Verbosity = fileLogVerbosity };
                            }

                            verbosityParsed = true;
                        }
                    }
                });

                if (!verbosityParsed)
                    return 1;

                return parserResult.MapResult(
                    (GenerateDocCommandLineOptions options) => GenerateDoc(options),
                    (GenerateDeclarationsCommandLineOptions options) => GenerateDeclarations(options),
                    (GenerateDocRootCommandLineOptions options) => GenerateDocRoot(options),
                    _ => 1);
            }
            catch (Exception ex)
            {
                WriteLine(ex.ToString());
            }
            finally
            {
                Out?.Dispose();
                Out = null;
#if DEBUG
                if (Debugger.IsAttached)
                    Console.ReadKey();
#endif
            }

            return 1;
        }

        private static int GenerateDoc(GenerateDocCommandLineOptions options)
        {
            if (options.MaxDerivedTypes < 0)
            {
                WriteLine("Maximum number of derived items must be equal or greater than 0.", Verbosity.Quiet);
                return 1;
            }

            if (!TryParseParameterValueAsEnum(options.Depth, ParameterNames.Depth, out DocumentationDepth depth, DocumentationOptions.Default.Depth))
                return 1;

            if (!TryParseParameterValueAsEnumFlags(options.IgnoredRootParts, ParameterNames.IgnoredRootParts, out RootDocumentationParts ignoredRootParts, DocumentationOptions.Default.IgnoredRootParts))
                return 1;

            if (!TryParseParameterValueAsEnumFlags(options.IgnoredNamespaceParts, ParameterNames.IgnoredNamespaceParts, out NamespaceDocumentationParts ignoredNamespaceParts, DocumentationOptions.Default.IgnoredNamespaceParts))
                return 1;

            if (!TryParseParameterValueAsEnumFlags(options.IgnoredTypeParts, ParameterNames.IgnoredTypeParts, out TypeDocumentationParts ignoredTypeParts, DocumentationOptions.Default.IgnoredTypeParts))
                return 1;

            if (!TryParseParameterValueAsEnumFlags(options.IgnoredMemberParts, ParameterNames.IgnoredMemberParts, out MemberDocumentationParts ignoredMemberParts, DocumentationOptions.Default.IgnoredMemberParts))
                return 1;

            if (!TryParseParameterValueAsEnumFlags(options.OmitContainingNamespaceParts, ParameterNames.OmitContainingNamespaceParts, out OmitContainingNamespaceParts omitContainingNamespaceParts, DocumentationOptions.Default.OmitContainingNamespaceParts))
                return 1;

            if (!TryParseParameterValueAsEnum(options.Visibility, ParameterNames.Visibility, out Visibility visibility))
                return 1;

            DocumentationModel documentationModel = CreateDocumentationModel(options.References, options.Assemblies, visibility, options.AdditionalXmlDocumentation);

            if (documentationModel == null)
                return 1;

            var documentationOptions = new DocumentationOptions(
                ignoredNames: options.IgnoredNames,
                preferredCultureName: options.PreferredCulture,
                maxDerivedTypes: options.MaxDerivedTypes,
                includeClassHierarchy: !options.NoClassHierarchy,
                placeSystemNamespaceFirst: !options.NoPrecedenceForSystem,
                formatDeclarationBaseList: !options.NoFormatBaseList,
                formatDeclarationConstraints: !options.NoFormatConstraints,
                markObsolete: !options.NoMarkObsolete,
                includeMemberInheritedFrom: !options.OmitMemberInheritedFrom,
                includeMemberOverrides: !options.OmitMemberOverrides,
                includeMemberImplements: !options.OmitMemberImplements,
                includeMemberConstantValue: !options.OmitMemberConstantValue,
                includeInheritedInterfaceMembers: options.IncludeInheritedInterfaceMembers,
                includeAllDerivedTypes: options.IncludeAllDerivedTypes,
                includeAttributeArguments: !options.OmitAttributeArguments,
                includeInheritedAttributes: !options.OmitInheritedAttributes,
                omitIEnumerable: !options.IncludeIEnumerable,
                depth: depth,
                inheritanceStyle: options.InheritanceStyle,
                ignoredRootParts: ignoredRootParts,
                ignoredNamespaceParts: ignoredNamespaceParts,
                ignoredTypeParts: ignoredTypeParts,
                ignoredMemberParts: ignoredMemberParts,
                omitContainingNamespaceParts: omitContainingNamespaceParts,
                scrollToContent: options.ScrollToContent);

            var generator = new MarkdownDocumentationGenerator(documentationModel, WellKnownUrlProviders.GitHub, documentationOptions);

            string directoryPath = options.OutputPath;

            if (!options.NoDelete
                && Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.Delete(directoryPath, recursive: true);
                }
                catch (IOException ex)
                {
                    WriteLine(ex.ToString(), Verbosity.Quiet);
                    return 1;
                }
            }

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            CancellationToken cancellationToken = cts.Token;

            WriteLine($"Documentation is being generated to '{options.OutputPath}'", Verbosity.Minimal);

            foreach (DocumentationGeneratorResult documentationFile in generator.Generate(heading: options.Heading, cancellationToken))
            {
                string path = Path.Combine(directoryPath, documentationFile.FilePath);

                Directory.CreateDirectory(Path.GetDirectoryName(path));

                WriteLine($"  Save '{path}'", ConsoleColor.DarkGray, Verbosity.Detailed);

                File.WriteAllText(path, documentationFile.Content, _defaultEncoding);
            }

            WriteLine($"Documentation successfully generated to '{options.OutputPath}'.", Verbosity.Minimal);

            return 0;
        }

        private static int GenerateDeclarations(GenerateDeclarationsCommandLineOptions options)
        {
            if (!TryParseParameterValueAsEnum(options.Depth, ParameterNames.Depth, out DocumentationDepth depth, DeclarationListOptions.Default.Depth))
                return 1;

            if (!TryParseParameterValueAsEnumFlags(options.IgnoredParts, ParameterNames.IgnoredParts, out DeclarationListParts ignoredParts, DeclarationListOptions.Default.IgnoredParts))
                return 1;

            if (!TryParseParameterValueAsEnum(options.Visibility, ParameterNames.Visibility, out Visibility visibility))
                return 1;

            DocumentationModel documentationModel = CreateDocumentationModel(options.References, options.Assemblies, visibility, options.AdditionalXmlDocumentation);

            if (documentationModel == null)
                return 1;

            var declarationListOptions = new DeclarationListOptions(
                visibility: visibility,
                ignoredNames: options.IgnoredNames,
                indent: !options.NoIndent,
                indentChars: options.IndentChars,
                nestNamespaces: options.NestNamespaces,
                newLineBeforeOpenBrace: !options.NoNewLineBeforeOpenBrace,
                emptyLineBetweenMembers: options.EmptyLineBetweenMembers,
                formatBaseList: options.FormatBaseList,
                formatConstraints: options.FormatConstraints,
                formatParameters: options.FormatParameters,
                splitAttributes: !options.MergeAttributes,
                includeAttributeArguments: !options.OmitAttributeArguments,
                omitIEnumerable: !options.IncludeIEnumerable,
                useDefaultLiteral: !options.NoDefaultLiteral,
                fullyQualifiedNames: options.FullyQualifiedNames,
                depth: depth,
                ignoredParts: ignoredParts);

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            CancellationToken cancellationToken = cts.Token;

            WriteLine($"Declaration list is being generated to '{options.OutputPath}'.", Verbosity.Minimal);

            Task<string> task = DeclarationListGenerator.GenerateAsync(
                documentationModel,
                declarationListOptions,
                namespaceComparer: NamespaceSymbolDefinitionComparer.GetInstance(systemNamespaceFirst: !options.NoPrecedenceForSystem),
                cancellationToken: cancellationToken);

            string content = task.Result;

            string path = options.OutputPath;

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, content, Encoding.UTF8);

            WriteLine($"Declaration list successfully generated to '{options.OutputPath}'.", Verbosity.Minimal);

            return 0;
        }

        private static int GenerateDocRoot(GenerateDocRootCommandLineOptions options)
        {
            if (!TryParseParameterValueAsEnum(options.Visibility, ParameterNames.Visibility, out Visibility visibility))
                return 1;

            if (!TryParseParameterValueAsEnum(options.Depth, ParameterNames.Depth, out DocumentationDepth depth, DocumentationOptions.Default.Depth))
                return 1;

            if (!TryParseParameterValueAsEnumFlags(options.IgnoredParts, ParameterNames.IgnoredRootParts, out RootDocumentationParts ignoredParts, DocumentationOptions.Default.IgnoredRootParts))
                return 1;

            DocumentationModel documentationModel = CreateDocumentationModel(options.References, options.Assemblies, visibility);

            if (documentationModel == null)
                return 1;

            var documentationOptions = new DocumentationOptions(
                ignoredNames: options.IgnoredNames,
                rootDirectoryUrl: options.RootDirectoryUrl,
                includeClassHierarchy: !options.NoClassHierarchy,
                placeSystemNamespaceFirst: !options.NoPrecedenceForSystem,
                markObsolete: !options.NoMarkObsolete,
                depth: depth,
                ignoredRootParts: ignoredParts,
                omitContainingNamespaceParts: (options.OmitContainingNamespace) ? OmitContainingNamespaceParts.Root : OmitContainingNamespaceParts.None,
                scrollToContent: options.ScrollToContent);

            var generator = new MarkdownDocumentationGenerator(documentationModel, WellKnownUrlProviders.GitHub, documentationOptions);

            string path = options.OutputPath;

            WriteLine($"Documentation root is being generated to '{path}'.", Verbosity.Minimal);

            string heading = options.Heading;

            if (string.IsNullOrEmpty(heading))
            {
                string fileName = Path.GetFileName(options.OutputPath);

                heading = (fileName.EndsWith(".dll", StringComparison.Ordinal))
                    ? Path.GetFileNameWithoutExtension(fileName)
                    : fileName;
            }

            DocumentationGeneratorResult result = generator.GenerateRoot(heading);

            File.WriteAllText(path, result.Content, _defaultEncoding);

            WriteLine($"Documentation root successfully generated to '{path}'.", Verbosity.Minimal);

            return 0;
        }
    }
}
