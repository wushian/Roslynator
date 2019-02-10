// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal static class ParseHelpers
    {
        private static readonly Regex _lowerLetterUpperLetterRegex = new Regex(@"\p{Ll}\p{Lu}");

        public static bool TryParseParameterValueAsEnumFlags<TEnum>(
            IEnumerable<string> values,
            string parameterName,
            out TEnum result,
            TEnum? defaultValue = null) where TEnum : struct
        {
            result = (TEnum)(object)0;

            if (values?.Any() != true)
            {
                if (defaultValue != null)
                {
                    result = (TEnum)(object)defaultValue;
                }

                return true;
            }

            int flags = 0;

            foreach (string value in values)
            {
                if (!TryParseParameterValueAsEnum(value, parameterName, out TEnum result2))
                    return false;

                flags |= (int)(object)result2;
            }

            result = (TEnum)(object)flags;

            return true;
        }

        public static bool TryParseParameterValueAsEnum<TEnum>(string value, string parameterName, out TEnum result, TEnum? defaultValue = null) where TEnum : struct
        {
            if (value == null
                && defaultValue != null)
            {
                result = defaultValue.Value;
                return true;
            }

            if (!Enum.TryParse(value?.Replace("-", ""), ignoreCase: true, out result))
            {
                IEnumerable<string> values = Enum.GetValues(typeof(TEnum))
                    .Cast<TEnum>()
                    .Select(f => _lowerLetterUpperLetterRegex.Replace(f.ToString(), e => e.Value.Insert(1, "-")).ToLowerInvariant());

                WriteLine($"Parameter '{parameterName}' has unknown value '{value}'. Known values: {string.Join(", ", values)}.", Verbosity.Quiet);
                return false;
            }

            return true;
        }

        public static bool TryParseVerbosity(string value, out Verbosity verbosity)
        {
            switch (value)
            {
                case "q":
                    {
                        verbosity = Verbosity.Quiet;
                        return true;
                    }
                case "m":
                    {
                        verbosity = Verbosity.Minimal;
                        return true;
                    }
                case "n":
                    {
                        verbosity = Verbosity.Normal;
                        return true;
                    }
                case "d":
                    {
                        verbosity = Verbosity.Detailed;
                        return true;
                    }
                case "diag":
                    {
                        verbosity = Verbosity.Diagnostic;
                        return true;
                    }
            }

            if (Enum.TryParse(value, ignoreCase: true, out verbosity))
                return true;

            WriteLine($"Unknown verbosity '{value}'.", Verbosity.Quiet);
            return false;
        }
    }
}
