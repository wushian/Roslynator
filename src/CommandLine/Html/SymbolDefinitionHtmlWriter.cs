// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Roslynator.FindSymbols;

namespace Roslynator.Documentation.Html
{
    internal class SymbolDefinitionHtmlWriter : SymbolDefinitionWriter
    {
        private XmlWriter _writer;
        private bool _pendingIndentation;
        private ImmutableHashSet<IAssemblySymbol> _assemblies = ImmutableHashSet<IAssemblySymbol>.Empty;

        private static readonly SymbolDisplayFormat _typeFormat = new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters);

        private static readonly SymbolDisplayFormat _memberFormat = new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        public SymbolDefinitionHtmlWriter(
            XmlWriter writer,
            SymbolFilterOptions filter = null,
            DefinitionListFormat format = null,
            SymbolDocumentationProvider documentationProvider = null) : base(filter, format, documentationProvider)
        {
            _writer = writer;
        }

        public override bool SupportsMultilineDefinitions => true;

        public override bool SupportsDocumentationComments => true;

        protected override SymbolDisplayFormat CreateNamespaceFormat(SymbolDisplayFormat format)
        {
            return UpdateFormat(format);
        }

        protected override SymbolDisplayFormat CreateTypeFormat(SymbolDisplayFormat format)
        {
            return UpdateFormat(format);
        }

        protected override SymbolDisplayFormat CreateMemberFormat(SymbolDisplayFormat format)
        {
            return UpdateFormat(format);
        }

        protected override SymbolDisplayFormat CreateEnumMemberFormat(SymbolDisplayFormat format)
        {
            return UpdateFormat(format);
        }

        protected override SymbolDisplayFormat CreateAttributeFormat(SymbolDisplayFormat format)
        {
            return UpdateFormat(format);
        }

        private static SymbolDisplayFormat UpdateFormat(SymbolDisplayFormat format)
        {
            return format.Update(
                globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                memberOptions: format.MemberOptions | SymbolDisplayMemberOptions.IncludeContainingType,
                miscellaneousOptions: format.MiscellaneousOptions & ~SymbolDisplayMiscellaneousOptions.UseSpecialTypes);
        }

        public override void WriteDocument(IEnumerable<IAssemblySymbol> assemblies, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                _assemblies = assemblies.ToImmutableHashSet();
                base.WriteDocument(assemblies, cancellationToken);
            }
            finally
            {
                _assemblies = ImmutableHashSet<IAssemblySymbol>.Empty;
            }
        }

        public override void WriteStartDocument()
        {
            _writer.WriteRaw(@"<!DOCTYPE html>
");
            WriteStartElement("html");
            WriteStartElement("head");
            WriteStartElement("meta");
            WriteAttributeString("charset", "utf-8");
            WriteEndElement();
#if DEBUG
            _writer.WriteRaw(@"
<style type=""text/css"">
* { font-family: Consolas, Courier; }
</style>
");
#endif
            WriteEndElement();
            WriteStartElement("body");
            WriteStartElement("pre");
        }

        public override void WriteEndDocument()
        {
            WriteEndElement();
            WriteEndElement();
            WriteEndElement();
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
            if (namespaceSymbol.IsGlobalNamespace)
                return;

            WriteLocalRef(namespaceSymbol);
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
                WriteLocalRef(typeSymbol);
                WriteStartCodeElement();
            }
        }

        public override void WriteTypeDefinition(INamedTypeSymbol typeSymbol, SymbolDisplayFormat format = null, SymbolDisplayTypeDeclarationOptions? typeDeclarationOptions = null)
        {
            if (typeSymbol != null)
            {
                WriteDocumentationComment(typeSymbol);
                Write(typeSymbol, format ?? TypeFormat, typeDeclarationOptions);
                WriteEndElement();
            }

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
            if (Format.EmptyLineBetweenMembers)
                WriteLine();
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
                GetAdditionalOptions() & ~SymbolDisplayAdditionalOptions.OmitContainingNamespace);

            (int startIndex, int endIndex, ISymbol s) = SymbolDefinitionWriterHelpers.FindDefinitionName(symbol, parts);

            Debug.Assert(startIndex >= 0, symbol.ToDisplayString(Roslynator.SymbolDisplayFormats.Test));

            if (startIndex >= 0)
            {
                Debug.Assert(symbol == s, symbol.ToDisplayString(Roslynator.SymbolDisplayFormats.Test));

                WriteParts(parts, 0, startIndex);

                bool isOperator = s.IsKind(SymbolKind.Method)
                    && ((IMethodSymbol)s).MethodKind.Is(MethodKind.Conversion, MethodKind.UserDefinedOperator);

                if (!isOperator)
                    WriteStartElement("b");

                SymbolDisplayFormat f;

                if (s.IsKind(SymbolKind.Namespace))
                {
                    f = SymbolDefinitionDisplayFormats.TypeNameAndContainingTypesAndNamespaces;
                }
                else if (s.IsKind(SymbolKind.NamedType))
                {
                    f = _typeFormat;
                }
                else
                {
                    f = _memberFormat;
                }

                Write(s.ToDisplayParts(f));

                if (!isOperator)
                    WriteEndElement();

                startIndex = endIndex + 1;
            }
            else
            {
                startIndex = 0;
            }

            WriteParts(parts, startIndex, parts.Length - startIndex);
        }

        private void WriteParts(ImmutableArray<SymbolDisplayPart> parts)
        {
            WriteParts(parts, 0, parts.Length);
        }

        private void WriteParts(ImmutableArray<SymbolDisplayPart> parts, int startIndex, int length)
        {
            int max = startIndex + length;

            int i = startIndex;
            int j = 0;

            while (i < max)
            {
                if (parts[i].IsGlobalNamespace())
                {
                    j = i;

                    if (Peek().IsPunctuation("::")
                        && Peek(2).IsNamespaceOrTypeName())
                    {
                        j += 2;

                        while (Peek().IsPunctuation(".")
                            && Peek(2).IsNamespaceOrTypeName())
                        {
                            j += 2;
                        }

                        WriteLink(parts[j].Symbol.OriginalDefinition);

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

        private void WriteLink(ISymbol symbol)
        {
            if (_assemblies.Contains(symbol.ContainingAssembly))
            {
                WriteStartElement("a");
                WriteStartAttribute("href");
                Write("#");
                WriteLocalLink(symbol);
                WriteEndAttribute();
                WriteSymbol(symbol.IsKind(SymbolKind.Namespace));
                WriteEndElement();
            }
            else
            {
                string url = WellKnownExternalUrlProviders.MicrosoftDocs.CreateUrl(symbol).Url;

                if (url != null)
                {
                    WriteStartElement("a");
                    WriteAttributeString("href", url);
                    WriteSymbol(symbol.IsKind(SymbolKind.Namespace));
                    WriteEndElement();
                }
                else
                {
                    WriteSymbol(symbol.IsKind(SymbolKind.Namespace) || Format.Includes(SymbolDefinitionPartFilter.ContainingNamespace));
                }
            }

            void WriteSymbol(bool includeNamespaces)
            {
                SymbolDisplayFormat format = (includeNamespaces)
                    ? SymbolDefinitionDisplayFormats.TypeNameAndContainingTypesAndNamespaces
                    : SymbolDefinitionDisplayFormats.TypeNameAndContainingTypes;

                ImmutableArray<SymbolDisplayPart> parts = symbol.ToDisplayParts(format);

                if (symbol.Name.EndsWith("Attribute", StringComparison.Ordinal)
                    && symbol.IsKind(SymbolKind.NamedType)
                    && ((INamedTypeSymbol)symbol).InheritsFrom(MetadataNames.System_Attribute))
                {
                    parts = SymbolDefinitionWriterHelpers.RemoveAttributeSuffix(parts);
                }

                base.Write(parts);
            }
        }

        private void WriteLocalRef(ISymbol symbol)
        {
            WriteStartElement("a");
            WriteStartAttribute("name");
            WriteLocalLink(symbol);
            WriteEndAttribute();
            WriteEndElement();
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

        protected override void WriteAttributeClass(INamedTypeSymbol attributeClass, SymbolDisplayFormat format = null)
        {
            ImmutableArray<SymbolDisplayPart> parts = attributeClass.ToDisplayParts(AttributeFormat);

            WriteParts(parts);
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
            IEnumerable<string> elementsText = DocumentationProvider?.GetXmlDocumentation(symbol)?.GetElementsAsText(skipEmptyElement: true, makeSingleLine: true);

            if (elementsText == null)
                return;

            foreach (string elementText in elementsText)
            {
                XElement element = XElement.Parse(elementText, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);

                Dictionary<int, List<XElement>> elementsByLine = null;

                foreach (XElement e in element.Descendants())
                {
                    switch (XmlTagMapper.GetTagOrDefault(e.Name.LocalName))
                    {
                        case XmlTag.See:
                        case XmlTag.ParamRef:
                        case XmlTag.TypeParamRef:
                            {
                                int lineNumber = ((IXmlLineInfo)e).LineNumber;

                                if (elementsByLine == null)
                                    elementsByLine = new Dictionary<int, List<XElement>>();

                                if (elementsByLine.ContainsKey(lineNumber))
                                {
                                    elementsByLine[lineNumber].Add(e);
                                }
                                else
                                {
                                    elementsByLine.Add(lineNumber, new List<XElement>() { e });
                                }

                                break;
                            }
                    }
                }

                using (var sr = new StringReader(elementText))
                {
                    int lineNumber = 1;

                    string line = null;

                    while ((line = sr.ReadLine()) != null)
                    {
                        Write("/// ");

                        if (elementsByLine != null
                            && elementsByLine.TryGetValue(lineNumber, out List<XElement> elements))
                        {
                            int lastPos = 0;

                            foreach (XElement e in elements.OrderBy(e => ((IXmlLineInfo)e).LinePosition))
                            {
                                int linePos = ((IXmlLineInfo)e).LinePosition - 2;

                                switch (XmlTagMapper.GetTagOrDefault(e.Name.LocalName))
                                {
                                    case XmlTag.ParamRef:
                                    case XmlTag.TypeParamRef:
                                        {
                                            string name = e.Attribute("name")?.Value;

                                            if (name != null)
                                            {
                                                Write(line.Substring(lastPos, linePos - lastPos));
                                                Write(name);
                                            }

                                            lastPos = linePos + e.ToString().Length;
                                            break;
                                        }
                                    case XmlTag.See:
                                        {
                                            string commentId = e.Attribute("cref")?.Value;

                                            if (commentId != null)
                                            {
                                                Write(line.Substring(lastPos, linePos - lastPos));

                                                ISymbol s = DocumentationProvider.GetFirstSymbolForDeclarationId(commentId)?.OriginalDefinition;

                                                if (s != null)
                                                {
                                                    WriteLink(s);
                                                }
                                                else
                                                {
                                                    Debug.Fail(commentId);
                                                    Write(TextUtility.RemovePrefixFromDocumentationCommentId(commentId));
                                                }
                                            }
                                            else
                                            {
                                                string langword = e.Attribute("langword")?.Value;

                                                if (langword != null)
                                                {
                                                    Write(line.Substring(lastPos, linePos - lastPos));
                                                    Write(langword);
                                                }
                                            }

                                            lastPos = linePos + e.ToString().Length;
                                            break;
                                        }
                                }
                            }

                            WriteLine(line.Substring(lastPos));
                        }
                        else
                        {
                            WriteLine(line);
                        }

                        WriteIndentation();

                        lineNumber++;
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
