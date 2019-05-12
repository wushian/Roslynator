// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, LanguageNames.VisualBasic, Name = nameof(NewlineCodeFixProvider))]
    [Shared]
    public class NewlineCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.UseLinefeedAsNewline,
                    DiagnosticIdentifiers.UseCarriageReturnAndLinefeedAsNewline);
            }
        }

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];
            TextSpan span = context.Span;

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.UseLinefeedAsNewline:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Use linefeed as newline",
                            ct => document.WithTextChangeAsync(new TextChange(span, "\n"), ct),
                            base.GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIdentifiers.UseCarriageReturnAndLinefeedAsNewline:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Use carriage return + linefeed as newline",
                            ct => document.WithTextChangeAsync(new TextChange(span, "\r\n"), ct),
                            base.GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }

            return Task.CompletedTask;
        }
    }
}

