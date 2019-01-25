// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CommandLine
{
    internal class DiagnosticMap
    {
        private OneOrMany<AnalyzerAssembly> _analyzerAssemblies;

        private ImmutableArray<DiagnosticAnalyzer> _analyzers;
        private ImmutableArray<CodeFixProvider> _fixers;
        private ImmutableDictionary<string, DiagnosticDescriptor> _diagnosticsById;
        private ImmutableDictionary<string, IEnumerable<DiagnosticAnalyzer>> _analyzersById;
        private ImmutableDictionary<string, IEnumerable<CodeFixProvider>> _fixersById;
        private ImmutableArray<FixAllProvider> _fixAllProviders;

        public DiagnosticMap(AnalyzerAssembly analyzerAssembly)
        {
            _analyzerAssemblies = new OneOrMany<AnalyzerAssembly>(analyzerAssembly);
        }

        public DiagnosticMap(IEnumerable<AnalyzerAssembly> analyzerAssemblies)
        {
            _analyzerAssemblies = new OneOrMany<AnalyzerAssembly>(analyzerAssemblies.ToImmutableArray());
        }

        public ImmutableArray<DiagnosticAnalyzer> Analyzers
        {
            get
            {
                if (_analyzers.IsDefault)
                    ImmutableInterlocked.InterlockedInitialize(ref _analyzers, LoadAnalyzers());

                return _analyzers;
            }
        }

        public ImmutableArray<CodeFixProvider> Fixers
        {
            get
            {
                if (_fixers.IsDefault)
                    ImmutableInterlocked.InterlockedInitialize(ref _fixers, LoadFixers());

                return _fixers;
            }
        }

        public ImmutableDictionary<string, DiagnosticDescriptor> DiagnosticsById
        {
            get
            {
                if (_diagnosticsById == null)
                    Interlocked.CompareExchange(ref _diagnosticsById, LoadDiagnosticsById(), null);

                return _diagnosticsById;
            }
        }

        public ImmutableDictionary<string, IEnumerable<DiagnosticAnalyzer>> AnalyzersById
        {
            get
            {
                if (_analyzersById == null)
                    Interlocked.CompareExchange(ref _analyzersById, LoadAnalyzersById(), null);

                return _analyzersById;
            }
        }

        public ImmutableDictionary<string, IEnumerable<CodeFixProvider>> FixersById
        {
            get
            {
                if (_fixersById == null)
                    Interlocked.CompareExchange(ref _fixersById, LoadFixersById(), null);

                return _fixersById;
            }
        }

        public ImmutableArray<FixAllProvider> FixAllProviders
        {
            get
            {
                if (_fixAllProviders.IsDefault)
                    ImmutableInterlocked.InterlockedInitialize(ref _fixAllProviders, LoadFixAllProviders());

                return _fixAllProviders;
            }
        }

        private ImmutableArray<DiagnosticAnalyzer> LoadAnalyzers()
        {
            if (_analyzerAssemblies.Count == 1)
            {
                return _analyzerAssemblies[0].Analyzers;
            }
            else
            {
                return _analyzerAssemblies.SelectMany(f => f.Analyzers).ToImmutableArray();
            }
        }

        private ImmutableArray<CodeFixProvider> LoadFixers()
        {
            if (_analyzerAssemblies.Count == 1)
            {
                return _analyzerAssemblies[0].Fixers;
            }
            else
            {
                return _analyzerAssemblies.SelectMany(f => f.Fixers).ToImmutableArray();
            }
        }

        private ImmutableDictionary<string, DiagnosticDescriptor> LoadDiagnosticsById()
        {
            ImmutableDictionary<string, DiagnosticDescriptor>.Builder diagnosticsById = ImmutableDictionary.CreateBuilder<string, DiagnosticDescriptor>();

            diagnosticsById.AddRange(Analyzers
                .SelectMany(f => f.SupportedDiagnostics)
                .Distinct(DiagnosticDescriptorComparer.Id)
                .OrderBy(f => f, DiagnosticDescriptorComparer.Id)
                .Select(f => new KeyValuePair<string, DiagnosticDescriptor>(f.Id, f)));

            foreach (CodeFixProvider fixer in Fixers)
            {
                foreach (string diagnosticId in fixer.FixableDiagnosticIds)
                {
                    if (!diagnosticsById.ContainsKey(diagnosticId))
                        diagnosticsById[diagnosticId] = null;
                }
            }

            return diagnosticsById.ToImmutable();
        }

        private ImmutableDictionary<string, IEnumerable<DiagnosticAnalyzer>> LoadAnalyzersById()
        {
            return Analyzers
                .SelectMany(analyzer => analyzer.SupportedDiagnostics.Select(descriptor => (analyzer, descriptor)))
                .GroupBy(f => f.descriptor.Id)
                .ToImmutableDictionary(g => g.Key, g => g.Select(f => f.analyzer).Distinct());
        }

        private ImmutableDictionary<string, IEnumerable<CodeFixProvider>> LoadFixersById()
        {
            return Fixers
                .SelectMany(fixer => fixer.FixableDiagnosticIds.Select(diagnosticId => (fixer, diagnosticId)))
                .GroupBy(f => f.diagnosticId)
                .ToImmutableDictionary(g => g.Key, g => g.Select(f => f.fixer).Distinct());
        }

        private ImmutableArray<FixAllProvider> LoadFixAllProviders()
        {
            return Fixers
                .Select(f => f.GetFixAllProvider())
                .Where(f => f != null)
                .Distinct()
                .ToImmutableArray();
        }
    }
}
