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
    internal class AddNewLineBeforeClosingBraceOfBlockAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddNewLineBeforeClosingBraceOfBlock); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeBlock, SyntaxKind.Block);
        }

        private static void AnalyzeBlock(SyntaxNodeAnalysisContext context)
        {
            var block = (BlockSyntax)context.Node;

            SyntaxList<StatementSyntax> statements = block.Statements;

            if (statements.Any())
                return;

            //TODO: ?
            if (block.Parent is AccessorDeclarationSyntax)
                return;

            if (block.Parent is AnonymousFunctionExpressionSyntax)
                return;

            SyntaxToken openBrace = block.OpenBraceToken;
            SyntaxToken closeBrace = block.CloseBraceToken;

            if (block.SyntaxTree.GetLineCount(TextSpan.FromBounds(openBrace.SpanStart, closeBrace.Span.End)) != 1)
                return;

            if (!openBrace.TrailingTrivia.IsEmptyOrWhitespace())
                return;

            if (closeBrace.LeadingTrivia.Any())
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.AddNewLineBeforeClosingBraceOfBlock,
                Location.Create(block.SyntaxTree, closeBrace.Span.WithLength(0)));
        }
    }
}
