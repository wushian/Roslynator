// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp;
using System.Collections.Generic;

namespace Roslynator.CommandLine
{
    [Verb("format", HelpText = "Formats documents in the specified project or solution.")]
    public class FormatCommandLineOptions : MSBuildCommandLineOptions
    {
        [Option(longName: "culture")]
        public string Culture { get; set; }

        [Option(longName: "empty-line-after-closing-brace")]
        public bool EmptyLineAfterClosingBrace { get; set; }

        [Option(longName: "empty-line-after-embedded-statement")]
        public bool EmptyLineAfterEmbeddedStatement { get; set; }

        [Option(longName: "empty-line-before-while-in-do-statement")]
        public bool EmptyLineBeforeWhileInDoStatement { get; set; }

        [Option(longName: "empty-line-between-declarations")]
        public bool EmptyLineBetweenDeclarations { get; set; }

        [Option(longName: "end-of-line")]
        public string EndOfLine { get; set; }

        [Option(longName: "format-accessor-list")]
        public bool FormatAccessorList { get; set; }

        //TODO: FormatEmptyDeclarationBraces
        [Option(longName: "format-declaration-braces")]
        public bool FormatDeclarationBraces { get; set; }

        //TODO: FormatEmptyBlockBraces
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

        [Option(longName: "new-line-before-binary-operator")]
        public bool NewLineBeforeBinaryOperator { get; set; }

        [Option(longName: "new-line-before-conditional-expression-operator")]
        public bool NewLineBeforeConditionalExpressionOperator { get; set; }

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
                yield return DiagnosticDescriptors.AddEmptyLineBeforeWhileInDoStatement;

            if (EmptyLineBetweenDeclarations)
                yield return DiagnosticDescriptors.AddEmptyLineBetweenDeclarations;

            if (FormatAccessorList)
                yield return DiagnosticDescriptors.FormatAccessorList;

            if (FormatDeclarationBraces)
                yield return DiagnosticDescriptors.FormatDeclarationBraces;

            if (FormatEmptyBlock)
                yield return DiagnosticDescriptors.FormatEmptyBlock;

            if (FormatSingleLineBlock)
                yield return DiagnosticDescriptors.FormatSingleLineBlock;

            if (NewLineAfterSwitchLabel)
                yield return DiagnosticDescriptors.AddNewLineAfterSwitchLabel;

            if (NewLineBeforeBinaryOperator)
                yield return DiagnosticDescriptors.FormatBinaryOperatorOnNextLine;

            if (NewLineBeforeConditionalExpressionOperator)
                yield return DiagnosticDescriptors.FormatConditionalExpression;

            if (NewLineBeforeEmbeddedStatement)
                yield return DiagnosticDescriptors.AddNewLineBeforeEmbeddedStatement;

            if (NewLineBeforeEnumMember)
                yield return DiagnosticDescriptors.AddNewLineBeforeEnumMember;

            if (NewLineBeforeStatement)
                yield return DiagnosticDescriptors.AddNewLineBeforeStatement;

            if (RemoveRedundantEmptyLine)
                yield return DiagnosticDescriptors.RemoveRedundantEmptyLine;

            if (EndOfLine == "lf")
            {
                yield return DiagnosticDescriptors.UseLinefeedAsNewLine;
            }
            else if (EndOfLine == "crlf")
            {
                yield return DiagnosticDescriptors.UseCarriageReturnAndLinefeedAsNewLine;
            }
        }
    }
}
