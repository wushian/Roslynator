// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator.FindSymbols
{
    //TODO: SymbolNamePattern, IgnoredModifiers, IgnoredSymbolNames
    internal class SymbolFinderOptions
    {
        private static readonly ImmutableArray<Visibility> _allVisibilities = ImmutableArray.Create(Visibility.Public, Visibility.Internal, Visibility.Private);

        private readonly VisibilityFlags _visibilityFlags;

        public SymbolFinderOptions(
            SymbolSpecialKinds symbolKinds = SymbolSpecialKinds.TypeOrMember,
            ImmutableArray<Visibility> visibilities = default,
            ImmutableArray<MetadataName> ignoredAttributes = default,
            bool includeGeneratedCode = false,
            bool unusedOnly = false)
        {
            SymbolKinds = symbolKinds;
            Visibilities = (!visibilities.IsDefault) ? visibilities : _allVisibilities;
            IgnoredAttributes = (!ignoredAttributes.IsDefault) ? ignoredAttributes : ImmutableArray<MetadataName>.Empty;
            IncludeGeneratedCode = includeGeneratedCode;
            UnusedOnly = unusedOnly;

            foreach (Visibility visibility in Visibilities)
            {
                switch (visibility)
                {
                    case Visibility.Private:
                        {
                            _visibilityFlags |= VisibilityFlags.Private;
                            break;
                        }
                    case Visibility.Internal:
                        {
                            _visibilityFlags |= VisibilityFlags.Internal;
                            break;
                        }
                    case Visibility.Public:
                        {
                            _visibilityFlags |= VisibilityFlags.Public;
                            break;
                        }
                    default:
                        {
                            throw new ArgumentException("", nameof(visibilities));
                        }
                }
            }
        }

        public static SymbolFinderOptions Default { get; } = new SymbolFinderOptions();

        public SymbolSpecialKinds SymbolKinds { get; }

        public ImmutableArray<Visibility> Visibilities { get; }

        public bool IncludeGeneratedCode { get; }

        public bool UnusedOnly { get; }

        public ImmutableArray<MetadataName> IgnoredAttributes { get; }

        public bool IsVisible(ISymbol symbol)
        {
            switch (symbol.GetVisibility())
            {
                case Visibility.NotApplicable:
                    break;
                case Visibility.Private:
                    return (_visibilityFlags & VisibilityFlags.Private) != 0;
                case Visibility.Internal:
                    return (_visibilityFlags & VisibilityFlags.Internal) != 0;
                case Visibility.Public:
                    return (_visibilityFlags & VisibilityFlags.Public) != 0;
            }

            Debug.Fail(symbol.ToDisplayString(SymbolDisplayFormats.Test));

            return false;
        }

        public bool HasIgnoredAttribute(ISymbol symbol)
        {
            if (IgnoredAttributes.Any())
            {
                foreach (AttributeData attribute in symbol.GetAttributes())
                {
                    foreach (MetadataName attributeName in IgnoredAttributes)
                    {
                        if (attribute.AttributeClass.HasMetadataName(attributeName))
                            return true;
                    }
                }
            }

            return false;
        }

        [Flags]
        internal enum VisibilityFlags
        {
            None = 0,
            Public = 1,
            Internal = 2,
            Private = 4
        }
    }
}
