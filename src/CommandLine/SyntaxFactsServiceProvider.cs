// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class SyntaxFactsServiceProvider
    {
        public static ISyntaxFactsService GetService(string language)
        {
            return GetServiceOrDefault(language) ?? throw new NotSupportedException($"Language '{language}' is not supported.");
        }

        public static ISyntaxFactsService GetServiceOrDefault(string language)
        {
            switch (language)
            {
                case LanguageNames.CSharp:
                    return CSharp.CSharpSyntaxFactsService.Instance;
                case LanguageNames.VisualBasic:
                    return VisualBasic.VisualBasicSyntaxFactsService.Instance;
                default:
                    return null;
            }
        }

        public static bool IsSupportedLanguage(string language)
        {
            switch (language)
            {
                case LanguageNames.CSharp:
                case LanguageNames.VisualBasic:
                    return true;
                default:
                    return false;
            }
        }
    }
}
