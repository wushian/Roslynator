// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.SyntaxRewriters
{
    internal sealed class LocalOrParameterSymbolRenamer : CSharpSyntaxRewriter
    {
        private readonly string _name;

        public LocalOrParameterSymbolRenamer(
            ILocalSymbol symbol,
            string newName,
            SemanticModel semanticModel,
            CancellationToken cancellationToken) : this((ISymbol)symbol, newName, semanticModel, cancellationToken)
        {
        }

        public LocalOrParameterSymbolRenamer(
            IParameterSymbol symbol,
            string newName,
            SemanticModel semanticModel,
            CancellationToken cancellationToken) : this((ISymbol)symbol, newName, semanticModel, cancellationToken)
        {
        }

        private LocalOrParameterSymbolRenamer(
            ISymbol symbol,
            string newName,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            Symbol = symbol;
            NewName = newName;
            SemanticModel = semanticModel;
            CancellationToken = cancellationToken;

            _name = symbol.Name;
        }

        public ISymbol Symbol { get; }

        public string NewName { get; }

        public SemanticModel SemanticModel { get; }

        public CancellationToken CancellationToken { get; }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (string.Equals(_name, node.Identifier.ValueText, StringComparison.Ordinal)
                && SemanticModel.GetSymbol(node, CancellationToken)?.OriginalDefinition.Equals(Symbol) == true)
            {
                return IdentifierName(Identifier(node.GetLeadingTrivia(), NewName, node.GetTrailingTrivia()));
            }

            return base.VisitIdentifierName(node);
        }
    }
}
