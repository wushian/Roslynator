﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// <auto-generated>

using System;
using Microsoft.CodeAnalysis;

namespace Roslynator.CodeAnalysis.CSharp
{
    public static partial class DiagnosticDescriptors
    {
        /// <summary>RCS9001</summary>
        public static readonly DiagnosticDescriptor UsePatternMatching = new DiagnosticDescriptor(
            id:                 DiagnosticIdentifiers.UsePatternMatching, 
            title:              "Use pattern matching.", 
            messageFormat:      "Use pattern matching.", 
            category:           DiagnosticCategories.Usage, 
            defaultSeverity:    DiagnosticSeverity.Hidden, 
            isEnabledByDefault: true, 
            description:        null, 
            helpLinkUri:        $"{HelpLinkUriRoot}{DiagnosticIdentifiers.UsePatternMatching}", 
            customTags:         Array.Empty<string>());

        /// <summary>RCS9002</summary>
        public static readonly DiagnosticDescriptor UsePropertySyntaxNodeSpanStart = new DiagnosticDescriptor(
            id:                 DiagnosticIdentifiers.UsePropertySyntaxNodeSpanStart, 
            title:              "Use property SyntaxNode.SpanStart.", 
            messageFormat:      "Use property SyntaxNode.SpanStart.", 
            category:           DiagnosticCategories.Performance, 
            defaultSeverity:    DiagnosticSeverity.Info, 
            isEnabledByDefault: true, 
            description:        null, 
            helpLinkUri:        $"{HelpLinkUriRoot}{DiagnosticIdentifiers.UsePropertySyntaxNodeSpanStart}", 
            customTags:         Array.Empty<string>());

        /// <summary>RCS9003</summary>
        public static readonly DiagnosticDescriptor UnnecessaryConditionalAccess = new DiagnosticDescriptor(
            id:                 DiagnosticIdentifiers.UnnecessaryConditionalAccess, 
            title:              "Unnecessary conditional access.", 
            messageFormat:      "Unnecessary conditional access.", 
            category:           DiagnosticCategories.Performance, 
            defaultSeverity:    DiagnosticSeverity.Info, 
            isEnabledByDefault: true, 
            description:        null, 
            helpLinkUri:        $"{HelpLinkUriRoot}{DiagnosticIdentifiers.UnnecessaryConditionalAccess}", 
            customTags:         WellKnownDiagnosticTags.Unnecessary);

        public static readonly DiagnosticDescriptor UnnecessaryConditionalAccessFadeOut = UnnecessaryConditionalAccess.CreateFadeOut();

        /// <summary>RCS9004</summary>
        public static readonly DiagnosticDescriptor CallAnyInsteadOfAccessingCount = new DiagnosticDescriptor(
            id:                 DiagnosticIdentifiers.CallAnyInsteadOfAccessingCount, 
            title:              "Call 'Any' instead of accessing 'Count'.", 
            messageFormat:      "Call 'Any' instead of accessing 'Count'.", 
            category:           DiagnosticCategories.Performance, 
            defaultSeverity:    DiagnosticSeverity.Info, 
            isEnabledByDefault: true, 
            description:        null, 
            helpLinkUri:        $"{HelpLinkUriRoot}{DiagnosticIdentifiers.CallAnyInsteadOfAccessingCount}", 
            customTags:         Array.Empty<string>());

        /// <summary>RCS9005</summary>
        public static readonly DiagnosticDescriptor UnnecessaryNullCheck = new DiagnosticDescriptor(
            id:                 DiagnosticIdentifiers.UnnecessaryNullCheck, 
            title:              "Unnecessary null check.", 
            messageFormat:      "Unnecessary null check.", 
            category:           DiagnosticCategories.Performance, 
            defaultSeverity:    DiagnosticSeverity.Info, 
            isEnabledByDefault: true, 
            description:        null, 
            helpLinkUri:        $"{HelpLinkUriRoot}{DiagnosticIdentifiers.UnnecessaryNullCheck}", 
            customTags:         WellKnownDiagnosticTags.Unnecessary);

        /// <summary>RCS9006</summary>
        public static readonly DiagnosticDescriptor UseElementAccess = new DiagnosticDescriptor(
            id:                 DiagnosticIdentifiers.UseElementAccess, 
            title:              "Use element access.", 
            messageFormat:      "Use element access.", 
            category:           DiagnosticCategories.Usage, 
            defaultSeverity:    DiagnosticSeverity.Info, 
            isEnabledByDefault: true, 
            description:        null, 
            helpLinkUri:        $"{HelpLinkUriRoot}{DiagnosticIdentifiers.UseElementAccess}", 
            customTags:         Array.Empty<string>());

        /// <summary>RCS9007</summary>
        public static readonly DiagnosticDescriptor UseReturnValue = new DiagnosticDescriptor(
            id:                 DiagnosticIdentifiers.UseReturnValue, 
            title:              "Use return value.", 
            messageFormat:      "Use return value.", 
            category:           DiagnosticCategories.Usage, 
            defaultSeverity:    DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, 
            description:        null, 
            helpLinkUri:        $"{HelpLinkUriRoot}{DiagnosticIdentifiers.UseReturnValue}", 
            customTags:         Array.Empty<string>());

        /// <summary>RCS9008</summary>
        public static readonly DiagnosticDescriptor CallLastInsteadOfUsingElementAccess = new DiagnosticDescriptor(
            id:                 DiagnosticIdentifiers.CallLastInsteadOfUsingElementAccess, 
            title:              "Call 'Last' instead of using [].", 
            messageFormat:      "Call 'Last' instead of using [].", 
            category:           DiagnosticCategories.Usage, 
            defaultSeverity:    DiagnosticSeverity.Info, 
            isEnabledByDefault: true, 
            description:        null, 
            helpLinkUri:        $"{HelpLinkUriRoot}{DiagnosticIdentifiers.CallLastInsteadOfUsingElementAccess}", 
            customTags:         Array.Empty<string>());

        /// <summary>RCS9009</summary>
        public static readonly DiagnosticDescriptor UnknownLanguageName = new DiagnosticDescriptor(
            id:                 DiagnosticIdentifiers.UnknownLanguageName, 
            title:              "Unknown language name.", 
            messageFormat:      "Unknown language name.", 
            category:           DiagnosticCategories.General, 
            defaultSeverity:    DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, 
            description:        null, 
            helpLinkUri:        $"{HelpLinkUriRoot}{DiagnosticIdentifiers.UnknownLanguageName}", 
            customTags:         Array.Empty<string>());

        /// <summary>RCS9010</summary>
        public static readonly DiagnosticDescriptor SpecifyExportCodeRefactoringProviderAttributeName = new DiagnosticDescriptor(
            id:                 DiagnosticIdentifiers.SpecifyExportCodeRefactoringProviderAttributeName, 
            title:              "Specify ExportCodeRefactoringProviderAttribute.Name.", 
            messageFormat:      "Specify ExportCodeRefactoringProviderAttribute.Name.", 
            category:           DiagnosticCategories.Usage, 
            defaultSeverity:    DiagnosticSeverity.Hidden, 
            isEnabledByDefault: true, 
            description:        null, 
            helpLinkUri:        $"{HelpLinkUriRoot}{DiagnosticIdentifiers.SpecifyExportCodeRefactoringProviderAttributeName}", 
            customTags:         Array.Empty<string>());

        /// <summary>RCS9011</summary>
        public static readonly DiagnosticDescriptor SpecifyExportCodeFixProviderAttributeName = new DiagnosticDescriptor(
            id:                 DiagnosticIdentifiers.SpecifyExportCodeFixProviderAttributeName, 
            title:              "Specify ExportCodeFixProviderAttribute.Name.", 
            messageFormat:      "Specify ExportCodeFixProviderAttribute.Name.", 
            category:           DiagnosticCategories.Usage, 
            defaultSeverity:    DiagnosticSeverity.Hidden, 
            isEnabledByDefault: true, 
            description:        null, 
            helpLinkUri:        $"{HelpLinkUriRoot}{DiagnosticIdentifiers.SpecifyExportCodeFixProviderAttributeName}", 
            customTags:         Array.Empty<string>());

    }
}