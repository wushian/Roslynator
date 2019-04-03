// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddMissingPropertiesToObjectInitializerRefactoring
    {
        private const string Title = "Add missing properties";

        public static void ComputeRefactorings(RefactoringContext context, InitializerExpressionSyntax initializer, SemanticModel semanticModel)
        {
            Debug.Assert(initializer.IsKind(SyntaxKind.ObjectInitializerExpression), initializer.Kind().ToString());

            if (!(initializer.Parent is ObjectCreationExpressionSyntax objectCreationExpression))
                return;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(objectCreationExpression, context.CancellationToken);

            if (typeSymbol?.IsErrorType() != false)
                return;

            if (typeSymbol.IsAnonymousType)
                return;

            int position = initializer.OpenBraceToken.Span.End;

            ImmutableArray<ISymbol> symbols = semanticModel.LookupSymbols(position, typeSymbol);

            if (!symbols.Any())
                return;

            SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;

            if (expressions.Any())
            {
                Dictionary<string, IPropertySymbol> namesToProperties = null;

                foreach (ISymbol symbol in symbols)
                {
                    IPropertySymbol propertySymbol = GetInitializableProperty(symbol, position, semanticModel);

                    if (propertySymbol == null)
                        continue;

                    if (namesToProperties != null)
                    {
                        if (namesToProperties.ContainsKey(propertySymbol.Name))
                            continue;
                    }
                    else
                    {
                        namesToProperties = new Dictionary<string, IPropertySymbol>();
                    }

                    namesToProperties.Add(propertySymbol.Name, propertySymbol);
                }

                if (namesToProperties == null)
                    return;

                foreach (ExpressionSyntax expression in expressions)
                {
                    Debug.Assert(expression.IsKind(SyntaxKind.SimpleAssignmentExpression), expression.Kind().ToString());

                    if (expression.IsKind(SyntaxKind.SimpleAssignmentExpression))
                    {
                        SimpleAssignmentExpressionInfo assignmentInfo = SyntaxInfo.SimpleAssignmentExpressionInfo((AssignmentExpressionSyntax)expression);

                        if (assignmentInfo.Success)
                        {
                            ExpressionSyntax left = assignmentInfo.Left;

                            Debug.Assert(left.IsKind(SyntaxKind.IdentifierName), left.Kind().ToString());

                            if (left is IdentifierNameSyntax identifierName
                                && namesToProperties.Remove(identifierName.Identifier.ValueText)
                                && namesToProperties.Count == 0)
                            {
                                return;
                            }
                        }
                    }
                }

                Document document = context.Document;

                context.RegisterRefactoring(
                    Title,
                    ct => RefactorAsync(document, initializer, namesToProperties, ct),
                    RefactoringIdentifiers.AddMissingPropertiesToObjectInitializer);
            }
            else if (HasAccessiblePropertySetter())
            {
                Document document = context.Document;

                context.RegisterRefactoring(
                    Title,
                    ct => RefactorAsync(document, initializer, symbols, semanticModel, ct),
                    RefactoringIdentifiers.AddMissingPropertiesToObjectInitializer);
            }

            bool HasAccessiblePropertySetter()
            {
                foreach (ISymbol symbol in symbols)
                {
                    if (GetInitializableProperty(symbol, position, semanticModel) != null)
                        return true;
                }

                return false;
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            InitializerExpressionSyntax initializer,
            ImmutableArray<ISymbol> symbols,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            IOrderedEnumerable<IPropertySymbol> propertySymbols = GetInitializableProperties().OrderBy(f => f.Name);

            return RefactorAsync(document, initializer, propertySymbols, cancellationToken);

            IEnumerable<IPropertySymbol> GetInitializableProperties()
            {
                int position = initializer.OpenBraceToken.Span.End;

                foreach (ISymbol symbol in symbols)
                {
                    IPropertySymbol propertySymbol = GetInitializableProperty(symbol, position, semanticModel);
                    if (propertySymbol != null)
                    {
                        yield return propertySymbol;
                    }
                }
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            InitializerExpressionSyntax initializer,
            Dictionary<string, IPropertySymbol> namesToProperties,
            CancellationToken cancellationToken)
        {
            IOrderedEnumerable<IPropertySymbol> propertySymbols = namesToProperties
                .Select(f => f.Value)
                .OrderBy(f => f.Name);

            return RefactorAsync(document, initializer, propertySymbols, cancellationToken);
        }

        private static Task<Document> RefactorAsync(Document document, InitializerExpressionSyntax initializer, IEnumerable<IPropertySymbol> propertySymbols, CancellationToken cancellationToken)
        {
            IEnumerable<AssignmentExpressionSyntax> newExpressions = propertySymbols.Select(propertySymbol =>
            {
                return SimpleAssignmentExpression(
                    IdentifierName(propertySymbol.Name),
                    propertySymbol.Type.GetDefaultValueSyntax(document.GetDefaultSyntaxOptions()));
            });

            InitializerExpressionSyntax newInitializer = initializer
                .WithExpressions(initializer.Expressions.AddRange(newExpressions))
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(initializer, newInitializer, cancellationToken);
        }

        private static IPropertySymbol GetInitializableProperty(ISymbol symbol, int position, SemanticModel semanticModel)
        {
            if (!symbol.IsStatic
                && symbol.IsKind(SymbolKind.Property))
            {
                var propertySymbol = (IPropertySymbol)symbol;

                if (!propertySymbol.IsIndexer)
                {
                    IMethodSymbol setter = propertySymbol.SetMethod;

                    if (setter != null
                        && semanticModel.IsAccessible(position, setter))
                    {
                        return propertySymbol;
                    }
                }
            }

            return null;
        }
    }
}
