// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal class AnalyzeAssemblyCommand
    {
        public AnalyzeAssemblyCommand(string language = null)
        {
            Language = language;
        }

        public string Language { get; }

        public CommandResult Execute(AnalyzeAssemblyCommandLineOptions options)
        {
            var assemblies = new HashSet<Assembly>();

            List<DiagnosticAnalyzer> allAnalyzers = (options.NoAnalyzers) ? null : new List<DiagnosticAnalyzer>();
            List<CodeFixProvider> allFixers = (options.NoFixers) ? null : new List<CodeFixProvider>();

            AnalyzerAssemblyInfo[] analyzerAssemblies = options.GetPaths()
                .SelectMany(path => AnalyzerAssemblyLoader.LoadFrom(
                    path: path,
                    loadAnalyzers: !options.NoAnalyzers,
                    loadFixers: !options.NoFixers,
                    language: Language))
                .OrderBy(f => f.AnalyzerAssembly.GetName().Name)
                .ThenBy(f => f.FilePath)
                .ToArray();

            for (int i = 0; i < analyzerAssemblies.Length; i++)
            {
                AnalyzerAssembly analyzerAssembly = analyzerAssemblies[i].AnalyzerAssembly;

                if (assemblies.Add(analyzerAssembly.Assembly))
                {
                    WriteLine($"{analyzerAssembly.FullName}", ConsoleColor.Cyan, Verbosity.Minimal);

                    if (ShouldWrite(Verbosity.Normal))
                    {
                        (DiagnosticAnalyzer[] analyzers, CodeFixProvider[] fixers) = WriteAnalyzerAssembly(analyzerAssemblies[i], writeAnalyzers: !options.NoAnalyzers, writeFixers: !options.NoFixers);

                        allAnalyzers?.AddRange(analyzers);
                        allFixers?.AddRange(fixers);

                        if (i < analyzerAssemblies.Length - 1)
                            WriteLine(Verbosity.Normal);
                    }
                }
                else
                {
                    Write($"{analyzerAssembly.FullName}", ConsoleColor.DarkGray, Verbosity.Minimal);
                    WriteLine($" [{analyzerAssemblies[i].FilePath}]", ConsoleColor.DarkGray, Verbosity.Minimal);
                }
            }

            if (ShouldWrite(Verbosity.Detailed))
            {
                WriteLine(Verbosity.Detailed);
                WriteDiagnostics(allAnalyzers, allFixers);
            }

            WriteLine(Verbosity.Minimal);
            WriteLine($"{assemblies.Count} analyzer {((assemblies.Count == 1) ? "assembly" : "assemblies")} found", ConsoleColor.Green, Verbosity.Minimal);
            WriteLine(Verbosity.Minimal);

            return CommandResult.Success;
        }

        private static (DiagnosticAnalyzer[] analyzers, CodeFixProvider[] fixers) WriteAnalyzerAssembly(AnalyzerAssemblyInfo analyzerAssemblyInfo, bool writeAnalyzers, bool writeFixers)
        {
            AnalyzerAssembly analyzerAssembly = analyzerAssemblyInfo.AnalyzerAssembly;

            WriteLine(Verbosity.Normal);
            WriteLine($"  Path:                 {analyzerAssemblyInfo.FilePath}", Verbosity.Normal);

            DiagnosticAnalyzer[] analyzers = Array.Empty<DiagnosticAnalyzer>();
            DiagnosticDescriptor[] supportedDiagnostics = Array.Empty<DiagnosticDescriptor>();

            if (writeAnalyzers)
            {
                analyzers = analyzerAssembly
                    .Analyzers
                    .SelectMany(f => f.Value)
                    .Distinct()
                    .ToArray();

                if (analyzers.Length > 0)
                {
                    supportedDiagnostics = analyzers
                        .SelectMany(f => f.SupportedDiagnostics)
                        .Distinct(DiagnosticDescriptorComparer.Id)
                        .OrderBy(f => f, DiagnosticDescriptorComparer.Id)
                        .ToArray();

                    WriteLine($"  DiagnosticAnalyzers:  {analyzers.Length}", Verbosity.Normal);

                    int maxLanguageLength = analyzerAssembly.Analyzers.Max(f => GetShortLanguageName(f.Key).Length);

                    foreach (KeyValuePair<string, ImmutableArray<DiagnosticAnalyzer>> kvp in analyzerAssembly.Analyzers.OrderBy(f => f.Key))
                    {
                        WriteLine($"    {GetShortLanguageName(kvp.Key).PadRight(maxLanguageLength)} {kvp.Value.Length} ", Verbosity.Normal);
                    }

                    WriteLine($"  SupportedDiagnostics: {supportedDiagnostics.Length}", Verbosity.Normal);

                    (string prefix, int count)[] prefixes = DiagnosticIdPrefix.CountPrefixes(supportedDiagnostics.Select(f => f.Id)).ToArray();

                    int maxPrefixLength = prefixes.Max(f => f.prefix.Length);

                    foreach ((string prefix, int count) in prefixes)
                    {
                        WriteLine($"    {prefix.PadRight(maxLanguageLength)} {count} ", Verbosity.Normal);
                    }
                }
            }

            CodeFixProvider[] fixers = Array.Empty<CodeFixProvider>();
            string[] fixableDiagnosticIds = Array.Empty<string>();

            if (writeFixers)
            {
                fixers = analyzerAssembly
                   .Fixers
                   .SelectMany(f => f.Value)
                   .Distinct()
                   .ToArray();

                if (fixers.Length > 0)
                {
                    fixableDiagnosticIds = fixers
                        .SelectMany(f => f.FixableDiagnosticIds)
                        .Distinct()
                        .OrderBy(f => f)
                        .ToArray();

                    WriteLine($"  CodeFixProviders:     {fixers.Length}", Verbosity.Normal);

                    int maxLanguageLength = analyzerAssembly.Fixers.Max(f => GetShortLanguageName(f.Key).Length);

                    foreach (KeyValuePair<string, ImmutableArray<CodeFixProvider>> kvp in analyzerAssembly.Fixers.OrderBy(f => f.Key))
                    {
                        WriteLine($"    {GetShortLanguageName(kvp.Key).PadRight(maxLanguageLength)} {kvp.Value.Length}", Verbosity.Normal);
                    }

                    WriteLine($"  FixableDiagnosticIds: {fixableDiagnosticIds.Length}", Verbosity.Normal);

                    (string prefix, int count)[] prefixes = DiagnosticIdPrefix.CountPrefixes(fixableDiagnosticIds).ToArray();

                    int maxPrefixLength = prefixes.Max(f => f.prefix.Length);

                    foreach ((string prefix, int count) in prefixes)
                    {
                        WriteLine($"    {prefix.PadRight(maxPrefixLength)} {count} ", Verbosity.Normal);
                    }
                }
            }

            if (ShouldWrite(Verbosity.Detailed))
            {
                if (analyzers.Length > 0)
                {
                    WriteLine(Verbosity.Detailed);
                    WriteDiagnosticAnalyzers(analyzers);
                }

                if (fixers.Length > 0)
                {
                    WriteLine(Verbosity.Detailed);
                    WriteCodeFixProviders(fixers);
                }
            }

            return (analyzers, fixers);
        }

        private static void WriteDiagnosticAnalyzers(DiagnosticAnalyzer[] analyzers)
        {
            WriteLine("  DiagnosticAnalyzers:", Verbosity.Detailed);

            foreach (DiagnosticAnalyzer analyzer in analyzers.OrderBy(f => f.GetType(), TypeComparer.NamespaceThenName))
            {
                Type type = analyzer.GetType();

                DiagnosticAnalyzerAttribute attribute = type.GetCustomAttribute<DiagnosticAnalyzerAttribute>();

                WriteLine($"    {type.FullName}", Verbosity.Detailed);

                if (ShouldWrite(Verbosity.Diagnostic))
                {
                    WriteLine($"      Languages:            {string.Join(", ", attribute.Languages.Select(f => GetShortLanguageName(f)).OrderBy(f => f))}", ConsoleColor.DarkGray, Verbosity.Diagnostic);
                    WriteLine($"      SupportedDiagnostics: {string.Join(", ", analyzer.SupportedDiagnostics.Select(f => f.Id).OrderBy(f => f))}", ConsoleColor.DarkGray, Verbosity.Diagnostic);
                }
            }
        }

        private static void WriteDiagnostics(IList<DiagnosticAnalyzer> analyzers, IList<CodeFixProvider> fixers)
        {
            if (analyzers == null)
                analyzers = Array.Empty<DiagnosticAnalyzer>();

            if (fixers == null)
                fixers = Array.Empty<CodeFixProvider>();

            Dictionary<string, DiagnosticDescriptor> diagnosticIds = analyzers
                .SelectMany(f => f.SupportedDiagnostics)
                .Distinct(DiagnosticDescriptorComparer.Id)
                .OrderBy(f => f, DiagnosticDescriptorComparer.Id)
                .ToDictionary(f => f.Id, f => f);

            foreach (CodeFixProvider fixer in fixers)
            {
                foreach (string diagnosticId in fixer.FixableDiagnosticIds)
                {
                    if (!diagnosticIds.ContainsKey(diagnosticId))
                        diagnosticIds[diagnosticId] = null;
                }
            }

            Dictionary<string, IEnumerable<DiagnosticAnalyzer>> analyzersById = analyzers
                .SelectMany(analyzer => analyzer.SupportedDiagnostics.Select(descriptor => (analyzer, descriptor)))
                .GroupBy(f => f.descriptor.Id)
                .ToDictionary(g => g.Key, g => g.Select(f => f.analyzer));

            Dictionary<string, IEnumerable<CodeFixProvider>> fixersById = fixers
                .SelectMany(fixer => fixer.FixableDiagnosticIds.Select(diagnosticId => (fixer, diagnosticId)))
                .GroupBy(f => f.diagnosticId)
                .ToDictionary(g => g.Key, g => g.Select(f => f.fixer));

            WriteLine("  Diagnostics:", Verbosity.Detailed);

            foreach (KeyValuePair<string, DiagnosticDescriptor> kvp in diagnosticIds.OrderBy(f => f.Key))
            {
                string diagnosticId = kvp.Key;
                DiagnosticDescriptor descriptor = kvp.Value;

                if (descriptor == null)
                {
                    WriteLine($"    {diagnosticId}", Verbosity.Detailed);
                }
                else
                {
                    string title = descriptor.Title?.ToString();
                    string messageFormat = descriptor.MessageFormat?.ToString();

                    if (string.IsNullOrEmpty(title))
                        title = messageFormat;

                    WriteLine($"    {diagnosticId} {title}", Verbosity.Detailed);

                    if (ShouldWrite(Verbosity.Diagnostic))
                    {
                        if (title != messageFormat)
                            WriteLine($"      MessageFormat:       {messageFormat}", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                        WriteLine($"      Category:            {descriptor.Category}", ConsoleColor.DarkGray, Verbosity.Diagnostic);
                        WriteLine($"      DefaultSeverity:     {descriptor.DefaultSeverity}", ConsoleColor.DarkGray, Verbosity.Diagnostic);
                        WriteLine($"      IsEnabledByDefault:  {descriptor.IsEnabledByDefault}", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                        string description = descriptor.Description?.ToString();

                        if (!string.IsNullOrEmpty(description))
                            WriteLine($"      Description:         {description}", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                        if (!string.IsNullOrEmpty(descriptor.HelpLinkUri))
                            WriteLine($"      HelpLinkUri:         {descriptor.HelpLinkUri}", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                        if (descriptor.CustomTags?.Any() == true)
                            WriteLine($"      CustomTags:          {string.Join(", ", descriptor.CustomTags)}", ConsoleColor.DarkGray, Verbosity.Diagnostic);
                    }
                }

                if (ShouldWrite(Verbosity.Diagnostic))
                {
                    if (analyzersById.TryGetValue(diagnosticId, out IEnumerable<DiagnosticAnalyzer> analyzers2))
                    {
                        Write("      DiagnosticAnalyzers: ", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                        WriteTypes(analyzers2.Select(f => f.GetType()));
                    }

                    if (fixersById.TryGetValue(diagnosticId, out IEnumerable<CodeFixProvider> fixers2))
                    {
                        Write("      CodeFixProviders:    ", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                        WriteTypes(fixers2.Select(f => f.GetType()));
                    }
                }
            }

            void WriteTypes(IEnumerable<Type> types)
            {
                using (IEnumerator<Type> en = types.OrderBy(f => f, TypeComparer.NamespaceThenName).GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        while (true)
                        {
                            WriteLine(en.Current.FullName, ConsoleColor.DarkGray, Verbosity.Diagnostic);

                            if (en.MoveNext())
                            {
                                Write("                           ", ConsoleColor.DarkGray, Verbosity.Diagnostic);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        private static void WriteCodeFixProviders(CodeFixProvider[] fixers)
        {
            WriteLine("  CodeFixProviders:", Verbosity.Detailed);

            foreach (CodeFixProvider fixer in fixers.OrderBy(f => f.GetType(), TypeComparer.NamespaceThenName))
            {
                Type type = fixer.GetType();

                ExportCodeFixProviderAttribute attribute = type.GetCustomAttribute<ExportCodeFixProviderAttribute>();

                WriteLine($"    {type.FullName}", Verbosity.Detailed);

                if (ShouldWrite(Verbosity.Diagnostic))
                {
                    WriteLine($"      Languages:            {string.Join(", ", attribute.Languages.Select(f => GetShortLanguageName(f)).OrderBy(f => f))}", ConsoleColor.DarkGray, Verbosity.Diagnostic);
                    WriteLine($"      FixableDiagnosticIds: {string.Join(", ", fixer.FixableDiagnosticIds.OrderBy(f => f))}", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                    Write("      FixAllProvider:       ", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                    FixAllProvider fixAllProvider = fixer.GetFixAllProvider();

                    if (fixAllProvider != null)
                    {
                        WriteLine($"{fixAllProvider.GetType().FullName} ({string.Join(", ", fixAllProvider.GetSupportedFixAllScopes().Select(f => f.ToString()).OrderBy(f => f))})", ConsoleColor.DarkGray, Verbosity.Diagnostic);
                    }
                    else
                    {
                        WriteLine("-", ConsoleColor.DarkGray, Verbosity.Diagnostic);
                    }
                }
            }
        }

        private static string GetShortLanguageName(string languageName)
        {
            switch (languageName)
            {
                case LanguageNames.CSharp:
                case LanguageNames.FSharp:
                    return languageName;
                case LanguageNames.VisualBasic:
                    return "VB";
            }

            Debug.Fail(languageName);

            return languageName;
        }
    }
}
