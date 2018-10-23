// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;
using Roslynator.Documentation;
using static Roslynator.Documentation.DocumentationOptions;

namespace Roslynator.CommandLine
{
    [Verb("generate-doc")]
    public class GenerateDocCommandLineOptions : AbstractGenerateDocCommandLineOptions
    {
        [Option(longName: "additional-xml-documentation")]
        public IEnumerable<string> AdditionalXmlDocumentation { get; set; }

        [Option(longName: "ignored-member-parts")]
        public IEnumerable<string> IgnoredMemberParts { get; set; }

        [Option(longName: "ignored-namespace-parts")]
        public IEnumerable<string> IgnoredNamespaceParts { get; set; }

        [Option(longName: "ignored-root-parts")]
        public IEnumerable<string> IgnoredRootParts { get; set; }

        [Option(longName: "ignored-type-parts")]
        public IEnumerable<string> IgnoredTypeParts { get; set; }

        [Option(longName: "inheritance-style", Default = DefaultValues.InheritanceStyle)]
        public InheritanceStyle InheritanceStyle { get; set; }

        [Option(longName: "include-all-derived-types")]
        public bool IncludeAllDerivedTypes { get; set; }

        [Option(longName: "include-ienumerable")]
        public bool IncludeIEnumerable { get; set; }

        [Option(longName: "include-inherited-interface-members")]
        public bool IncludeInheritedInterfaceMembers { get; set; }

        [Option(longName: "max-derived-types", Default = DefaultValues.MaxDerivedTypes)]
        public int MaxDerivedTypes { get; set; }

        [Option(longName: "no-delete", Default = false)]
        public bool NoDelete { get; set; }

        [Option(longName: "no-format-base-list")]
        public bool NoFormatBaseList { get; set; }

        [Option(longName: "no-format-constraints")]
        public bool NoFormatConstraints { get; set; }

        [Option(longName: "omit-attribute-arguments")]
        public bool OmitAttributeArguments { get; set; }

        [Option(longName: "omit-containing-namespace-parts")]
        public IEnumerable<string> OmitContainingNamespaceParts { get; set; }

        [Option(longName: "omit-inherited-atttributes")]
        public bool OmitInheritedAttributes { get; set; }

        [Option(longName: "omit-member-constant-value")]
        public bool OmitMemberConstantValue { get; set; }

        [Option(longName: "omit-member-implements")]
        public bool OmitMemberImplements { get; set; }

        [Option(longName: "omit-member-inherited-from")]
        public bool OmitMemberInheritedFrom { get; set; }

        [Option(longName: "omit-member-overrides")]
        public bool OmitMemberOverrides { get; set; }

        [Option(longName: "preferred-culture")]
        public string PreferredCulture { get; set; }
    }
}
