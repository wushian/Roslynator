// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.CodeFixes.ConsoleHelpers;

namespace Roslynator.CodeFixes
{
    internal sealed class AnalyzerAssemblyCache
    {
        private readonly Dictionary<string, AnalyzerAssembly> _analyzerAssemblies;

        public AnalyzerAssemblyCache()
        {
            _analyzerAssemblies = new Dictionary<string, AnalyzerAssembly>();
        }

        internal bool Contains(string fullName)
        {
            return _analyzerAssemblies.ContainsKey(fullName);
        }

        internal void LoadFrom(IEnumerable<string> paths, bool loadAnalyzers = true, bool loadFixers = true)
        {
            foreach (string path in paths)
                LoadFrom(path, loadAnalyzers: loadAnalyzers, loadFixers: loadFixers);
        }

        internal void LoadFrom(string path, bool loadAnalyzers = true, bool loadFixers = true)
        {
            foreach (AnalyzerAssembly analyzerAssembly in AssemblyAnalyzer.Analyze(path, loadAnalyzers: loadAnalyzers, loadFixers: loadFixers))
            {
                Add(analyzerAssembly);
            }
        }

        public bool Add(AnalyzerAssembly analyzerAssembly)
        {
            if (!_analyzerAssemblies.ContainsKey(analyzerAssembly.Assembly.FullName))
            {
                AddImpl(analyzerAssembly);
                return true;
            }

            return false;
        }

        private void AddImpl(AnalyzerAssembly analyzerAssembly)
        {
            WriteLine($"Add analyzer assembly '{analyzerAssembly.Assembly.FullName}'");

            _analyzerAssemblies.Add(analyzerAssembly.Assembly.FullName, analyzerAssembly);
        }

        public AnalyzerAssembly GetOrAdd(Assembly assembly)
        {
            if (!_analyzerAssemblies.TryGetValue(assembly.FullName, out AnalyzerAssembly analyzerAssembly))
            {
                analyzerAssembly = AnalyzerAssembly.Load(assembly);
                AddImpl(analyzerAssembly);
                return analyzerAssembly;
            }

            return analyzerAssembly;
        }

        public ImmutableArray<DiagnosticAnalyzer> GetAnalyzers(string language)
        {
            return GetAnalyzers(_analyzerAssemblies.Values, language);
        }

        public ImmutableArray<DiagnosticAnalyzer> GetAnalyzers(IEnumerable<Assembly> assemblies, string language)
        {
            return GetAnalyzers(assemblies.Select(GetOrAdd), language);
        }

        private static ImmutableArray<DiagnosticAnalyzer> GetAnalyzers(IEnumerable<AnalyzerAssembly> analyzerAssemblies, string language)
        {
            return analyzerAssemblies
                .SelectMany(f => f.Analyzers)
                .Where(f => f.Key == language)
                .SelectMany(f => f.Value)
                .ToImmutableArray();
        }

        public ImmutableArray<CodeFixProvider> GetFixers(string language)
        {
            return GetFixers(_analyzerAssemblies.Values, language);
        }

        public ImmutableArray<CodeFixProvider> GetFixers(IEnumerable<Assembly> assemblies, string language)
        {
            return GetFixers(assemblies.Select(GetOrAdd), language);
        }

        public static ImmutableArray<CodeFixProvider> GetFixers(IEnumerable<AnalyzerAssembly> analyzerAssemblies, string language)
        {
            return analyzerAssemblies
                .SelectMany(f => f.Fixers)
                .Where(f => f.Key == language)
                .SelectMany(f => f.Value)
                .ToImmutableArray();
        }
    }
}
