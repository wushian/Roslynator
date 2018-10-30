// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Diagnostics;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal static class DiagnosticXmlSerializer
    {
        public static void Serialize(
            ProjectAnalysisResult result,
            Project project,
            string filePath,
            IFormatProvider formatProvider = null)
        {
            var document = new XDocument(
                new XElement(
                    "Projects",
                    Serialize(result, project, formatProvider)));

            Serialize(document, filePath);
        }

        public static void Serialize(
            ImmutableArray<ProjectAnalysisResult> results,
            Solution solution,
            string filePath,
            IFormatProvider formatProvider = null)
        {
            var document = new XDocument(
                new XElement(
                    "Projects",
                    results
                        .Where(f => f.Diagnostics.Any())
                        .Select(result => Serialize(result, solution.GetProject(result.ProjectId), formatProvider))));

            Serialize(document, filePath);
        }

        private static void Serialize(XDocument document, string filePath)
        {
            WriteLine($"Save diagnostics to '{filePath}'", Verbosity.Detailed);

            using (var fs = new FileStream(filePath, FileMode.Create))
            using (XmlWriter xw = XmlWriter.Create(fs, new XmlWriterSettings() { Indent = true }))
                document.Save(xw);
        }

        private static XElement Serialize(
            ProjectAnalysisResult result,
            Project project,
            IFormatProvider formatProvider)
        {
            return new XElement(
                "Project",
                new XAttribute("Name", project.Name),
                new XElement("FilePath", project.FilePath),
                new XElement("Diagnostics",
                    result.Diagnostics
                        .OrderBy(f => f.Id)
                        .ThenBy(f => f.Location.SourceTree.FilePath)
                        .ThenBy(f => f.Location.SourceSpan.Start)
                        .Select(f =>
                        {
                            XElement filePathElement = null;
                            XElement locationElement = null;

                            FileLinePositionSpan span = f.Location.GetMappedLineSpan();

                            if (span.IsValid)
                            {
                                filePathElement = new XElement("FilePath", span.Path);

                                LinePosition linePosition = span.Span.Start;

                                locationElement = new XElement("Location",
                                    new XAttribute("Line", linePosition.Line + 1),
                                    new XAttribute("Character", linePosition.Character + 1));
                            }

                            return new XElement(
                                "Diagnostic",
                                new XElement("Id", f.Id),
                                new XElement("Severity", f.Severity),
                                new XElement("Message", f.GetMessage(formatProvider)),
                                filePathElement,
                                locationElement);
                        })));
        }
    }
}
