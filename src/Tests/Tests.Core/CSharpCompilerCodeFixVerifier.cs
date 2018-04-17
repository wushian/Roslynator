// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Roslynator.Tests
{
    public static class CSharpCompilerCodeFixVerifier
    {
        public static void VerifyNoFix(
            string source,
            CodeFixProvider codeFixProvider,
            string equivalenceKey = null)
        {
            CompilerCodeFixVerifier.VerifyNoFix(
                source: source,
                codeFixProvider: codeFixProvider,
                language: LanguageNames.CSharp,
                equivalenceKey: equivalenceKey);
        }

        public static void VerifyFix(
            string source,
            string newSource,
            CodeFixProvider codeFixProvider,
            string equivalenceKey = null)
        {
            CompilerCodeFixVerifier.VerifyFix(
                source: source,
                newSource: newSource,
                codeFixProvider: codeFixProvider,
                language: LanguageNames.CSharp,
                equivalenceKey: equivalenceKey);
        }
    }
}
