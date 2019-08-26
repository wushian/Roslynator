// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class FormatNodesInListAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.FormatNodesInList); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeTypeArgumentList, SyntaxKind.TypeArgumentList);
            context.RegisterSyntaxNodeAction(AnalyzeBaseArgumentList, SyntaxKind.ArgumentList);
            context.RegisterSyntaxNodeAction(AnalyzeBaseArgumentList, SyntaxKind.BracketedArgumentList);
            context.RegisterSyntaxNodeAction(AnalyzeAttributeList, SyntaxKind.AttributeList);
            context.RegisterSyntaxNodeAction(AnalyzeAttributeArgumentList, SyntaxKind.AttributeArgumentList);
            context.RegisterSyntaxNodeAction(AnalyzeBaseList, SyntaxKind.BaseList);
            context.RegisterSyntaxNodeAction(AnalyzeBaseParameterList, SyntaxKind.ParameterList);
            context.RegisterSyntaxNodeAction(AnalyzeBaseParameterList, SyntaxKind.BracketedParameterList);
            context.RegisterSyntaxNodeAction(AnalyzeTypeParameterList, SyntaxKind.TypeParameterList);
        }

        private static void AnalyzeTypeArgumentList(SyntaxNodeAnalysisContext context)
        {
            var typeArgumentList = (TypeArgumentListSyntax)context.Node;

            Analyze(context, typeArgumentList.Arguments);
        }

        private static void AnalyzeBaseArgumentList(SyntaxNodeAnalysisContext context)
        {
            var argumentList = (BaseArgumentListSyntax)context.Node;

            Analyze(context, argumentList.Arguments);
        }

        private static void AnalyzeAttributeList(SyntaxNodeAnalysisContext context)
        {
            var attributeList = (AttributeListSyntax)context.Node;

            Analyze(context, attributeList.Attributes);
        }

        private static void AnalyzeAttributeArgumentList(SyntaxNodeAnalysisContext context)
        {
            var attributeArgumentList = (AttributeArgumentListSyntax)context.Node;

            Analyze(context, attributeArgumentList.Arguments);
        }

        private static void AnalyzeBaseList(SyntaxNodeAnalysisContext context)
        {
            var baseList = (BaseListSyntax)context.Node;

            Analyze(context, baseList.Types);
        }

        private static void AnalyzeBaseParameterList(SyntaxNodeAnalysisContext context)
        {
            var parameterList = (BaseParameterListSyntax)context.Node;

            Analyze(context, parameterList.Parameters);
        }

        private static void AnalyzeTypeParameterList(SyntaxNodeAnalysisContext context)
        {
            var typeParameterList = (TypeParameterListSyntax)context.Node;

            Analyze(context, typeParameterList.Parameters);
        }

        private static void Analyze<TNode>(SyntaxNodeAnalysisContext context, SeparatedSyntaxList<TNode> nodes) where TNode : SyntaxNode
        {
            int count = nodes.Count;

            if (count <= 1)
                return;

            SyntaxTree syntaxTree = nodes[0].SyntaxTree;

            bool isSingleLine = true;

            for (int i = 1; i < count; i++)
            {
                TNode node1 = nodes[i - 1];
                TNode node2 = nodes[i];

                bool isSingleLine1 = IsSingleLine(node1.Span);
                bool isSingleLine2 = IsSingleLine(node2.Span);
                bool isSingleLineBetween = IsSingleLine(TextSpan.FromBounds(node1.Span.End, node2.SpanStart));

                if (isSingleLine1)
                {
                    if (isSingleLine2)
                    {
                        if (i == 1)
                        {
                            isSingleLine = isSingleLineBetween;
                        }
                        else if (isSingleLineBetween != isSingleLine)
                        {
                            ReportDiagnostic();
                            return;
                        }
                    }
                    else if (isSingleLineBetween)
                    {
                        ReportDiagnostic();
                        return;
                    }
                    else if (isSingleLineBetween
                        || isSingleLine)
                    {
                        ReportDiagnostic();
                        return;
                    }
                }
                else if (isSingleLine2)
                {
                    if (isSingleLineBetween)
                    {
                        ReportDiagnostic();
                        return;
                    }
                    else if (isSingleLineBetween
                        || isSingleLine)
                    {
                        ReportDiagnostic();
                        return;
                    }
                }
                else if (isSingleLineBetween)
                {
                    ReportDiagnostic();
                    return;
                }
                else
                {
                    isSingleLine = false;
                }
            }

            bool IsSingleLine(TextSpan span)
            {
                return syntaxTree.GetLineSpan(span, context.CancellationToken).IsSingleLine();
            }

            void ReportDiagnostic()
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.FormatNodesInList,
                    Location.Create(syntaxTree, TextSpan.FromBounds(nodes[0].SpanStart, nodes.Last().Span.End)));
            }
        }
    }
}
