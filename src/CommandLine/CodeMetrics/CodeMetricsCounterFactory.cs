// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator.CodeMetrics
{
    internal static class CodeMetricsCounterFactory
    {
        public static CodeMetricsCounter GetPhysicalLinesCounter(string language)
        {
            switch (language)
            {
                case LanguageNames.CSharp:
                    return CSharp.CSharpPhysicalLinesCounter.Instance;
                case LanguageNames.VisualBasic:
                    return VisualBasic.VisualBasicPhysicalLinesCounter.Instance;
            }

            Debug.Assert(language == LanguageNames.FSharp, language);

            return null;
        }

        public static CodeMetricsCounter GetLogicalLinesCounter(string language)
        {
            switch (language)
            {
                case LanguageNames.CSharp:
                    return CSharp.CSharpLogicalLinesCounter.Instance;
                case LanguageNames.VisualBasic:
                    return VisualBasic.VisualBasicLogicalLinesCounter.Instance;
            }

            Debug.Assert(language == LanguageNames.FSharp, language);

            return null;
        }
    }
}
