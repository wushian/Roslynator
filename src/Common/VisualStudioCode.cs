// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Roslynator.Configuration;

#if VSCODE
#endif

namespace Roslynator
{
    internal static class VisualStudioCode
    {
        public static CodeAnalysisConfiguration Configuration { get; } = Load();

        private static CodeAnalysisConfiguration Load()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            CodeAnalysisConfiguration configuration = null;

            if (!string.IsNullOrEmpty(path))
            {
                path = Path.Combine(path, "JosefPihrt", "Roslynator", "VisualStudioCode", CodeAnalysisConfiguration.ConfigFileName);

                try
                {
                    configuration = CodeAnalysisConfiguration.Load(path);
                }
                catch (Exception ex) when (ex is IOException
                        || ex is UnauthorizedAccessException
                        || ex is XmlException)
                {
                    Debug.Fail(ex.ToString());
                }
            }

            return configuration ?? CodeAnalysisConfiguration.Default;
        }

        public static void CopyConfiguration(CodeAnalysisConfiguration configuration, RefactoringSettings settings)
        {
            foreach (KeyValuePair<string, bool> kvp in configuration.Refactorings)
            {
                settings.Set(kvp.Key, kvp.Value);
            }

            settings.PrefixFieldIdentifierWithUnderscore = configuration.PrefixFieldIdentifierWithUnderscore;
        }

        public static void CopyConfiguration(CodeAnalysisConfiguration configuration, CodeFixSettings settings)
        {
            foreach (KeyValuePair<string, bool> kvp in configuration.CodeFixes)
            {
                if (CodeFixIdentifier.TryParse(kvp.Key, out CodeFixIdentifier codeFixIdentifier))
                {
                    settings.Set(codeFixIdentifier, kvp.Value);
                }
                else
                {
                    Debug.Fail($"Unable to parse '{kvp.Key}'");
                }
            }
        }
    }
}
