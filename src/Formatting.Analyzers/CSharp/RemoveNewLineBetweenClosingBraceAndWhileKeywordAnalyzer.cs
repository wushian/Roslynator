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
    internal class RemoveNewLineBetweenClosingBraceAndWhileKeywordAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveNewLineBetweenClosingBraceAndWhileKeyword); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeDoStatement, SyntaxKind.DoStatement);
        }

        private static void AnalyzeDoStatement(SyntaxNodeAnalysisContext context)
        {
            var doStatement = (DoStatementSyntax)context.Node;

            StatementSyntax statement = doStatement.Statement;

            if (!statement.IsKind(SyntaxKind.Block))
                return;

            if (!SyntaxTriviaAnalysis.IsOptionalWhitespaceThenEndOfLineTrivia(statement.GetTrailingTrivia()))
                return;

            if (!doStatement.WhileKeyword.LeadingTrivia.IsEmptyOrWhitespace())
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.RemoveNewLineBetweenClosingBraceAndWhileKeyword,
                Location.Create(doStatement.SyntaxTree, new TextSpan(statement.Span.End, 0)));
        }
    }
}
