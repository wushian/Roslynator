// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class PlaceConditionalOperatorBeforeExpressionAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.PlaceConditionalOperatorBeforeExpression); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeConditionalExpression, SyntaxKind.ConditionalExpression);
        }

        private static void AnalyzeConditionalExpression(SyntaxNodeAnalysisContext context)
        {
            var conditionalExpression = (ConditionalExpressionSyntax)context.Node;

            if (conditionalExpression.ContainsDiagnostics)
                return;

            if (conditionalExpression.ContainsDirectives)
                return;

            if (!IsFixable(conditionalExpression.Condition, conditionalExpression.QuestionToken)
                && !IsFixable(conditionalExpression.WhenTrue, conditionalExpression.ColonToken))
            {
                return;
            }

            context.ReportDiagnostic(DiagnosticDescriptors.PlaceConditionalOperatorBeforeExpression, conditionalExpression);
        }

        internal static bool IsFixable(ExpressionSyntax expression, SyntaxToken token)
        {
            SyntaxTriviaList expressionTrailing = expression.GetTrailingTrivia();

            if (expressionTrailing.IsEmptyOrWhitespace())
            {
                SyntaxTriviaList tokenLeading = token.LeadingTrivia;

                if (tokenLeading.IsEmptyOrWhitespace())
                {
                    SyntaxTriviaList tokenTrailing = token.TrailingTrivia;

                    int count = tokenTrailing.Count;

                    if (count == 1)
                    {
                        if (tokenTrailing[0].IsEndOfLineTrivia())
                            return true;
                    }
                    else if (count > 1)
                    {
                        for (int i = 0; i < count - 1; i++)
                        {
                            if (!tokenTrailing[i].IsWhitespaceTrivia())
                                return false;
                        }

                        if (tokenTrailing.Last().IsEndOfLineTrivia())
                            return true;
                    }
                }
            }

            return false;
        }
    }
}
