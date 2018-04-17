// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Tests
{
    public static class CSharpCodeRefactoringVerifier
    {
        public static void VerifyNoRefactoring(
            string source,
            CodeRefactoringProvider codeRefactoringProvider,
            string equivalenceKey = null)
        {
            (string source2, TextSpan span) = TextUtility.GetMarkedSpan(source);

            VerifyNoRefactoring(
                source: source2,
                span: span,
                codeRefactoringProvider: codeRefactoringProvider,
                equivalenceKey: equivalenceKey);
        }

        public static void VerifyNoRefactoring(
            string source,
            TextSpan span,
            CodeRefactoringProvider codeRefactoringProvider,
            string equivalenceKey = null)
        {
            CodeRefactoringVerifier.VerifyNoRefactoring(
                source: source,
                span: span,
                codeRefactoringProvider: codeRefactoringProvider,
                language: LanguageNames.CSharp,
                equivalenceKey: equivalenceKey);
        }

        public static void VerifyRefactoring(
            string source,
            string newSource,
            CodeRefactoringProvider codeRefactoringProvider,
            string equivalenceKey = null,
            bool allowNewCompilerDiagnostics = false)
        {
            (string source2, TextSpan span) = TextUtility.GetMarkedSpan(source);

            VerifyRefactoring(
                source: source2,
                newSource: newSource,
                span: span,
                codeRefactoringProvider: codeRefactoringProvider,
                equivalenceKey: equivalenceKey,
                allowNewCompilerDiagnostics: allowNewCompilerDiagnostics);
        }

        public static void VerifyRefactoring(
            string source,
            string newSource,
            TextSpan span,
            CodeRefactoringProvider codeRefactoringProvider,
            string equivalenceKey = null,
            bool allowNewCompilerDiagnostics = false)
        {
            CodeRefactoringVerifier.VerifyRefactoring(
                source: source,
                newSource: newSource,
                span: span,
                codeRefactoringProvider: codeRefactoringProvider,
                language: LanguageNames.CSharp,
                equivalenceKey: equivalenceKey,
                allowNewCompilerDiagnostics: allowNewCompilerDiagnostics);
        }
    }
}
