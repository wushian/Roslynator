// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class SimplifyNullCheckWithFirstOrDefaultAnalysis
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, SimpleMemberInvocationExpressionInfo invocationInfo)
        {
            InvocationExpressionSyntax invocation = invocationInfo.InvocationExpression;

            SyntaxNode parent = invocation.WalkUpParentheses().Parent;

            NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(parent, NullCheckStyles.ComparisonToNull | NullCheckStyles.IsNull);

            if (!nullCheck.Success)
                return;

            SyntaxNode node = nullCheck.NullCheckExpression;

            if (node.ContainsDirectives)
                return;

            IMethodSymbol methodSymbol = context.SemanticModel
                .GetReducedExtensionMethodInfo(invocation, context.CancellationToken)
                .Symbol;

            if (methodSymbol == null)
                return;

            if (!SymbolUtility.IsLinqExtensionOfIEnumerableOfTWithPredicate(methodSymbol, context.SemanticModel, name : invocationInfo.NameText, allowImmutableArrayExtension: true))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.SimplifyLinqMethodChain, node);
        }
    }
}
