// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class AddEmptyLineAfterClosingBraceOfBlockAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddEmptyLineAfterClosingBraceOfBlock); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeBlock, SyntaxKind.Block);
            context.RegisterSyntaxNodeAction(AnalyzeTryStatement, SyntaxKind.TryStatement);
            context.RegisterSyntaxNodeAction(AnalyzeSwitchStatement, SyntaxKind.SwitchStatement);
        }

        private static void AnalyzeBlock(SyntaxNodeAnalysisContext context)
        {
            var block = (BlockSyntax)context.Node;

            StatementSyntax blockOrStatement = block;

            switch (block.Parent.Kind())
            {
                case SyntaxKind.Block:
                    {
                        break;
                    }
                case SyntaxKind.WhileStatement:
                case SyntaxKind.ForStatement:
                case SyntaxKind.ForEachStatement:
                case SyntaxKind.ForEachVariableStatement:
                case SyntaxKind.UsingStatement:
                case SyntaxKind.FixedStatement:
                case SyntaxKind.CheckedStatement:
                case SyntaxKind.UncheckedStatement:
                case SyntaxKind.UnsafeStatement:
                case SyntaxKind.LockStatement:
                case SyntaxKind.IfStatement:
                case SyntaxKind.SwitchStatement:
                case SyntaxKind.TryStatement:
                    {
                        blockOrStatement = (StatementSyntax)block.Parent;
                        break;
                    }
                case SyntaxKind.ElseClause:
                    {
                        var elseClause = (ElseClauseSyntax)block.Parent;

                        blockOrStatement = elseClause.GetTopmostIf();
                        break;
                    }
                default:
                    {
                        return;
                    }
            }

            Analyze(context, block.CloseBraceToken, blockOrStatement);
        }

        private static void AnalyzeTryStatement(SyntaxNodeAnalysisContext context)
        {
            var tryStatement = (TryStatementSyntax)context.Node;

            BlockSyntax block = tryStatement.Finally?.Block ?? tryStatement.Catches.LastOrDefault()?.Block;

            if (block == null)
                return;

            SyntaxToken closeBrace = block.CloseBraceToken;

            if (closeBrace.IsMissing)
                return;

            Analyze(context, closeBrace, tryStatement);
        }

        private static void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            var switchStatement = (SwitchStatementSyntax)context.Node;

            SyntaxToken closeBrace = switchStatement.CloseBraceToken;

            if (closeBrace.IsMissing)
                return;

            Analyze(context, closeBrace, switchStatement);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, SyntaxToken closeBrace, StatementSyntax blockOrStatement)
        {
            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(blockOrStatement);

            if (!statementsInfo.Success)
                return;

            SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

            int index = statements.IndexOf(blockOrStatement);

            if (index < statements.Count - 1)
            {
                int endLine = closeBrace.GetSpanEndLine();

                StatementSyntax nextStatement = statements[index + 1];

                if (nextStatement.GetSpanStartLine() - endLine == 1)
                {
                    SyntaxTrivia trivia = closeBrace
                        .TrailingTrivia
                        .FirstOrDefault(f => f.IsEndOfLineTrivia());

                    if (trivia.IsEndOfLineTrivia())
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.AddEmptyLineAfterClosingBraceOfBlock,
                            Location.Create(trivia.SyntaxTree, trivia.Span.WithLength(0)));
                    }
                }
            }
        }
    }
}
