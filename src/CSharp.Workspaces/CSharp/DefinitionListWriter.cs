// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp
{
    internal class DefinitionListWriter
    {
        private static readonly SymbolDisplayFormat _namespaceFormat = SymbolDefinitionDisplayFormats.NamespaceDeclaration;

        private static readonly SymbolDisplayFormat _namespaceHierarchyFormat = SymbolDefinitionDisplayFormats.NamespaceDeclaration.Update(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly);

        private static readonly SymbolDisplayFormat _typeFormat = SymbolDefinitionDisplayFormats.FullDeclaration.Update(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly);

        private static readonly SymbolDisplayFormat _memberFormat = SymbolDefinitionDisplayFormats.FullDeclaration.Update(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

        private static readonly SymbolDisplayFormat _enumFieldFormat = SymbolDefinitionDisplayFormats.FullDeclaration;

        private bool _pendingIndentation;
        private int _indentationLevel;
        private INamespaceSymbol _currentNamespace;

        public DefinitionListWriter(
            TextWriter writer,
            DefinitionListOptions options = null,
            IComparer<ISymbol> comparer = null)
        {
            Writer = writer;
            Options = options ?? DefinitionListOptions.Default;
            Comparer = comparer ?? SymbolDefinitionComparer.Instance;
        }

        public TextWriter Writer { get; }

        public DefinitionListOptions Options { get; }

        public IComparer<ISymbol> Comparer { get; }

        public virtual bool IsVisibleType(ISymbol symbol)
        {
            return symbol.IsVisible(Options.Visibility);
        }

        public virtual bool IsVisibleMember(ISymbol symbol)
        {
            if (!symbol.IsVisible(Options.Visibility))
                return false;

            switch (symbol.Kind)
            {
                case SymbolKind.Event:
                case SymbolKind.Field:
                case SymbolKind.Property:
                    {
                        return true;
                    }
                case SymbolKind.Method:
                    {
                        var methodSymbol = (IMethodSymbol)symbol;

                        switch (methodSymbol.MethodKind)
                        {
                            case MethodKind.Constructor:
                                {
                                    return methodSymbol.ContainingType.TypeKind != TypeKind.Struct
                                        || methodSymbol.Parameters.Any();
                                }
                            case MethodKind.Conversion:
                            case MethodKind.UserDefinedOperator:
                            case MethodKind.Ordinary:
                                return true;
                            case MethodKind.ExplicitInterfaceImplementation:
                            case MethodKind.StaticConstructor:
                            case MethodKind.Destructor:
                            case MethodKind.EventAdd:
                            case MethodKind.EventRaise:
                            case MethodKind.EventRemove:
                            case MethodKind.PropertyGet:
                            case MethodKind.PropertySet:
                                return false;
                            default:
                                {
                                    Debug.Fail(methodSymbol.MethodKind.ToString());
                                    break;
                                }
                        }

                        return true;
                    }
                case SymbolKind.NamedType:
                    {
                        return false;
                    }
                default:
                    {
                        Debug.Fail(symbol.Kind.ToString());
                        return false;
                    }
            }
        }

        public virtual bool IsVisibleAttribute(INamedTypeSymbol attributeType)
        {
            return AttributeDisplay.ShouldBeDisplayed(attributeType);
        }

        public void Write(IEnumerable<IAssemblySymbol> assemblies)
        {
            foreach (IAssemblySymbol assembly in assemblies
                .OrderBy(f => f.Name)
                .ThenBy(f => f.Identity.Version))
            {
                AppendAssemblyAttributes(assembly);
            }

            IEnumerable<INamedTypeSymbol> types = assemblies.SelectMany(a => a.GetTypes(t => t.ContainingType == null
                && t.IsVisible(Options.Visibility)
                && !Options.ShouldBeIgnored(t)));

            if (Options.NestNamespaces)
            {
                WriteWithNamespaceHierarchy(types);
            }
            else
            {
                foreach (INamespaceSymbol namespaceSymbol in types
                    .Select(f => f.ContainingNamespace)
                    .Distinct(MetadataNameEqualityComparer<INamespaceSymbol>.Instance)
                    .OrderBy(f => f, Comparer))
                {
                    if (!namespaceSymbol.IsGlobalNamespace)
                    {
                        Write(namespaceSymbol, _namespaceFormat);
                        BeginTypeContent();
                    }

                    _currentNamespace = namespaceSymbol;

                    if (Options.Depth <= DefinitionListDepth.Type)
                        WriteTypes(types.Where(f => MetadataNameEqualityComparer<INamespaceSymbol>.Instance.Equals(f.ContainingNamespace, namespaceSymbol)));

                    _currentNamespace = null;

                    if (!namespaceSymbol.IsGlobalNamespace)
                    {
                        EndTypeContent();
                        WriteLine();
                    }
                }
            }
        }

        private void WriteWithNamespaceHierarchy(IEnumerable<INamedTypeSymbol> types)
        {
            var rootNamespaces = new HashSet<INamespaceSymbol>(MetadataNameEqualityComparer<INamespaceSymbol>.Instance);

            var nestedNamespaces = new HashSet<INamespaceSymbol>(MetadataNameEqualityComparer<INamespaceSymbol>.Instance);

            foreach (INamespaceSymbol namespaceSymbol in types.Select(f => f.ContainingNamespace))
            {
                if (namespaceSymbol.IsGlobalNamespace)
                {
                    rootNamespaces.Add(namespaceSymbol);
                }
                else
                {
                    INamespaceSymbol n = namespaceSymbol;

                    while (true)
                    {
                        INamespaceSymbol containingNamespace = n.ContainingNamespace;

                        if (containingNamespace.IsGlobalNamespace)
                        {
                            rootNamespaces.Add(n);
                            break;
                        }

                        nestedNamespaces.Add(n);

                        n = containingNamespace;
                    }
                }
            }

            foreach (INamespaceSymbol namespaceSymbol in rootNamespaces
                .OrderBy(f => f, Comparer))
            {
                WriteNamespace(namespaceSymbol);
                WriteLine();
            }

            void WriteNamespace(INamespaceSymbol namespaceSymbol, bool isNested = false, bool startsWithNewLine = false)
            {
                if (!namespaceSymbol.IsGlobalNamespace)
                {
                    if (isNested)
                    {
                        if (startsWithNewLine)
                            WriteLine();

                        Write("// ");
                        Write(namespaceSymbol, SymbolDefinitionDisplayFormats.TypeNameAndContainingTypesAndNamespaces);
                        WriteLine();
                    }

                    Write(namespaceSymbol, _namespaceHierarchyFormat);
                    BeginTypeContent();
                }

                _currentNamespace = namespaceSymbol;

                if (Options.Depth <= DefinitionListDepth.Type)
                    WriteTypes(types.Where(f => MetadataNameEqualityComparer<INamespaceSymbol>.Instance.Equals(f.ContainingNamespace, namespaceSymbol)));

                startsWithNewLine = false;

                foreach (INamespaceSymbol namespaceSymbol2 in nestedNamespaces
                    .Where(f => MetadataNameEqualityComparer<INamespaceSymbol>.Instance.Equals(f.ContainingNamespace, namespaceSymbol))
                    .Distinct(MetadataNameEqualityComparer<INamespaceSymbol>.Instance)
                    .OrderBy(f => f, Comparer)
                    .ToArray())
                {
                    nestedNamespaces.Remove(namespaceSymbol2);

                    WriteNamespace(namespaceSymbol2, isNested: true, startsWithNewLine: startsWithNewLine);

                    startsWithNewLine = true;
                }

                _currentNamespace = null;

                if (!namespaceSymbol.IsGlobalNamespace)
                    EndTypeContent();
            }
        }

        private void WriteTypes(IEnumerable<INamedTypeSymbol> types)
        {
            using (IEnumerator<INamedTypeSymbol> en = types.OrderBy(f => f, Comparer).GetEnumerator())
            {
                if (en.MoveNext())
                {
                    WriteLine();

                    while (true)
                    {
                        TypeKind typeKind = en.Current.TypeKind;

                        Write(SymbolDefinitionDisplay.GetDisplayParts(
                            en.Current,
                            _typeFormat,
                            SymbolDisplayTypeDeclarationOptions.IncludeAccessibility | SymbolDisplayTypeDeclarationOptions.IncludeModifiers,
                            containingNamespaceStyle: Options.ContainingNamespaceStyle,
                            isVisibleAttribute: IsVisibleAttribute,
                            formatBaseList: Options.FormatBaseList,
                            formatConstraints: Options.FormatConstraints,
                            formatParameters: Options.FormatParameters,
                            splitAttributes: Options.SplitAttributes,
                            includeAttributeArguments: Options.IncludeAttributeArguments,
                            omitIEnumerable: Options.OmitIEnumerable,
                            useDefaultLiteral: Options.UseDefaultLiteral));

                        switch (typeKind)
                        {
                            case TypeKind.Class:
                                {
                                    BeginTypeContent();

                                    WriteMembers(en.Current);

                                    EndTypeContent();
                                    break;
                                }
                            case TypeKind.Delegate:
                                {
                                    WriteLine(";");
                                    break;
                                }
                            case TypeKind.Enum:
                                {
                                    BeginTypeContent();

                                    if (Options.Depth == DefinitionListDepth.Member)
                                    {
                                        foreach (ISymbol member in en.Current.GetMembers())
                                        {
                                            if (member.Kind == SymbolKind.Field
                                                && member.DeclaredAccessibility == Accessibility.Public)
                                            {
                                                Write(member, _enumFieldFormat);
                                                Write(",");
                                                WriteLine();
                                            }
                                        }
                                    }

                                    EndTypeContent();
                                    break;
                                }
                            case TypeKind.Interface:
                                {
                                    BeginTypeContent();

                                    WriteMembers(en.Current);

                                    EndTypeContent();
                                    break;
                                }
                            case TypeKind.Struct:
                                {
                                    BeginTypeContent();

                                    WriteMembers(en.Current);

                                    EndTypeContent();
                                    break;
                                }
                        }

                        if (en.MoveNext())
                        {
                            WriteLine();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void BeginTypeContent()
        {
            WriteLine();

            _indentationLevel++;
        }

        private void EndTypeContent()
        {
            Debug.Assert(_indentationLevel > 0, "Cannot decrease indentation level.");

            _indentationLevel--;
        }

        private void WriteMembers(INamedTypeSymbol namedType)
        {
            if (Options.Depth == DefinitionListDepth.Member)
            {
                using (IEnumerator<ISymbol> en = namedType.GetMembers().Where(f => IsVisibleMember(f))
                    .OrderBy(f => f, Comparer)
                    .GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        WriteLine();

                        MemberDeclarationKind kind = en.Current.GetMemberDeclarationKind();

                        while (true)
                        {
                            ImmutableArray<SymbolDisplayPart> attributeParts = SymbolDefinitionDisplay.GetAttributesParts(
                                en.Current,
                                predicate: IsVisibleAttribute,
                                containingNamespaceStyle: Options.ContainingNamespaceStyle,
                                splitAttributes: Options.SplitAttributes,
                                includeAttributeArguments: Options.IncludeAttributeArguments);

                            Write(attributeParts);

                            ImmutableArray<SymbolDisplayPart> parts = en.Current.ToDisplayParts(_memberFormat);

                            if (Options.UseDefaultLiteral
                                && en.Current.GetParameters().Any(f => f.HasExplicitDefaultValue))
                            {
                                parts = SymbolDefinitionDisplay.ReplaceDefaultExpressionWithDefaultLiteral(en.Current, parts);
                            }

                            if (en.Current.Kind == SymbolKind.Property)
                            {
                                var propertySymbol = (IPropertySymbol)en.Current;

                                IMethodSymbol getMethod = propertySymbol.GetMethod;

                                if (getMethod != null)
                                    parts = SymbolDefinitionDisplay.AddAccessorAttributes(parts, getMethod);

                                IMethodSymbol setMethod = propertySymbol.SetMethod;

                                if (setMethod != null)
                                    parts = SymbolDefinitionDisplay.AddAccessorAttributes(parts, setMethod);
                            }
                            else if (en.Current.Kind == SymbolKind.Event)
                            {
                                var eventSymbol = (IEventSymbol)en.Current;

                                IMethodSymbol addMethod = eventSymbol.AddMethod;

                                if (addMethod != null)
                                    parts = SymbolDefinitionDisplay.AddAccessorAttributes(parts, addMethod);

                                IMethodSymbol removeMethod = eventSymbol.RemoveMethod;

                                if (removeMethod != null)
                                    parts = SymbolDefinitionDisplay.AddAccessorAttributes(parts, removeMethod);
                            }

                            ImmutableArray<IParameterSymbol> parameters = en.Current.GetParameters();

                            if (parameters.Any())
                            {
                                parts = SymbolDefinitionDisplay.AddParameterAttributes(
                                    parts,
                                    en.Current,
                                    parameters,
                                    containingNamespaceStyle: Options.ContainingNamespaceStyle);

                                if (Options.FormatParameters
                                    && parameters.Length > 1)
                                {
                                    ImmutableArray<SymbolDisplayPart>.Builder builder = parts.ToBuilder();
                                    SymbolDefinitionDisplay.FormatParameters(en.Current, builder, Options.IndentChars);

                                    parts = builder.ToImmutableArray();
                                }
                            }

                            Write(parts);

                            if (en.Current.Kind != SymbolKind.Property)
                                Write(";");

                            WriteLine();

                            if (en.MoveNext())
                            {
                                MemberDeclarationKind kind2 = en.Current.GetMemberDeclarationKind();

                                if (kind != kind2
                                    || Options.EmptyLineBetweenMembers)
                                {
                                    WriteLine();
                                }

                                kind = kind2;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            if (Options.Depth <= DefinitionListDepth.Type)
            {
                WriteTypes(namedType.GetTypeMembers().Where(f => IsVisibleType(f)));
            }
        }

        private void AppendAssemblyAttributes(IAssemblySymbol assemblySymbol)
        {
            ImmutableArray<SymbolDisplayPart> attributeParts = SymbolDefinitionDisplay.GetAttributesParts(
                assemblySymbol,
                IsVisibleAttribute,
                containingNamespaceStyle: Options.ContainingNamespaceStyle,
                splitAttributes: Options.SplitAttributes,
                includeAttributeArguments: Options.IncludeAttributeArguments);

            if (attributeParts.Any())
            {
                WriteLine(assemblySymbol.Identity.ToString());
                Write(attributeParts);
                WriteLine();
            }
        }

        private void Write(ISymbol symbol, SymbolDisplayFormat format)
        {
            Write(symbol.ToDisplayParts(format));
        }

        private void Write(ImmutableArray<SymbolDisplayPart> parts)
        {
            foreach (SymbolDisplayPart part in parts)
            {
                CheckPendingIndentation();

                Writer.Write(part);

                if (part.Kind == SymbolDisplayPartKind.LineBreak
                    && Options.Indent)
                {
                    _pendingIndentation = true;
                }
            }
        }

        public void Write(string value)
        {
            CheckPendingIndentation();
            Writer.Write(value);
        }

        public void WriteLine()
        {
            Writer.WriteLine();

            if (Options.Indent)
                _pendingIndentation = true;
        }

        public void WriteLine(string value)
        {
            Write(value);
            WriteLine();
        }

        public void WriteIndentation()
        {
            for (int i = 0; i < _indentationLevel; i++)
            {
                Write(Options.IndentChars);
            }
        }

        private void CheckPendingIndentation()
        {
            if (_pendingIndentation)
            {
                _pendingIndentation = false;
                WriteIndentation();
            }
        }

        public override string ToString()
        {
            return Writer.ToString();
        }
    }
}
