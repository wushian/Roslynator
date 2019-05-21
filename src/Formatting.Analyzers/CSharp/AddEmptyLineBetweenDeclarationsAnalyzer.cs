// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class AddEmptyLineBetweenDeclarationsAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.AddEmptyLineBetweenDeclarations,
                    DiagnosticDescriptors.AddEmptyLineBetweenSinglelineDeclarations,
                    DiagnosticDescriptors.AddEmptyLineBetweenDeclarationAndDocumentationComment);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeCompilationUnit, SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeAction(AnalyzeNamespaceDeclaration, SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeEnumDeclaration, SyntaxKind.EnumDeclaration);
            //TODO: AnalyzeAccessorDeclaration
        }

        private static void AnalyzeCompilationUnit(SyntaxNodeAnalysisContext context)
        {
            var compilationUnit = (CompilationUnitSyntax)context.Node;

            Analyze(context, compilationUnit.Members);
        }

        private static void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;

            Analyze(context, namespaceDeclaration.Members);
        }

        private static void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            Analyze(context, typeDeclaration.Members);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, SyntaxList<MemberDeclarationSyntax> members)
        {
            int count = members.Count;

            if (count <= 1)
                return;

            SyntaxTree tree = context.Node.SyntaxTree;
            CancellationToken cancellationToken = context.CancellationToken;
            MemberDeclarationSyntax previousMember = members[0];
            bool? isSingleline = null;
            bool? isPreviousSingleline = null;
            bool shouldReportDocumentationComment = !context.IsAnalyzerSuppressed(DiagnosticDescriptors.AddEmptyLineBetweenDeclarationAndDocumentationComment);

            for (int i = 1; i < count; i++)
            {
                MemberDeclarationSyntax member = members[i];
                isSingleline = null;

                SyntaxTrivia endOfLine = previousMember.GetTrailingTrivia().LastOrDefault();

                if (endOfLine.IsEndOfLineTrivia())
                {
                    bool containsEndOfLineOrDocumentationComment = false;

                    foreach (SyntaxTrivia trivia in member.GetLeadingTrivia())
                    {
                        SyntaxKind kind = trivia.Kind();

                        if (SyntaxFacts.IsDocumentationCommentTrivia(kind))
                        {
                            if (shouldReportDocumentationComment
                                && tree.GetLineCount(TextSpan.FromBounds(previousMember.Span.End, trivia.SpanStart), cancellationToken) == 2)
                            {
                                ReportDiagnostic(context, DiagnosticDescriptors.AddEmptyLineBetweenDeclarationAndDocumentationComment, endOfLine);
                            }

                            containsEndOfLineOrDocumentationComment = true;
                            break;
                        }
                        else if (kind == SyntaxKind.EndOfLineTrivia)
                        {
                            containsEndOfLineOrDocumentationComment = true;
                            break;
                        }
                    }

                    if (!containsEndOfLineOrDocumentationComment
                        && tree.GetLineCount(TextSpan.FromBounds(previousMember.Span.End, member.SpanStart), cancellationToken) == 2)
                    {
                        isSingleline = tree.IsSingleLineSpan(member.Span, cancellationToken);

                        if (isSingleline.Value
                            && (isPreviousSingleline ?? tree.IsSingleLineSpan(previousMember.Span, cancellationToken)))
                        {
                            ReportDiagnostic(context, DiagnosticDescriptors.AddEmptyLineBetweenSinglelineDeclarations, endOfLine);
                        }
                        else
                        {
                            ReportDiagnostic(context, DiagnosticDescriptors.AddEmptyLineBetweenDeclarations, endOfLine);
                        }
                    }
                }

                previousMember = member;
                isPreviousSingleline = isSingleline;
            }
        }

        private static void AnalyzeEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            var enumDeclaration = (EnumDeclarationSyntax)context.Node;

            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

            int count = members.Count;

            if (count <= 1)
                return;

            SyntaxTree tree = enumDeclaration.SyntaxTree;
            CancellationToken cancellationToken = context.CancellationToken;
            EnumMemberDeclarationSyntax previousMember = members[0];
            bool? isSingleline = null;
            bool? isPreviousSingleline = null;
            bool shouldReportDocumentationComment = !context.IsAnalyzerSuppressed(DiagnosticDescriptors.AddEmptyLineBetweenDeclarationAndDocumentationComment);

            for (int i = 1; i < count; i++)
            {
                SyntaxToken commaToken = members.GetSeparator(i - 1);
                EnumMemberDeclarationSyntax member = members[i];
                isSingleline = null;

                SyntaxTrivia endOfLine = commaToken.TrailingTrivia.LastOrDefault();

                if (endOfLine.IsEndOfLineTrivia())
                {
                    bool containsEndOfLineOrDocumentationComment = false;

                    foreach (SyntaxTrivia trivia in member.GetLeadingTrivia())
                    {
                        SyntaxKind kind = trivia.Kind();

                        if (SyntaxFacts.IsDocumentationCommentTrivia(kind))
                        {
                            if (shouldReportDocumentationComment
                                && tree.GetLineCount(TextSpan.FromBounds(commaToken.Span.End, trivia.SpanStart), cancellationToken) == 2)
                            {
                                ReportDiagnostic(context, DiagnosticDescriptors.AddEmptyLineBetweenDeclarationAndDocumentationComment, endOfLine);
                            }

                            containsEndOfLineOrDocumentationComment = true;
                            break;
                        }
                        else if (kind == SyntaxKind.EndOfLineTrivia)
                        {
                            containsEndOfLineOrDocumentationComment = true;
                            break;
                        }
                    }

                    if (!containsEndOfLineOrDocumentationComment
                        && tree.GetLineCount(TextSpan.FromBounds(commaToken.Span.End, member.SpanStart), cancellationToken) == 2)
                    {
                        isSingleline = tree.IsSingleLineSpan(member.Span, cancellationToken);

                        if (isSingleline.Value
                            && (isPreviousSingleline ?? tree.IsSingleLineSpan(members[i - 1].Span, cancellationToken)))
                        {
                            ReportDiagnostic(context, DiagnosticDescriptors.AddEmptyLineBetweenSinglelineDeclarations, endOfLine);
                        }
                        else
                        {
                            ReportDiagnostic(context, DiagnosticDescriptors.AddEmptyLineBetweenDeclarations, endOfLine);
                        }
                    }
                }

                previousMember = member;
                isPreviousSingleline = isSingleline;
            }
        }

        private static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxTrivia trivia)
        {
            if (!context.IsAnalyzerSuppressed(descriptor))
                context.ReportDiagnostic(descriptor, Location.Create(context.Node.SyntaxTree, trivia.Span.WithLength(0)));
        }
    }
}
