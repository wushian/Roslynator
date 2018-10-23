// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics.Telemetry;

namespace Roslynator
{
    internal static class Extensions
    {
        public static bool IsAnalyzerExceptionDiagnostic(this Diagnostic diagnostic)
        {
            if (diagnostic.Id == "AD0001"
                || diagnostic.Id == "AD0002")
            {
                foreach (string tag in diagnostic.Descriptor.CustomTags)
                {
                    if (tag == WellKnownDiagnosticTags.AnalyzerException)
                        return true;
                }
            }

            return false;
        }

        public static bool IsFadeDiagnostic(this Diagnostic diagnostic)
        {
            return diagnostic.Id.EndsWith("FadedToken", StringComparison.Ordinal)
                || diagnostic.Id.EndsWith("FadeOut", StringComparison.Ordinal);
        }

        public static void Add(this AnalyzerTelemetryInfo telemetryInfo, AnalyzerTelemetryInfo telemetryInfoToAdd)
        {
            telemetryInfo.CodeBlockActionsCount += telemetryInfoToAdd.CodeBlockActionsCount;
            telemetryInfo.CodeBlockEndActionsCount += telemetryInfoToAdd.CodeBlockEndActionsCount;
            telemetryInfo.CodeBlockStartActionsCount += telemetryInfoToAdd.CodeBlockStartActionsCount;
            telemetryInfo.CompilationActionsCount += telemetryInfoToAdd.CompilationActionsCount;
            telemetryInfo.CompilationEndActionsCount += telemetryInfoToAdd.CompilationEndActionsCount;
            telemetryInfo.CompilationStartActionsCount += telemetryInfoToAdd.CompilationStartActionsCount;
            telemetryInfo.ExecutionTime += telemetryInfoToAdd.ExecutionTime;
            telemetryInfo.OperationActionsCount += telemetryInfoToAdd.OperationActionsCount;
            telemetryInfo.OperationBlockActionsCount += telemetryInfoToAdd.OperationBlockActionsCount;
            telemetryInfo.OperationBlockEndActionsCount += telemetryInfoToAdd.OperationBlockEndActionsCount;
            telemetryInfo.OperationBlockStartActionsCount += telemetryInfoToAdd.OperationBlockStartActionsCount;
            telemetryInfo.SemanticModelActionsCount += telemetryInfoToAdd.SemanticModelActionsCount;
            telemetryInfo.SymbolActionsCount += telemetryInfoToAdd.SymbolActionsCount;
            telemetryInfo.SyntaxNodeActionsCount += telemetryInfoToAdd.SyntaxNodeActionsCount;
            telemetryInfo.SyntaxTreeActionsCount += telemetryInfoToAdd.SyntaxTreeActionsCount;
        }

        public static ReportDiagnostic ToReportDiagnostic(this DiagnosticSeverity diagnosticSeverity)
        {
            switch (diagnosticSeverity)
            {
                case DiagnosticSeverity.Hidden:
                    return ReportDiagnostic.Hidden;
                case DiagnosticSeverity.Info:
                    return ReportDiagnostic.Info;
                case DiagnosticSeverity.Warning:
                    return ReportDiagnostic.Warn;
                case DiagnosticSeverity.Error:
                    return ReportDiagnostic.Error;
                default:
                    throw new ArgumentException("", nameof(diagnosticSeverity));
            }
        }

        public static DiagnosticSeverity ToDiagnosticSeverity(this ReportDiagnostic reportDiagnostic)
        {
            switch (reportDiagnostic)
            {
                case ReportDiagnostic.Error:
                    return DiagnosticSeverity.Error;
                case ReportDiagnostic.Warn:
                    return DiagnosticSeverity.Warning;
                case ReportDiagnostic.Info:
                    return DiagnosticSeverity.Info;
                case ReportDiagnostic.Hidden:
                    return DiagnosticSeverity.Hidden;
                default:
                    throw new ArgumentException("", nameof(reportDiagnostic));
            }
        }
    }
}
