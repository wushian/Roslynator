// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class PathUtilities
    {
        internal static string MakeRelativePath(Document document, Project project, bool trimLeadingDirectorySeparator = true)
        {
            return MakeRelativePath(document.FilePath, Path.GetDirectoryName(project.FilePath), trimLeadingDirectorySeparator: trimLeadingDirectorySeparator);
        }

        internal static string MakeRelativePath(string path, string basePath, bool trimLeadingDirectorySeparator = true)
        {
            if (basePath != null
                && path.StartsWith(basePath))
            {
                int length = basePath.Length;

                if (trimLeadingDirectorySeparator)
                {
                    while (length < path.Length
                        && path[length] == Path.DirectorySeparatorChar)
                    {
                        length++;
                    }

                    return path.Remove(0, length);
                }
            }

            return path;
        }
    }
}
