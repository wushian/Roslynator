// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Roslynator.Configuration;

namespace Roslynator.VisualStudio
{
    internal static class CodeAnalysisConfigurationHelpers
    {
        public static void Save(string path, CodeAnalysisConfiguration configuration)
        {
            var settings = new XElement("Settings",
                new XElement("General",
                    new XElement("PrefixFieldIdentifierWithUnderscore", configuration.PrefixFieldIdentifierWithUnderscore)));

            if (configuration.GetRefactorings().Any(f => !f.Value))
            {
                settings.Add(
                    new XElement("Refactorings",
                        configuration.GetRefactorings()
                            .Where(f => !f.Value)
                            .OrderBy(f => f.Key)
                            .Select(f => new XElement("Refactoring", new XAttribute("Id", f.Key), new XAttribute("IsEnabled", f.Value)))
                    ));
            }

            if (configuration.GetCodeFixes().Any(f => !f.Value))
            {
                settings.Add(
                    new XElement("CodeFixes",
                        configuration.GetCodeFixes()
                            .Where(f => !f.Value)
                            .OrderBy(f => f.Key)
                            .Select(f => new XElement("CodeFix", new XAttribute("Id", f.Key), new XAttribute("IsEnabled", f.Value)))
                    ));
            }

            var doc = new XDocument(new XElement("Roslynator", settings));

            var xmlWriterSettings = new XmlWriterSettings()
            {
                OmitXmlDeclaration = false,
                NewLineChars = Environment.NewLine,
                IndentChars = "  ",
                Indent = true,
            };

            using (var fileStream = new FileStream(path, FileMode.Create))
            using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
            using (XmlWriter xmlWriter = XmlWriter.Create(streamWriter, xmlWriterSettings))
                doc.WriteTo(xmlWriter);
        }
    }
}
