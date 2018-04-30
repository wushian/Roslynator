// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Tests
{
    public readonly struct LinePositionInfo
    {
        public LinePositionInfo(int index, int rowIndex, int columnIndex)
        {
            Index = index;
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
        }

        public int Index { get; }

        public int RowIndex { get; }

        public int ColumnIndex { get; }

        public LinePosition LinePosition
        {
            get { return new LinePosition(RowIndex, ColumnIndex); }
        }
    }
}
