// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class AddEmptyLineAfterRegionAndBeforeEndRegionAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddEmptyLineAfterRegionAndBeforeEndRegion); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeRegionDirectiveTrivia, SyntaxKind.RegionDirectiveTrivia);
            context.RegisterSyntaxNodeAction(AnalyzeEndRegionDirectiveTrivia, SyntaxKind.EndRegionDirectiveTrivia);
        }

        private static void AnalyzeRegionDirectiveTrivia(SyntaxNodeAnalysisContext context)
        {
            var regionDirective = (RegionDirectiveTriviaSyntax)context.Node;

            if (IsFollowedWithEmptyLineOrEndRegionDirective())
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.AddEmptyLineAfterRegionAndBeforeEndRegion,
                Location.Create(regionDirective.SyntaxTree, regionDirective.EndOfDirectiveToken.Span),
                "after #region");

            bool IsFollowedWithEmptyLineOrEndRegionDirective()
            {
                SyntaxTrivia parentTrivia = regionDirective.ParentTrivia;

                SyntaxTriviaList.Enumerator en = parentTrivia.Token.LeadingTrivia.GetEnumerator();

                while (en.MoveNext())
                {
                    if (en.Current == parentTrivia)
                    {
                        if (!en.MoveNext())
                            return false;

                        if (en.Current.IsWhitespaceTrivia()
                            && !en.MoveNext())
                        {
                            return false;
                        }

                        if (en.Current.IsKind(SyntaxKind.EndRegionDirectiveTrivia))
                            return true;

                        return en.Current.IsEndOfLineTrivia();
                    }
                }

                return false;
            }
        }

        private static void AnalyzeEndRegionDirectiveTrivia(SyntaxNodeAnalysisContext context)
        {
            var endRegionDirective = (EndRegionDirectiveTriviaSyntax)context.Node;

            if (IsPrecededWithEmptyLineOrRegionDirective())
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.AddEmptyLineAfterRegionAndBeforeEndRegion,
                Location.Create(endRegionDirective.SyntaxTree, endRegionDirective.Span.WithLength(0)),
                "before #endregion");

            bool IsPrecededWithEmptyLineOrRegionDirective()
            {
                SyntaxTrivia parentTrivia = endRegionDirective.ParentTrivia;

                SyntaxTriviaList.Reversed.Enumerator en = parentTrivia.Token.LeadingTrivia.Reverse().GetEnumerator();

                while (en.MoveNext())
                {
                    if (en.Current == parentTrivia)
                    {
                        if (!en.MoveNext())
                            return false;

                        if (en.Current.IsWhitespaceTrivia()
                            && !en.MoveNext())
                        {
                            return false;
                        }

                        if (en.Current.IsKind(SyntaxKind.RegionDirectiveTrivia))
                            return true;

                        if (!en.Current.IsEndOfLineTrivia())
                            return false;

                        if (!en.MoveNext())
                            return true;

                        if (en.Current.IsWhitespaceTrivia()
                            && !en.MoveNext())
                        {
                            return false;
                        }

                        return en.Current.IsEndOfLineTrivia();
                    }
                }

                return false;
            }
        }
    }
}
