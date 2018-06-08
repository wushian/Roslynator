// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.ExtractLinqToLocalFunction
{
    internal static class ExtractLinqToLocalFunctionRefactorings
    {
        public static ExtractAnyToLocalFunctionRefactoring ExtractAny { get; } = new ExtractAnyToLocalFunctionRefactoring();

        public static ExtractAllToLocalFunctionRefactoring ExtractAll { get; } = new ExtractAllToLocalFunctionRefactoring();

        public static ExtractFirstOrDefaultToLocalFunctionRefactoring ExtractFirstOrDefault { get; } = new ExtractFirstOrDefaultToLocalFunctionRefactoring();
    }
}
