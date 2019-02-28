// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Roslynator.FindSymbols;

namespace Roslynator.Documentation.Json
{
    internal class SymbolDefinitionJsonWriter : SymbolDefinitionWriter
    {
        private readonly JsonWriter _writer;
        private StringBuilder _attributeStringBuilder;
        private SymbolDefinitionWriter _attributeWriter;

        public SymbolDefinitionJsonWriter(
            JsonWriter writer,
            SymbolFilterOptions filter = null,
            DefinitionListFormat format = null,
            SymbolDocumentationProvider documentationProvider = null) : base(filter, format, documentationProvider)
        {
            _writer = writer;
        }

        public override bool SupportsMultilineDefinitions => false;

        protected override SymbolDisplayFormat CreateNamespaceFormat(SymbolDisplayFormat format)
        {
            return format.Update(kindOptions: SymbolDisplayKindOptions.None);
        }

        protected override SymbolDisplayAdditionalOptions GetAdditionalOptions()
        {
            return base.GetAdditionalOptions() & ~(SymbolDisplayAdditionalOptions.IncludeAccessorAttributes
                | SymbolDisplayAdditionalOptions.IncludeParameterAttributes
                | SymbolDisplayAdditionalOptions.IncludeTrailingSemicolon);
        }

        public override void WriteStartDocument()
        {
            WriteStartObject();
        }

        public override void WriteEndDocument()
        {
            WriteEndObject();
        }

        public override void WriteStartAssemblies()
        {
            WriteStartObject("assemblies");
        }

        public override void WriteEndAssemblies()
        {
            WriteEndObject();
        }

        public override void WriteStartAssembly(IAssemblySymbol assemblySymbol)
        {
            WriteStartObject("assembly");
        }

        public override void WriteAssembly(IAssemblySymbol assemblySymbol)
        {
            WriteProperty("name", assemblySymbol.Identity.ToString());
        }

        public override void WriteEndAssembly(IAssemblySymbol assemblySymbol)
        {
            WriteEndObject();
        }

        public override void WriteAssemblySeparator()
        {
        }

        public override void WriteStartNamespaces()
        {
            WriteStartObject("namespaces");
        }

        public override void WriteEndNamespaces()
        {
            WriteEndObject();
        }

        public override void WriteStartNamespace(INamespaceSymbol namespaceSymbol)
        {
            WriteStartObject("namespace");
        }

        public override void WriteNamespace(INamespaceSymbol namespaceSymbol, SymbolDisplayFormat format = null)
        {
            WritePropertyName("name");

            if (!namespaceSymbol.IsGlobalNamespace)
                Write(namespaceSymbol, format ?? NamespaceFormat);

            WriteDocumentationComment(namespaceSymbol);
        }

        public override void WriteEndNamespace(INamespaceSymbol namespaceSymbol)
        {
            WriteEndObject();
        }

        public override void WriteNamespaceSeparator()
        {
        }

        public override void WriteStartTypes()
        {
            WriteStartObject("types");
        }

        public override void WriteEndTypes()
        {
            WriteEndObject();
        }

        public override void WriteStartType(INamedTypeSymbol typeSymbol)
        {
            WriteStartObject("type");
        }

        public override void WriteType(INamedTypeSymbol typeSymbol, SymbolDisplayFormat format = null, SymbolDisplayTypeDeclarationOptions? typeDeclarationOptions = null)
        {
            if (typeSymbol != null)
            {
                WritePropertyName("def");
                Write(typeSymbol, format ?? TypeFormat, typeDeclarationOptions);
                WriteDocumentationComment(typeSymbol);

                if (Format.Includes(SymbolDefinitionPartFilter.Attributes))
                    WriteAttributes(typeSymbol);
            }
            else
            {
                WriteProperty("def", "");
            }
        }

        public override void WriteEndType(INamedTypeSymbol typeSymbol)
        {
            WriteEndObject();
        }

        public override void WriteTypeSeparator()
        {
        }

        public override void WriteStartMembers()
        {
            WriteStartObject("members");
        }

        public override void WriteEndMembers()
        {
            WriteEndObject();
        }

        public override void WriteStartMember(ISymbol symbol)
        {
            WriteStartObject("member");
        }

        public override void WriteMember(ISymbol symbol, SymbolDisplayFormat format = null)
        {
            if (format == null)
            {
                format = (symbol.GetFirstExplicitInterfaceImplementation() != null)
                    ? ExplicitInterfaceImplementationFormat
                    : MemberFormat;
            }

            WritePropertyName("def");
            Write(symbol, format);
            WriteDocumentationComment(symbol);

            if (Format.Includes(SymbolDefinitionPartFilter.Attributes))
            {
                WriteAttributes(symbol);

                switch (symbol.Kind)
                {
                    case SymbolKind.NamedType:
                        {
                            var typeSymbol = (INamedTypeSymbol)symbol;

                            if (typeSymbol.TypeKind == TypeKind.Delegate)
                            {
                                foreach (IParameterSymbol parameterSymbol in typeSymbol.DelegateInvokeMethod.Parameters)
                                    WriteAttributes(parameterSymbol);
                            }

                            break;
                        }
                    case SymbolKind.Event:
                        {
                            var eventSymbol = (IEventSymbol)symbol;

                            if (eventSymbol.AddMethod != null)
                                WriteAttributes(eventSymbol.AddMethod);

                            if (eventSymbol.RemoveMethod != null)
                                WriteAttributes(eventSymbol.RemoveMethod);

                            break;
                        }
                    case SymbolKind.Method:
                        {
                            var methodSymbol = (IMethodSymbol)symbol;

                            foreach (IParameterSymbol parameterSymbol in methodSymbol.Parameters)
                                WriteAttributes(parameterSymbol);

                            break;
                        }
                    case SymbolKind.Property:
                        {
                            var propertySymbol = (IPropertySymbol)symbol;

                            foreach (IParameterSymbol parameterSymbol in propertySymbol.Parameters)
                                WriteAttributes(parameterSymbol);

                            if (propertySymbol.GetMethod != null)
                                WriteAttributes(propertySymbol.GetMethod);

                            if (propertySymbol.SetMethod != null)
                                WriteAttributes(propertySymbol.SetMethod);

                            break;
                        }
                }
            }
        }

        public override void WriteEndMember(ISymbol symbol)
        {
            WriteEndObject();
        }

        public override void WriteMemberSeparator()
        {
        }

        public override void WriteStartEnumMembers()
        {
            WriteStartObject("members");
        }

        public override void WriteEndEnumMembers()
        {
            WriteEndObject();
        }

        public override void WriteStartEnumMember(ISymbol symbol)
        {
            WriteStartObject("member");
        }

        public override void WriteEnumMember(ISymbol symbol, SymbolDisplayFormat format = null)
        {
            WritePropertyName("def");
            Write(symbol, format ?? EnumMemberFormat);
            WriteDocumentationComment(symbol);

            if (Format.Includes(SymbolDefinitionPartFilter.Attributes))
                WriteAttributes(symbol);
        }

        public override void WriteEndEnumMember(ISymbol symbol)
        {
            WriteEndObject();
        }

        public override void WriteEnumMemberSeparator()
        {
        }

        public override void WriteStartAttributes(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Method:
                    {
                        var methodSymbol = (IMethodSymbol)symbol;

                        string accessorName = GetAccessorName(methodSymbol);

                        if (accessorName != null)
                        {
                            WriteStartObject("accessor");
                            WriteProperty("name", accessorName);
                        }

                        break;
                    }
                case SymbolKind.Parameter:
                    {
                        var parameterSymbol = (IParameterSymbol)symbol;

                        WritePropertyName("parameter");
                        WriteProperty("name", parameterSymbol.Name);
                        break;
                    }
            }

            WritePropertyName("attributes");
            WriteStartArray();

            string GetAccessorName(IMethodSymbol methodSymbol)
            {
                switch (methodSymbol.MethodKind)
                {
                    case MethodKind.EventAdd:
                        return "add";
                    case MethodKind.EventRemove:
                        return "remove";
                    case MethodKind.PropertyGet:
                        return "get";
                    case MethodKind.PropertySet:
                        return "set";
                    default:
                        return null;
                }
            }
        }

        public override void WriteEndAttributes(ISymbol symbol)
        {
            WriteEndArray();

            switch (symbol.Kind)
            {
                case SymbolKind.Parameter:
                    {
                        WriteEndObject();
                        break;
                    }
                case SymbolKind.Method:
                    {
                        var methodSymbol = (IMethodSymbol)symbol;

                        if (methodSymbol.MethodKind.Is(
                            MethodKind.PropertyGet,
                            MethodKind.PropertySet,
                            MethodKind.EventAdd,
                            MethodKind.EventRemove))
                        {
                            WriteEndObject();
                        }

                        break;
                    }
            }
        }

        public override void WriteStartAttribute(AttributeData attribute, ISymbol symbol)
        {
        }

        public override void WriteAttribute(AttributeData attribute)
        {
            if (_attributeWriter == null)
            {
                _attributeStringBuilder = new StringBuilder();
                var stringWriter = new StringWriter(_attributeStringBuilder);
                _attributeWriter = new SymbolDefinitionTextWriter(stringWriter, Filter, Format, DocumentationProvider);
            }

            _attributeWriter.WriteAttribute(attribute);

            WriteValue(_attributeStringBuilder.ToString());

            _attributeStringBuilder.Clear();
        }

        public override void WriteEndAttribute(AttributeData attribute, ISymbol symbol)
        {
        }

        public override void WriteAttributeSeparator(ISymbol symbol)
        {
        }

        public override void Write(IEnumerable<SymbolDisplayPart> parts)
        {
            if (_attributeWriter == null)
            {
                _attributeStringBuilder = new StringBuilder();
                var stringWriter = new StringWriter(_attributeStringBuilder);
                _attributeWriter = new SymbolDefinitionTextWriter(stringWriter, Filter, Format, DocumentationProvider);
            }

            _attributeWriter.Write(parts);

            WriteValue(_attributeStringBuilder.ToString());

            _attributeStringBuilder.Clear();
        }

        public void WriteValue(string value)
        {
            Debug.Assert(value?.Contains("\n") != true, @"\n");
            Debug.Assert(value?.Contains("\r") != true, @"\r");

            _writer.WriteValue(value);
        }

        public override void Write(string value)
        {
            Debug.Assert(value?.Contains("\n") != true, @"\n");
            Debug.Assert(value?.Contains("\r") != true, @"\r");

            _writer.WriteRawValue(value);
        }

        public override void WriteLine()
        {
            throw new InvalidOperationException();
        }

        public override void WriteLine(string value)
        {
            throw new InvalidOperationException();
        }

        private void WriteStartObject()
        {
            _writer.WriteStartObject();
            IncreaseDepth();
        }

        private void WriteStartObject(string name)
        {
            _writer.WritePropertyName(name);
            WriteStartObject();
        }

        private void WriteEndObject()
        {
            _writer.WriteEndObject();
            DecreaseDepth();
        }

        private void WriteStartArray()
        {
            _writer.WriteStartArray();
            IncreaseDepth();
        }

        private void WriteEndArray()
        {
            _writer.WriteEndArray();
            DecreaseDepth();
        }

        private void WritePropertyName(string name)
        {
            _writer.WritePropertyName(name);
        }

        private void WriteProperty(string name, string value)
        {
            WritePropertyName(name);
            Write(value);
        }

        public override void WriteDocumentationComment(ISymbol symbol)
        {
        }
    }
}
