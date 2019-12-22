// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.Tests;

namespace Roslynator.CSharp.Tests
{
    public class XunitAssert : Assert
    {
        public static XunitAssert Instance { get; } = new XunitAssert();

        public override void Equal(string expected, string actual)
        {
            Xunit.Assert.Equal(expected, actual);
        }

        public override void True(bool condition, string userMessage)
        {
            Xunit.Assert.True(condition, userMessage);
        }
    }
}
