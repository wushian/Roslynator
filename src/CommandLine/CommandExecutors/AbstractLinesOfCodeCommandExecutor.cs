// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Roslynator.Metrics;
using static Roslynator.ConsoleHelpers;

namespace Roslynator.CommandLine
{
    internal abstract class AbstractLinesOfCodeCommandExecutor : MSBuildWorkspaceCommandExecutor
    {
        internal static void WriteLinesOfCode(Solution solution, ImmutableDictionary<ProjectId, CodeMetrics> projectsMetrics)
        {
            int maxDigits = projectsMetrics.Max(f => f.Value.CodeLineCount).ToString("n0").Length;
            int maxNameLength = projectsMetrics.Max(f => solution.GetProject(f.Key).Name.Length);

            foreach (KeyValuePair<ProjectId, CodeMetrics> kvp in projectsMetrics.OrderByDescending(f => f.Value.CodeLineCount))
            {
                Project project = solution.GetProject(kvp.Key);
                CodeMetrics codeMetrics = kvp.Value;

                string count = (codeMetrics.CodeLineCount >= 0)
                    ? codeMetrics.CodeLineCount.ToString("n0").PadLeft(maxDigits)
                    : "-";

                WriteLine($"{count} {project.Name.PadRight(maxNameLength)} {project.Language}");
            }
        }
    }
}
