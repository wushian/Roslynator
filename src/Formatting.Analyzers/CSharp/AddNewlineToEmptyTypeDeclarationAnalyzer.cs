// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class AddNewLineToEmptyTypeDeclarationAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddNewLineToEmptyTypeDeclaration); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration, SyntaxKind.InterfaceDeclaration);
        }

        private static void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            if (typeDeclaration.Members.Any())
                return;

            SyntaxToken openBrace = typeDeclaration.OpenBraceToken;

            if (openBrace.IsMissing)
                return;

            SyntaxToken closeBrace = typeDeclaration.CloseBraceToken;

            if (closeBrace.IsMissing)
                return;

            if (typeDeclaration.SyntaxTree.GetLineCount(TextSpan.FromBounds(openBrace.SpanStart, closeBrace.Span.End)) != 1)
                return;

            if (!openBrace.TrailingTrivia.IsEmptyOrWhitespace())
                return;

            if (closeBrace.LeadingTrivia.Any())
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.AddNewLineToEmptyTypeDeclaration,
                Location.Create(typeDeclaration.SyntaxTree, closeBrace.Span.WithLength(0)));
        }
    }
}
