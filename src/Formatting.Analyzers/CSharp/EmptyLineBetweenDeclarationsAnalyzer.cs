// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class EmptyLineBetweenDeclarationsAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.AddEmptyLineBetweenDeclarations,
                    DiagnosticDescriptors.AddEmptyLineBetweenSinglelineDeclarations,
                    DiagnosticDescriptors.AddEmptyLineBetweenDeclarationAndDocumentationComment,
                    DiagnosticDescriptors.AddEmptyLineBetweenSinglelineDeclarationsOfDifferentKind,
                    DiagnosticDescriptors.RemoveEmptyLineBetweenSinglelineDeclarationsOfSameKind);
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
            MemberDeclarationSyntax member = null;
            MemberDeclarationSyntax previousMember = members[0];
            bool? isSingleline = null;
            bool? isPreviousSingleline = null;
            bool shouldReportDocumentationComment = !context.IsAnalyzerSuppressed(DiagnosticDescriptors.AddEmptyLineBetweenDeclarationAndDocumentationComment);

            for (int i = 1; i < count; i++, previousMember = member, isPreviousSingleline = isSingleline)
            {
                member = members[i];
                isSingleline = null;
                SyntaxTriviaList trailingTrivia = previousMember.GetTrailingTrivia();

                if (!SyntaxTriviaAnalysis.IsOptionalWhitespaceThenOptionalSingleLineCommentThenEndOfLineTrivia(trailingTrivia))
                    continue;

                SyntaxTriviaList leadingTrivia = member.GetLeadingTrivia();

                (bool containsDocumentationComment, bool containsUndefinedTrivia, bool startsWithEmptyLine) = AnalyzeLeadingTrivia(leadingTrivia);

                if (containsDocumentationComment)
                {
                    ReportDiagnostic(context, DiagnosticDescriptors.AddEmptyLineBetweenDeclarationAndDocumentationComment, trailingTrivia.Last());
                    continue;
                }

                if ((isSingleline ?? (isSingleline = tree.IsSingleLineSpan(member.Span, cancellationToken)).Value)
                    && (isPreviousSingleline ?? tree.IsSingleLineSpan(members[i - 1].Span, cancellationToken)))
                {
                    if (startsWithEmptyLine)
                    {
                        if (MemberKindEquals(previousMember, member))
                            ReportDiagnostic(context, DiagnosticDescriptors.RemoveEmptyLineBetweenSinglelineDeclarationsOfSameKind, leadingTrivia[0]);
                    }
                    else if (!containsUndefinedTrivia)
                    {
                        ReportDiagnostic(context, DiagnosticDescriptors.AddEmptyLineBetweenSinglelineDeclarations, trailingTrivia.Last());

                        if (!MemberKindEquals(previousMember, member))
                            ReportDiagnostic(context, DiagnosticDescriptors.AddEmptyLineBetweenSinglelineDeclarationsOfDifferentKind, trailingTrivia.Last());
                    }
                }
                else if (!startsWithEmptyLine
                    && !containsUndefinedTrivia)
                {
                    ReportDiagnostic(context, DiagnosticDescriptors.AddEmptyLineBetweenDeclarations, trailingTrivia.Last());
                }
            }
        }

        private static bool MemberKindEquals(MemberDeclarationSyntax member1, MemberDeclarationSyntax member2)
        {
            SyntaxKind kind1 = member1.Kind();
            SyntaxKind kind2 = member2.Kind();

            if (kind1 == kind2)
            {
                if (kind1 == SyntaxKind.FieldDeclaration)
                {
                    return ((FieldDeclarationSyntax)member1).Modifiers.Contains(SyntaxKind.ConstKeyword)
                        == ((FieldDeclarationSyntax)member2).Modifiers.Contains(SyntaxKind.ConstKeyword);
                }

                return true;
            }

            switch (kind1)
            {
                case SyntaxKind.EventDeclaration:
                    return kind2 == SyntaxKind.EventFieldDeclaration;
                case SyntaxKind.EventFieldDeclaration:
                    return kind2 == SyntaxKind.EventDeclaration;
            }

            return false;
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
            EnumMemberDeclarationSyntax member = null;
            EnumMemberDeclarationSyntax previousMember = members[0];
            bool? isSingleline = null;
            bool? isPreviousSingleline = null;

            for (int i = 1; i < count; i++, previousMember = member, isPreviousSingleline = isSingleline)
            {
                member = members[i];
                isSingleline = null;
                SyntaxToken commaToken = members.GetSeparator(i - 1);
                SyntaxTriviaList trailingTrivia = commaToken.TrailingTrivia;

                if (!SyntaxTriviaAnalysis.IsOptionalWhitespaceThenOptionalSingleLineCommentThenEndOfLineTrivia(trailingTrivia))
                    continue;

                SyntaxTriviaList leadingTrivia = member.GetLeadingTrivia();

                (bool containsDocumentationComment, bool containsUndefinedTrivia, bool startsWithEmptyLine) = AnalyzeLeadingTrivia(leadingTrivia);

                if (containsDocumentationComment)
                {
                    ReportDiagnostic(context, DiagnosticDescriptors.AddEmptyLineBetweenDeclarationAndDocumentationComment, trailingTrivia.Last());
                    continue;
                }

                if ((isSingleline ?? (isSingleline = tree.IsSingleLineSpan(member.Span, cancellationToken)).Value)
                    && (isPreviousSingleline ?? tree.IsSingleLineSpan(members[i - 1].Span, cancellationToken)))
                {
                    if (startsWithEmptyLine)
                    {
                        ReportDiagnostic(context, DiagnosticDescriptors.RemoveEmptyLineBetweenSinglelineDeclarationsOfSameKind, leadingTrivia[0]);
                    }
                    else if (!containsUndefinedTrivia)
                    {
                        ReportDiagnostic(context, DiagnosticDescriptors.AddEmptyLineBetweenSinglelineDeclarations, trailingTrivia.Last());
                    }
                }
                else if (!startsWithEmptyLine
                    && !containsUndefinedTrivia)
                {
                    ReportDiagnostic(context, DiagnosticDescriptors.AddEmptyLineBetweenDeclarations, trailingTrivia.Last());
                }
            }
        }

        private static (bool containsDocumentationComment, bool containsUndefinedTrivia, bool startsWithEmptyLine) AnalyzeLeadingTrivia(SyntaxTriviaList leadingTrivia)
        {
            SyntaxTriviaList.Enumerator en = leadingTrivia.GetEnumerator();

            if (!en.MoveNext())
                return default;

            if (en.Current.IsWhitespaceTrivia()
                && !en.MoveNext())
            {
                return default;
            }

            switch (en.Current.Kind())
            {
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                    {
                        return (true, false, false);
                    }
                case SyntaxKind.EndOfLineTrivia:
                    {
                        return (false, false, true);
                    }
            }

            return (false, true, false);
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
