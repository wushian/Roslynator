// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using Xunit;

namespace Roslynator.Test
{
    public static class CodeFixVerifier
    {
        public static void VerifyCSharpFix(
            string oldSource,
            string newSource,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider codeFixProvider,
            int? codeFixIndex = null,
            bool allowNewCompilerDiagnostics = false)
        {
            VerifyFix(LanguageNames.CSharp, analyzer, codeFixProvider, oldSource, newSource, codeFixIndex, allowNewCompilerDiagnostics);
        }

        public static void VerifyFix(
            string language,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider codeFixProvider,
            string oldSource,
            string newSource,
            int? codeFixIndex = null,
            bool allowNewCompilerDiagnostics = false)
        {
            Document document = TestUtility.GetDocument(oldSource, language);

            Diagnostic[] analyzerDiagnostics = TestUtility.GetSortedDiagnostics(analyzer, document);

            IEnumerable<Diagnostic> compilerDiagnostics = GetCompilerDiagnostics(document);

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

                if (codeFixIndex != null)
                {
                    document = ApplyFix(document, actions[(int)codeFixIndex]);
                    break;
                }

                document = ApplyFix(document, actions[0]);
                analyzerDiagnostics = TestUtility.GetSortedDiagnostics(analyzer, document);

                IEnumerable<Diagnostic> newCompilerDiagnostics = GetNewDiagnostics(compilerDiagnostics, GetCompilerDiagnostics(document));

                if (!allowNewCompilerDiagnostics
                    && newCompilerDiagnostics.Any())
                {
                    document = document.WithSyntaxRoot(Formatter.Format(document.GetSyntaxRootAsync().Result, Formatter.Annotation, document.Project.Solution.Workspace));
                    newCompilerDiagnostics = GetNewDiagnostics(compilerDiagnostics, GetCompilerDiagnostics(document));

                    string diagnostics = string.Join("\r\n", newCompilerDiagnostics.Select(d => d.ToString()));

                    Assert.True(false,
                        $"Fix introduced new compiler diagnostics:\r\n{diagnostics}\r\n\r\nNew document:\r\n{document.GetSyntaxRootAsync().Result.ToFullString()}\r\n");
                }
            }

            string actual = GetStringFromDocument(document);

            Assert.Equal(newSource, actual);
        }

        public static Document ApplyFix(Document document, CodeAction codeAction)
        {
            return codeAction
                .GetOperationsAsync(CancellationToken.None)
                .Result
                .OfType<ApplyChangesOperation>()
                .Single()
                .ChangedSolution
                .GetDocument(document.Id);
        }

        public static IEnumerable<Diagnostic> GetNewDiagnostics(
            IEnumerable<Diagnostic> diagnostics,
            IEnumerable<Diagnostic> newDiagnostics)
        {
            using (IEnumerator<Diagnostic> en = newDiagnostics.OrderBy(f => f.Location.SourceSpan.Start).GetEnumerator())
            {
                using (IEnumerator<Diagnostic> e = diagnostics.OrderBy(f => f.Location.SourceSpan.Start).GetEnumerator())
                {
                    while (en.MoveNext())
                    {
                        if (e.MoveNext())
                        {
                            if (e.Current.Id != en.Current.Id)
                                yield return en.Current;
                        }
                        else
                        {
                            yield return en.Current;

                            while (en.MoveNext())
                                yield return en.Current;

                            yield break;
                        }
                    }
                }
            }
        }

        public static IEnumerable<Diagnostic> GetCompilerDiagnostics(Document document)
        {
            return document.GetSemanticModelAsync().Result.GetDiagnostics();
        }

        public static string GetStringFromDocument(Document document)
        {
            Document simplifiedDocument = Simplifier.ReduceAsync(document, Simplifier.Annotation).Result;

            SyntaxNode root = simplifiedDocument.GetSyntaxRootAsync().Result;

            root = Formatter.Format(root, Formatter.Annotation, simplifiedDocument.Project.Solution.Workspace);

            return root.GetText().ToString();
        }
    }
}
