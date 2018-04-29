// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator
{
    public static class CompilationExtensions
    {
        public static Compilation EnableDiagnosticsDisabledByDefault(this Compilation compilation, DiagnosticAnalyzer analyzer)
        {
            return EnableDiagnosticsDisabledByDefault(compilation, analyzer.SupportedDiagnostics);
        }

        public static Compilation EnableDiagnosticsDisabledByDefault(this Compilation compilation, ImmutableArray<DiagnosticDescriptor> diagnosticDescriptors)
        {
            foreach (DiagnosticDescriptor descriptor in diagnosticDescriptors)
            {
                if (descriptor.IsEnabledByDefault)
                    continue;

                CompilationOptions compilationOptions = compilation.Options;
                ImmutableDictionary<string, ReportDiagnostic> specificDiagnosticOptions = compilationOptions.SpecificDiagnosticOptions;

                specificDiagnosticOptions = specificDiagnosticOptions.Add(descriptor.Id, descriptor.DefaultSeverity.ToReportDiagnostic());
                CompilationOptions options = compilationOptions.WithSpecificDiagnosticOptions(specificDiagnosticOptions);

                compilation = compilation.WithOptions(options);
            }

            return compilation;
        }

        public static Task<ImmutableArray<Diagnostic>> GetAnalyzerDiagnosticsAsync(
            this Compilation compilation,
            DiagnosticAnalyzer analyzer,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            CompilationWithAnalyzers compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create(analyzer), default(AnalyzerOptions), cancellationToken);

            return compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(cancellationToken);
        }
    }
}
