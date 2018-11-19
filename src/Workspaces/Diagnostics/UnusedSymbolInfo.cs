// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator.Diagnostics
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal readonly struct UnusedSymbolInfo
    {
        public UnusedSymbolInfo(ISymbol symbol, string id, ProjectId projectId)
        {
            Symbol = symbol;
            Id = id;
            ProjectId = projectId;
        }

        public ISymbol Symbol { get; }

        public string Id { get; }

        public ProjectId ProjectId { get; }

        public UnusedSymbolKinds Kind
        {
            get { return (Symbol != null) ? UnusedSymbolFinder.GetUnusedSymbolKind(Symbol) : UnusedSymbolKinds.None; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{Kind} {Symbol?.ToDisplayString(SymbolDisplayFormats.Test)}"; }
        }
    }
}
