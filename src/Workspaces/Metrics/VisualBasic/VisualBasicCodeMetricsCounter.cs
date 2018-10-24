// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;

namespace Roslynator.Metrics.VisualBasic
{
    public abstract class VisualBasicCodeMetricsCounter : CodeMetricsCounter
    {
        public override bool IsComment(SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.CommentTrivia);
        }

        public override bool IsEndOfLine(SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.EndOfLineTrivia);
        }
    }
}
