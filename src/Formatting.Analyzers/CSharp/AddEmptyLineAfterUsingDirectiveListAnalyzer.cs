﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class AddEmptyLineAfterUsingDirectiveListAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddEmptyLineAfterUsingDirectiveList); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeCompilationUnit, SyntaxKind.CompilationUnit);
        }

        private static void AnalyzeCompilationUnit(SyntaxNodeAnalysisContext context)
        {
            var compilationUnit = (CompilationUnitSyntax)context.Node;

            UsingDirectiveSyntax usingDirective = compilationUnit.Usings.LastOrDefault();

            if (usingDirective == null)
                return;

            SyntaxTriviaList trailingTrivia = usingDirective.GetTrailingTrivia();

            if (!SyntaxTriviaAnalysis.IsOptionalWhitespaceThenOptionalSingleLineCommentThenEndOfLineTrivia(triviaList: trailingTrivia))
                return;

            SyntaxToken nextToken = compilationUnit.AttributeLists.FirstOrDefault()?.OpenBracketToken
                ?? compilationUnit.Members.FirstOrDefault()?.GetFirstToken()
                ?? compilationUnit.EndOfFileToken;

            switch (nextToken.Parent.Kind())
            {
                case SyntaxKind.AttributeList:
                case SyntaxKind.NamespaceDeclaration:
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.DelegateDeclaration:
                    {
                        SyntaxTriviaList.Enumerator en = nextToken.LeadingTrivia.GetEnumerator();

                        if (en.MoveNext())
                        {
                            if (en.Current.IsWhitespaceTrivia()
                                && !en.MoveNext())
                            {
                                ReportDiagnostic(trailingTrivia.Last().SpanStart);
                            }
                            else
                            {
                                switch (en.Current.Kind())
                                {
                                    case SyntaxKind.SingleLineCommentTrivia:
                                    case SyntaxKind.SingleLineDocumentationCommentTrivia:
                                    case SyntaxKind.MultiLineDocumentationCommentTrivia:
                                        {
                                            ReportDiagnostic(trailingTrivia.Last().SpanStart);
                                            break;
                                        }
                                    case SyntaxKind.EndRegionDirectiveTrivia:
                                        {
                                            SyntaxTrivia endRegionDirective = en.Current;

                                            if (en.MoveNext())
                                            {
                                                if (en.Current.IsWhitespaceTrivia()
                                                    && !en.MoveNext())
                                                {
                                                    ReportDiagnostic(endRegionDirective.Span.End);
                                                }
                                                else if (!en.Current.IsEndOfLineTrivia())
                                                {
                                                    ReportDiagnostic(endRegionDirective.Span.End);
                                                }
                                            }
                                            else
                                            {
                                                ReportDiagnostic(endRegionDirective.Span.End);
                                            }

                                            break;
                                        }
                                    default:
                                        {
                                            if (en.Current.IsDirective)
                                                ReportDiagnostic(trailingTrivia.Last().SpanStart);

                                            break;
                                        }
                                }
                            }
                        }
                        else
                        {
                            ReportDiagnostic(trailingTrivia.Last().SpanStart);
                        }

                        break;
                    }
                default:
                    {
                        Debug.Fail(nextToken.Parent.Kind().ToString());
                        break;
                    }
            }

            void ReportDiagnostic(int position)
            {
                context.ReportDiagnostic(
                   DiagnosticDescriptors.AddEmptyLineAfterUsingDirectiveList,
                   Location.Create(compilationUnit.SyntaxTree, new TextSpan(position, 0)));
            }
        }
    }
}
