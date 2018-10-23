// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;

namespace Roslynator.Analysis
{
    public static class AssemblyAnalyzer
    {
        public static IEnumerable<AnalyzerAssembly> Analyze(
            string path,
            bool loadAnalyzers = true,
            bool loadFixers = true,
            string language = null)
        {
            if (File.Exists(path))
            {
                AnalyzerAssembly analyzerAssembly = Load(path);

                if (analyzerAssembly?.IsEmpty == false)
                    yield return analyzerAssembly;
            }
            else if (Directory.Exists(path))
            {
                using (IEnumerator<string> en = Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories).GetEnumerator())
                {
                    while (true)
                    {
                        AnalyzerAssembly analyzerAssembly = null;

                        try
                        {
                            if (en.MoveNext())
                            {
                                analyzerAssembly = Load(en.Current);
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch (IOException)
                        {
                            continue;
                        }
                        catch (SecurityException)
                        {
                            continue;
                        }
                        catch (UnauthorizedAccessException)
                        {
                            continue;
                        }

                        if (analyzerAssembly?.IsEmpty == false)
                            yield return analyzerAssembly;
                    }
                }
            }

            AnalyzerAssembly Load(string filePath)
            {
                Assembly assembly = null;

                try
                {
                    assembly = Assembly.LoadFrom(filePath);
                }
                catch (Exception ex)
                {
                    if (ex is FileLoadException
                        || ex is BadImageFormatException
                        || ex is SecurityException)
                    {
                        ConsoleHelpers.WriteLine($"Cannot load assembly '{filePath}'", ConsoleColor.DarkGray);

                        return null;
                    }
                    else
                    {
                        throw;
                    }
                }

                return AnalyzerAssembly.Load(assembly, loadAnalyzers: loadAnalyzers, loadFixers: loadFixers, language: language);
            }
        }
    }
}
