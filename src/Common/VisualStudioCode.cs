// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Roslynator.Configuration;

#if VSCODE
namespace Roslynator
{
    internal static class VisualStudioCode
    {
        public static CodeAnalysisConfiguration Configuration { get; } = Load();

        private static CodeAnalysisConfiguration Load()
        {
            CodeAnalysisConfiguration configuration = null;

            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            if (!string.IsNullOrEmpty(path))
            {
                path = Path.Combine(path, "JosefPihrt", "Roslynator", "VisualStudioCode", CodeAnalysisConfiguration.ConfigFileName);

                if (File.Exists(path))
                {
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
            }

            return configuration ?? CodeAnalysisConfiguration.Default;
        }
    }
}
#endif
