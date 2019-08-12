// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Roslynator.CodeFixes;
using Roslynator.Configuration;
using Roslynator.VisualStudio;

namespace Roslynator
{
    internal sealed class SettingsManager
    {
        private SettingsManager()
        {
        }

        public bool UseConfigFile { get; set; }

        public static SettingsManager Instance { get; } = new SettingsManager();

        public CodeAnalysisConfiguration VisualStudioSettings { get; private set; } = new CodeAnalysisConfiguration();

        public CodeAnalysisConfiguration ConfigFileSettings { get; set; }

        public void Propagate(GeneralOptionsPage generalOptionsPage)
        {
            UseConfigFile = generalOptionsPage.UseConfigFile;

            VisualStudioSettings.PrefixFieldIdentifierWithUnderscore = generalOptionsPage.PrefixFieldIdentifierWithUnderscore;
        }

        public void Propagate(RefactoringsOptionsPage refactoringsOptionsPage)
        {
            IEnumerable<KeyValuePair<string, bool>> refactorings = refactoringsOptionsPage
                .GetDisabledItems()
                .Select(f => new KeyValuePair<string, bool>(f, false));

            VisualStudioSettings = VisualStudioSettings.WithRefactorings(refactorings);
        }

        public void Propagate(CodeFixesOptionsPage codeFixOptionsPage)
        {
            IEnumerable<KeyValuePair<string, bool>> codeFixes = codeFixOptionsPage
                .GetDisabledItems()
                .Select(f => new KeyValuePair<string, bool>(f, false));

            VisualStudioSettings = VisualStudioSettings.WithCodeFixes(codeFixes);
        }

        internal void Propagate(RefactoringSettings refactoringSettings)
        {
            refactoringSettings.Reset();

            Propagate(VisualStudioSettings);

            if (UseConfigFile)
                Propagate(ConfigFileSettings);

            void Propagate(CodeAnalysisConfiguration settings)
            {
                if (settings != null)
                {
                    refactoringSettings.PrefixFieldIdentifierWithUnderscore = settings.PrefixFieldIdentifierWithUnderscore;
                    refactoringSettings.Set(settings.GetRefactorings());
                }
            }
        }

        public void Propagate(CodeFixSettings codeFixSettings)
        {
            codeFixSettings.Reset();

            Propagate(VisualStudioSettings);

            if (UseConfigFile)
                Propagate(ConfigFileSettings);

            void Propagate(CodeAnalysisConfiguration settings)
            {
                if (settings != null)
                {
                    foreach (KeyValuePair<string, bool> kvp in settings.GetCodeFixes())
                    {
                        if (CodeFixIdentifier.TryParse(kvp.Key, out CodeFixIdentifier codeFixIdentifier))
                        {
                            codeFixSettings.Set(codeFixIdentifier, kvp.Value);
                        }
                    }
                }
            }
        }
    }
}
