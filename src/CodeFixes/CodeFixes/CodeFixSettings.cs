// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Roslynator.Configuration;

namespace Roslynator.CodeFixes
{
    public sealed class CodeFixSettings : CodeAnalysisSettings<CodeFixIdentifier>
    {
        public static CodeFixSettings Current { get; } = Load();

        private static CodeFixSettings Load()
        {
            var settings = new CodeFixSettings();

            foreach (KeyValuePair<string, bool> kvp in CodeAnalysisConfiguration.Default.CodeFixes)
            {
                string id = kvp.Key;
                bool isEnabled = kvp.Value;

                if (CodeFixIdentifier.TryParse(id, out CodeFixIdentifier codeFixIdentifier))
                {
                    settings.Set(codeFixIdentifier, isEnabled);
                }
                else if (id.StartsWith(CodeFixIdentifier.CodeFixIdPrefix, StringComparison.Ordinal))
                {
                    foreach (string compilerDiagnosticId in CodeFixMap.GetCompilerDiagnosticIds(id))
                    {
                        settings.Set(compilerDiagnosticId, id, isEnabled);
                    }
                }
                else if (id.StartsWith("CS", StringComparison.Ordinal))
                {
                    foreach (string codeFixId in CodeFixMap.GetCodeFixIds(id))
                    {
                        settings.Set(id, codeFixId, isEnabled);
                    }
                }
                else
                {
                    Debug.Fail(id);
                }
            }

            return settings;
        }

        private void Set(string compilerDiagnosticId, string codeFixId, bool isEnabled)
        {
            Set(new CodeFixIdentifier(compilerDiagnosticId, codeFixId), isEnabled);
        }

        public bool IsEnabled(string compilerDiagnosticId, string codeFixId)
        {
            return IsEnabled(new CodeFixIdentifier(compilerDiagnosticId, codeFixId));
        }
    }
}
