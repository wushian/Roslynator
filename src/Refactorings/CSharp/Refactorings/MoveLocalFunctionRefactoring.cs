// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MoveLocalFunctionRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            LocalFunctionStatementSyntax localFunction,
            CancellationToken cancellationToken)
        {
            var block = (BlockSyntax)localFunction.Parent;

            SyntaxList<StatementSyntax> statements = block.Statements;

            int index = statements.IndexOf(localFunction);

            int newIndex = index + 1;

            while (newIndex < statements.Count
                && !statements[newIndex].IsKind(SyntaxKind.LocalFunctionStatement))
            {
                newIndex++;
            }

            SyntaxList<StatementSyntax> newStatements = statements
                .Insert(newIndex, localFunction.WithNavigationAnnotation())
                .RemoveAt(index);

            BlockSyntax newBlock = block.WithStatements(newStatements).WithFormatterAnnotation();

            return document.ReplaceNodeAsync(block, newBlock, cancellationToken);
        }
    }
}
