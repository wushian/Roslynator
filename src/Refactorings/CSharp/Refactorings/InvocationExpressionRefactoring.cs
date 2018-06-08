// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.Refactorings.InlineDefinition;
using Roslynator.CSharp.Refactorings.ExtractLinqToLocalFunction;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InvocationExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InvocationExpressionSyntax invocationExpression)
        {
            if (context.IsAnyRefactoringEnabled(
                RefactoringIdentifiers.UseElementAccessInsteadOfEnumerableMethod,
                RefactoringIdentifiers.ExtractLinqToLocalFunction,
                RefactoringIdentifiers.ReplaceAnyWithAllOrAllWithAny,
                RefactoringIdentifiers.CallExtensionMethodAsInstanceMethod,
                RefactoringIdentifiers.CallIndexOfInsteadOfContains))
            {
                SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationExpression);

                if (invocationInfo.Success)
                {
                    if (context.Span.IsEmptyAndContainedInSpan(invocationInfo.Name))
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseElementAccessInsteadOfEnumerableMethod))
                            UseElementAccessRefactoring.ComputeRefactorings(context, invocationInfo, semanticModel);

                        if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExtractLinqToLocalFunction))
                            ExtractLinqToLocalFunctionRefactoring.ComputeRefactorings(context, invocationInfo, semanticModel);

                        if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceAnyWithAllOrAllWithAny))
                            ReplaceAnyWithAllOrAllWithAnyRefactoring.ComputeRefactoring(context, invocationExpression, semanticModel);

                        if (context.IsRefactoringEnabled(RefactoringIdentifiers.CallIndexOfInsteadOfContains))
                            CallIndexOfInsteadOfContainsRefactoring.ComputeRefactoring(context, invocationExpression, semanticModel);
                    }

                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.CallExtensionMethodAsInstanceMethod))
                    {
                        SyntaxNodeOrToken nodeOrToken = CallExtensionMethodAsInstanceMethodAnalysis.GetNodeOrToken(invocationExpression.Expression);

                        if (nodeOrToken.Span.Contains(context.Span))
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            CallExtensionMethodAsInstanceMethodAnalysisResult analysis = CallExtensionMethodAsInstanceMethodAnalysis.Analyze(invocationExpression, semanticModel, allowAnyExpression: true, cancellationToken: context.CancellationToken);

                            if (analysis.Success)
                            {
                                context.RegisterRefactoring(
                                    CallExtensionMethodAsInstanceMethodRefactoring.Title,
                                    cancellationToken =>
                                    {
                                        return context.Document.ReplaceNodeAsync(
                                            analysis.InvocationExpression,
                                            analysis.NewInvocationExpression,
                                            cancellationToken);
                                    },
                                    RefactoringIdentifiers.CallExtensionMethodAsInstanceMethod);
                            }
                        }
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceStringFormatWithInterpolatedString)
                && context.SupportsCSharp6)
            {
                await ReplaceStringFormatWithInterpolatedStringRefactoring.ComputeRefactoringsAsync(context, invocationExpression).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseBitwiseOperationInsteadOfCallingHasFlag))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (UseBitwiseOperationInsteadOfCallingHasFlagAnalysis.IsFixable(invocationExpression, semanticModel, context.CancellationToken))
                {
                    context.RegisterRefactoring(
                        UseBitwiseOperationInsteadOfCallingHasFlagRefactoring.Title,
                        cancellationToken =>
                        {
                            return UseBitwiseOperationInsteadOfCallingHasFlagRefactoring.RefactorAsync(
                                context.Document,
                                invocationExpression,
                                cancellationToken);
                        },
                        RefactoringIdentifiers.UseBitwiseOperationInsteadOfCallingHasFlag);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.InlineMethod))
                await InlineMethodRefactoring.ComputeRefactoringsAsync(context, invocationExpression).ConfigureAwait(false);
        }
    }
}
