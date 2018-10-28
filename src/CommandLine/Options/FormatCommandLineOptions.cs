// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp;
using System.Collections.Generic;

namespace Roslynator.CommandLine
{
    //TODO: NormalizeLineEndings
    [Verb("format")]
    public class FormatCommandLineOptions : MSBuildCommandLineOptions
    {
        [Option(longName: "empty-line-after-closing-brace")]
        public bool EmptyLineAfterClosingBrace { get; set; }

        [Option(longName: "empty-line-after-embedded-statement")]
        public bool EmptyLineAfterEmbeddedStatement { get; set; }

        [Option(longName: "empty-line-before-while-in-do-statement")]
        public bool EmptyLineBeforeWhileInDoStatement { get; set; }

        [Option(longName: "empty-line-between-declarations")]
        public bool EmptyLineBetweenDeclarations { get; set; }

        [Option(longName: "format-accessor-list")]
        public bool FormatAccessorList { get; set; }

        [Option(longName: "format-declaration-braces")]
        public bool FormatDeclarationBraces { get; set; }

        [Option(longName: "format-empty-block")]
        public bool FormatEmptyBlock { get; set; }

        [Option(longName: "format-single-line-block")]
        public bool FormatSingleLineBlock { get; set; }

        [Option(longName: "include-generated-code")]
        public bool IncludeGeneratedCode { get; set; }

        [Option(longName: "new-line-after-switch-label")]
        public bool NewLineAfterSwitchLabel { get; set; }

        [Option(longName: "new-line-before-embedded-statement")]
        public bool NewLineBeforeEmbeddedStatement { get; set; }

        [Option(longName: "new-line-before-enum-member")]
        public bool NewLineBeforeEnumMember { get; set; }

        [Option(longName: "new-line-before-statement")]
        public bool NewLineBeforeStatement { get; set; }

        [Option(longName: "remove-redundant-empty-line")]
        public bool RemoveRedundantEmptyLine { get; set; }

        internal IEnumerable<DiagnosticDescriptor> GetSupportedDiagnostics()
        {
            if (EmptyLineAfterClosingBrace)
                yield return DiagnosticDescriptors.AddEmptyLineAfterClosingBrace;

            if (EmptyLineAfterEmbeddedStatement)
                yield return DiagnosticDescriptors.AddEmptyLineAfterEmbeddedStatement;

            if (EmptyLineBeforeWhileInDoStatement)
                yield return DiagnosticDescriptors.AddEmptyLineAfterLastStatementInDoStatement;

            if (EmptyLineBetweenDeclarations)
                yield return DiagnosticDescriptors.AddEmptyLineBetweenDeclarations;

            if (FormatAccessorList)
                yield return DiagnosticDescriptors.FormatAccessorList;

            if (FormatDeclarationBraces)
                yield return DiagnosticDescriptors.FormatDeclarationBraces;

            if (FormatEmptyBlock)
                yield return DiagnosticDescriptors.FormatEmptyBlock;

            if (FormatSingleLineBlock)
                yield return DiagnosticDescriptors.AvoidSingleLineBlock;

            if (NewLineAfterSwitchLabel)
                yield return DiagnosticDescriptors.FormatSwitchSectionStatementOnSeparateLine;

            if (NewLineBeforeEmbeddedStatement)
                yield return DiagnosticDescriptors.FormatEmbeddedStatementOnSeparateLine;

            if (NewLineBeforeEnumMember)
                yield return DiagnosticDescriptors.FormatEachEnumMemberOnSeparateLine;

            if (NewLineBeforeStatement)
                yield return DiagnosticDescriptors.FormatEachStatementOnSeparateLine;

            if (RemoveRedundantEmptyLine)
                yield return DiagnosticDescriptors.RemoveRedundantEmptyLine;
        }
    }
}
