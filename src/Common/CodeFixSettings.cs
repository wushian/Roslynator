// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator
{
    public sealed class CodeFixSettings : CodeAnalysisSettings<CodeFixIdentifier>
    {
        public static CodeFixSettings Current { get; } = Load();

        private static CodeFixSettings Load()
        {
            var settings = new CodeFixSettings();

            //TODO: vscode
#if VSCODE
#endif
            VisualStudioCode.CopyConfiguration(VisualStudioCode.Configuration, settings);

            return settings;
        }

        public bool IsEnabled(string compilerDiagnosticId, string codeFixId)
        {
            return IsEnabled(new CodeFixIdentifier(compilerDiagnosticId, codeFixId));
        }
    }
}
