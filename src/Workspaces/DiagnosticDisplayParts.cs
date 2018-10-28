// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator
{
    [Flags]
    internal enum DiagnosticDisplayParts
    {

        /// <summary>
        /// 0
        /// </summary>
        None = 0,
        /// <summary>
        /// 1
        /// </summary>
        Severity = 1,
        /// <summary>
        /// 2
        /// </summary>
        Id = 2,
        /// <summary>
        /// 4
        /// </summary>
        Message = 4,
        /// <summary>
        /// 8
        /// </summary>
        Path = 8,
        /// <summary>
        /// 16
        /// </summary>
        Location = 16,
        /// <summary>
        /// 
        /// </summary>
        PathAndLocation = Path | Location,
        /// <summary>
        /// 
        /// </summary>
        All = Severity | Id | Message | Path | Location
    }
}
