// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Roslynator.Configuration;

namespace Roslynator
{
    public sealed class RefactoringSettings : CodeAnalysisSettings<string>
    {
        public static RefactoringSettings Current { get; } = LoadSettings();

        private static RefactoringSettings LoadSettings()
        {
            var settings = new RefactoringSettings();

            settings.Reset();

            return settings;
        }

        public bool PrefixFieldIdentifierWithUnderscore { get; set; }

        public override void Reset()
        {
            Disabled.Clear();

            foreach (KeyValuePair<string, bool> kvp in CodeAnalysisConfiguration.Default.Refactorings)
            {
                Set(kvp.Key, kvp.Value);
            }

            PrefixFieldIdentifierWithUnderscore = CodeAnalysisConfiguration.Default.PrefixFieldIdentifierWithUnderscore;
        }
    }
}
