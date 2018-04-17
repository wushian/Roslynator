// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Formatting;
using Xunit;

namespace Roslynator.Tests
{
    public static class CodeFixVerifier
    {
        public static void VerifyNoFix(
            string source,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider codeFixProvider,
            string language)
        {
            Document document = WorkspaceUtility.GetDocument(source, language);

            Diagnostic[] analyzerDiagnostics = DiagnosticUtility.GetSortedDiagnostics(analyzer, document);

            List<CodeAction> actions = null;

            var context = new CodeFixContext(
                document,
                analyzerDiagnostics[0],
                (a, _) => (actions ?? (actions = new List<CodeAction>())).Add(a),
                CancellationToken.None);

            codeFixProvider.RegisterCodeFixesAsync(context).Wait();

            Assert.True(actions == null, $"Expected no code fix, actual: {actions.Count}.");
        }

        public static void VerifyFix(
            string source,
            string newSource,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider codeFixProvider,
            string language,
            bool allowNewCompilerDiagnostics = false)
        {
            Document document = WorkspaceUtility.GetDocument(source, language);

            Diagnostic[] analyzerDiagnostics = DiagnosticUtility.GetSortedDiagnostics(analyzer, document);

            ImmutableArray<Diagnostic> compilerDiagnostics = DiagnosticUtility.GetCompilerDiagnostics(document);

            while (analyzerDiagnostics.Length > 0)
            {
                var actions = new List<CodeAction>();

                var context = new CodeFixContext(
                    document,
                    analyzerDiagnostics[0],
                    (a, _) => actions.Add(a),
                    CancellationToken.None);

                codeFixProvider.RegisterCodeFixesAsync(context).Wait();

                if (actions.Count == 0)
                    break;

                document = WorkspaceUtility.ApplyCodeAction(document, actions[0]);

                analyzerDiagnostics = DiagnosticUtility.GetSortedDiagnostics(analyzer, document);

                IEnumerable<Diagnostic> newCompilerDiagnostics = DiagnosticUtility.GetNewDiagnostics(compilerDiagnostics, DiagnosticUtility.GetCompilerDiagnostics(document));

                if (!allowNewCompilerDiagnostics
                    && newCompilerDiagnostics.Any())
                {
                    document = document.WithSyntaxRoot(Formatter.Format(document.GetSyntaxRootAsync().Result, Formatter.Annotation, document.Project.Solution.Workspace));
                    newCompilerDiagnostics = DiagnosticUtility.GetNewDiagnostics(compilerDiagnostics, DiagnosticUtility.GetCompilerDiagnostics(document));

                    string diagnostics = string.Join("\r\n", newCompilerDiagnostics.Select(d => d.ToString()));

                    Assert.True(false,
                        $"Fix introduced new compiler diagnostics:\r\n{diagnostics}\r\n\r\nNew document:\r\n{document.GetSyntaxRootAsync().Result.ToFullString()}\r\n");
                }
            }

            string actual = WorkspaceUtility.GetSimplifiedAndFormattedText(document);

            Assert.Equal(newSource, actual);
        }
    }
}
