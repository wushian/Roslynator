// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.CodeAnalysis;
using Roslynator.FindSymbols;

namespace Roslynator.Documentation.Html
{
    internal class SymbolDefinitionHtmlWriter : SymbolDefinitionWriter
    {
        private XmlWriter _writer;
        private bool _pendingIndentation;

        private readonly SymbolDisplayFormat _namespaceFormat;
        private readonly SymbolDisplayFormat _typeFormat;

        public SymbolDefinitionHtmlWriter(
            XmlWriter writer,
            SymbolFilterOptions filter = null,
            DefinitionListFormat format = null,
            SymbolDocumentationProvider documentationProvider = null) : base(filter, format, documentationProvider)
        {
            _writer = writer;

            _namespaceFormat = new SymbolDisplayFormat(
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

            _typeFormat = new SymbolDisplayFormat(
                typeQualificationStyle: (Format.Includes(SymbolDefinitionPartFilter.ContainingNamespace))
                    ? SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces
                    : SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
                miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);
        }

        public override bool SupportsMultilineDefinitions => true;

        public override bool SupportsDocumentationComments => true;

        protected override SymbolDisplayFormat CreateNamespaceFormat(SymbolDisplayFormat format)
        {
            return format.Update(
                globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
        }

        protected override SymbolDisplayFormat CreateTypeFormat(SymbolDisplayFormat format)
        {
            return format.Update(
                globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                miscellaneousOptions: format.MiscellaneousOptions & ~ SymbolDisplayMiscellaneousOptions.UseSpecialTypes);
        }

        protected override SymbolDisplayFormat CreateMemberFormat(SymbolDisplayFormat format)
        {
            return format.Update(
                globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                miscellaneousOptions: format.MiscellaneousOptions & ~SymbolDisplayMiscellaneousOptions.UseSpecialTypes);
        }

        public override void WriteStartDocument()
        {
            _writer.WriteRaw("<!DOCTYPE html>");
            WriteLine();

            WriteStartElement("html");
            WriteStartElement("body");
            WriteStartElement("pre");
        }

        public override void WriteEndDocument()
        {
            WriteEndElement();
            WriteLine();
            WriteEndElement();
            WriteLine();
            WriteEndElement();
            WriteLine();

            Debug.Assert(Depth == 0, "Depth should be equal to 0.");

            _writer.WriteEndDocument();
        }

        public override void WriteStartAssemblies()
        {
        }

        public override void WriteEndAssemblies()
        {
        }

        public override void WriteStartAssembly(IAssemblySymbol assemblySymbol)
        {
        }

        public override void WriteAssemblyDefinition(IAssemblySymbol assemblySymbol)
        {
            Write("assembly ");
            WriteLine(assemblySymbol.Identity.ToString());
            IncreaseDepth();

            if (Format.Includes(SymbolDefinitionPartFilter.AssemblyAttributes))
                WriteAttributes(assemblySymbol);
        }

        public override void WriteEndAssembly(IAssemblySymbol assemblySymbol)
        {
            DecreaseDepth();
        }

        public override void WriteAssemblySeparator()
        {
            if (Format.Includes(SymbolDefinitionPartFilter.AssemblyAttributes))
                WriteLine();
        }

        public override void WriteStartNamespaces()
        {
            WriteLine();
        }

        public override void WriteEndNamespaces()
        {
        }

        public override void WriteStartNamespace(INamespaceSymbol namespaceSymbol)
        {
            WriteStartElement("a");
            WriteStartAttribute("name");
            WriteLocalLink(namespaceSymbol);
            WriteEndAttribute();
            WriteEndElement();
            WriteStartCodeElement();
        }

        public override void WriteNamespaceDefinition(INamespaceSymbol namespaceSymbol, SymbolDisplayFormat format = null)
        {
            if (namespaceSymbol.IsGlobalNamespace)
                return;

            WriteDocumentationComment(namespaceSymbol);
            Write(namespaceSymbol, format ?? NamespaceFormat);
            WriteEndElement();
            WriteLine();
            IncreaseDepth();
        }

        public override void WriteEndNamespace(INamespaceSymbol namespaceSymbol)
        {
            if (namespaceSymbol.IsGlobalNamespace)
                return;

            DecreaseDepth();
        }

        public override void WriteNamespaceSeparator()
        {
            WriteLine();
        }

        public override void WriteStartTypes()
        {
            WriteLine();
        }

        public override void WriteEndTypes()
        {
        }

        public override void WriteStartType(INamedTypeSymbol typeSymbol)
        {
            if (typeSymbol != null)
            {
                WriteStartElement("a");
                WriteStartAttribute("name");
                WriteLocalLink(typeSymbol);
                WriteEndAttribute();
                WriteEndElement();
                WriteStartCodeElement();
            }
        }

        public override void WriteTypeDefinition(INamedTypeSymbol typeSymbol, SymbolDisplayFormat format = null, SymbolDisplayTypeDeclarationOptions? typeDeclarationOptions = null)
        {
            if (typeSymbol != null)
            {
                WriteDocumentationComment(typeSymbol);
                Write(typeSymbol, format ?? TypeFormat, typeDeclarationOptions);
            }

            WriteEndElement();
            WriteLine();
            IncreaseDepth();
        }

        public override void WriteEndType(INamedTypeSymbol typeSymbol)
        {
            DecreaseDepth();
        }

        public override void WriteTypeSeparator()
        {
            WriteLine();
        }

        public override void WriteStartMembers()
        {
            WriteLine();
        }

        public override void WriteEndMembers()
        {
        }

        public override void WriteStartMember(ISymbol symbol)
        {
            WriteStartCodeElement();
        }

        public override void WriteMemberDefinition(ISymbol symbol, SymbolDisplayFormat format = null)
        {
            if (format == null)
            {
                format = (symbol.GetFirstExplicitInterfaceImplementation() != null)
                    ? ExplicitInterfaceImplementationFormat
                    : MemberFormat;
            }

            WriteDocumentationComment(symbol);
            Write(symbol, format);
            WriteEndElement();
            WriteLine();
            IncreaseDepth();
        }

        public override void WriteEndMember(ISymbol symbol)
        {
            DecreaseDepth();
        }

        public override void WriteMemberSeparator()
        {
            WriteLine();
        }

        public override void WriteStartEnumMembers()
        {
            WriteLine();
        }

        public override void WriteEndEnumMembers()
        {
        }

        public override void WriteStartEnumMember(ISymbol symbol)
        {
            WriteStartCodeElement();
        }

        public override void WriteEnumMemberDefinition(ISymbol symbol, SymbolDisplayFormat format = null)
        {
            WriteDocumentationComment(symbol);

            Write(symbol, format ?? EnumMemberFormat);

            if (Format.Includes(SymbolDefinitionPartFilter.TrailingComma))
                Write(",");

            WriteEndElement();
            WriteLine();
            IncreaseDepth();
        }

        public override void WriteEndEnumMember(ISymbol symbol)
        {
            DecreaseDepth();
        }

        public override void WriteEnumMemberSeparator()
        {
        }

        public override void WriteStartAttributes(ISymbol symbol)
        {
            Write("[");
        }

        public override void WriteEndAttributes(ISymbol symbol)
        {
            Write("]");
            if (symbol.Kind == SymbolKind.Assembly || SupportsMultilineDefinitions)
            {
                WriteLine();
            }
            else
            {
                Write(" ");
            }
        }

        public override void WriteStartAttribute(AttributeData attribute, ISymbol symbol)
        {
        }

        public override void WriteEndAttribute(AttributeData attribute, ISymbol symbol)
        {
        }

        public override void WriteAttributeSeparator(ISymbol symbol)
        {
            if (symbol.Kind == SymbolKind.Assembly
                || (Format.Includes(SymbolDefinitionFormatOptions.Attributes) && SupportsMultilineDefinitions))
            {
                Write("]");
                WriteLine();
                Write("[");
            }
            else
            {
                Write(", ");
            }
        }

        public override void Write(SymbolDisplayPart part)
        {
            base.Write(part);

            if (part.Kind == SymbolDisplayPartKind.LineBreak)
                _pendingIndentation = true;
        }

        public override void Write(ISymbol symbol, SymbolDisplayFormat format, SymbolDisplayTypeDeclarationOptions? typeDeclarationOptions = null, SymbolDisplayAdditionalOptions? additionalOptions = null)
        {
            if (Format.Includes(SymbolDefinitionPartFilter.Attributes))
                WriteAttributes(symbol);

            ImmutableArray<SymbolDisplayPart> parts = GetDisplayParts(
                symbol,
                format,
                typeDeclarationOptions,
                additionalOptions);

            int i = 0;
            int j = 0;

            while (i < parts.Length)
            {
                if (parts[i].IsKeyword("global")
                    && parts[i].Symbol.IsKind(SymbolKind.Namespace)
                    && ((INamespaceSymbol)parts[i].Symbol).IsGlobalNamespace)
                {
                    j = i;

                    if (Peek().IsPunctuation("::")
                        && Peek(2).IsTypeOrNamespaceName())
                    {
                        j += 2;

                        while (Peek().IsPunctuation(".")
                            && Peek(2).IsTypeOrNamespaceName())
                        {
                            j += 2;
                        }

                        ISymbol symbol2 = parts[j].Symbol.OriginalDefinition;

                        SymbolDisplayFormat format2 = (symbol2.IsKind(SymbolKind.Namespace))
                            ? _namespaceFormat
                            : _typeFormat;

                        if (symbol == symbol2)
                        {
                            Write(symbol2.ToDisplayParts(format2));
                        }
                        else
                        {
                            WriteStartElement("a");
                            WriteStartAttribute("href");
                            Write("#");
                            WriteLocalLink(symbol2);
                            WriteEndAttribute();

                            Write(symbol2.ToDisplayParts(format2));
                            WriteEndElement();
                        }

                        i = j + 1;
                        continue;
                    }
                }

                Write(parts[i]);

                i++;
            }

            SymbolDisplayPart Peek(int offset = 1)
            {
                if (j < parts.Length - offset)
                {
                    return parts[j + offset];
                }

                return default;
            }
        }

        private void WriteLocalLink(ISymbol symbol)
        {
            int cnc = 0;

            INamespaceSymbol cn = symbol.ContainingNamespace;

            while (cn?.IsGlobalNamespace == false)
            {
                cn = cn.ContainingNamespace;
                cnc++;
            }

            while (cnc > 0)
            {
                WriteString(GetContainingNamespace(cnc).Name);
                WriteString("_");
                cnc--;
            }

            INamedTypeSymbol ct = symbol.ContainingType;

            int ctc = 0;

            while (ct != null)
            {
                ct = ct.ContainingType;
                ctc++;
            }

            while (ctc > 0)
            {
                WriteType(GetContainingType(ctc));
                WriteString("_");
                ctc--;
            }

            if (symbol.IsKind(SymbolKind.NamedType))
            {
                WriteType((INamedTypeSymbol)symbol);
            }
            else
            {
                WriteString(symbol.Name);
            }

            INamespaceSymbol GetContainingNamespace(int count)
            {
                INamespaceSymbol n = symbol.ContainingNamespace;

                while (count > 1)
                {
                    n = n.ContainingNamespace;
                    count--;
                }

                return n;
            }

            INamedTypeSymbol GetContainingType(int count)
            {
                INamedTypeSymbol t = symbol.ContainingType;

                while (count > 1)
                {
                    t = t.ContainingType;
                    count--;
                }

                return t;
            }

            void WriteType(INamedTypeSymbol typeSymbol)
            {
                WriteString(typeSymbol.Name);

                int arity = typeSymbol.Arity;

                if (arity > 0)
                {
                    WriteString("_");
                    WriteString(arity.ToString());
                }
            }
        }

        private void WriteStartCodeElement()
        {
            WriteStartElement("code");
            WriteAttributeString("class", "csharp");
            WriteIndentation();
        }

        private void WriteStartElement(string name)
        {
            _writer.WriteStartElement(name);
        }

        private void WriteEndElement()
        {
            _writer.WriteEndElement();
        }

        private void WriteStartAttribute(string name)
        {
            _writer.WriteStartAttribute(name);
        }

        private void WriteEndAttribute()
        {
            _writer.WriteEndAttribute();
        }

        private void WriteAttributeString(string name, string value)
        {
            _writer.WriteAttributeString(name, value);
        }

        public override void Write(string value)
        {
            if (_pendingIndentation)
                WriteIndentation();

            WriteString(value);
        }

        private void WriteString(string text)
        {
            _writer.WriteString(text);
        }

        public override void WriteLine()
        {
            _writer.WriteWhitespace(_writer.Settings.NewLineChars);

            _pendingIndentation = true;
        }

        private void WriteIndentation()
        {
            _pendingIndentation = false;

            for (int i = 0; i < Depth; i++)
            {
                Write(Format.IndentChars);
            }
        }

        public override void WriteDocumentationComment(ISymbol symbol)
        {
            IEnumerable<string> elements = DocumentationProvider?.GetXmlDocumentation(symbol)?.GetElementsAsText(skipEmptyElement: true, makeSingleLine: true);

            if (elements == null)
                return;

            foreach (string element in elements)
                WriteDocumentation(element);

            void WriteDocumentation(string element)
            {
                using (var sr = new StringReader(element))
                {
                    string line = null;

                    while ((line = sr.ReadLine()) != null)
                    {
                        WriteLine(line);
                        WriteIndentation();
                    }
                }
            }
        }

        public override void Close()
        {
            if (_writer != null)
            {
                try
                {
                    _writer.Flush();
                }
                finally
                {
                    try
                    {
                        _writer.Dispose();
                    }
                    finally
                    {
                        _writer = null;
                    }
                }
            }
        }
    }
}
