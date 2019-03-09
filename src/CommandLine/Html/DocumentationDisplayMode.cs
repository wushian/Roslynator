// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Roslynator.FindSymbols;

namespace Roslynator.Documentation.Html
{
    internal enum DocumentationDisplayMode
    {
        ToolTip,
        Xml,
    }
}
