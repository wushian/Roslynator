// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator
{
    public sealed class RefactoringSettings : CodeAnalysisSettings<string>
    {
        public static RefactoringSettings Current { get; } = LoadSettings();

        private static RefactoringSettings LoadSettings()
        {
            var settings = new RefactoringSettings();
#if VSCODE
            foreach (KeyValuePair<string, bool> kvp in VisualStudioCode.Configuration.Refactorings)
            {
                settings.Set(kvp.Key, kvp.Value);
            }

            settings.PrefixFieldIdentifierWithUnderscore = VisualStudioCode.Configuration.PrefixFieldIdentifierWithUnderscore;
#endif
            return settings;
        }

        public bool PrefixFieldIdentifierWithUnderscore { get; set; }

        public override void Reset()
        {
            PrefixFieldIdentifierWithUnderscore = false;
            Disabled.Clear();
        }
    }
}
