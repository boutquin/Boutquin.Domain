// Copyright (c) 2023 Pierre G. Boutquin. All rights reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License").
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Boutquin.Domain.Helpers;

namespace Boutquin.Domain.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="string"/> class.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class StringExtensions
{
    /// <summary>
    /// Determines whether the specified string is null or empty.
    /// </summary>
    /// <param name="value">The string to test.</param>
    /// <returns>
    ///   <c>true</c> if the specified string is null or empty; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method is an extension method for the <see cref="string"/> class, allowing you to call the method directly on a string variable.
    /// It internally calls the <see cref="string.IsNullOrEmpty(string)"/> method to determine if the string is null or empty.
    /// </remarks>
    /// <example>
    /// This example demonstrates how to use the <see cref="IsNullOrEmpty"/> extension method.
    /// <code>
    /// string testString = null;
    /// bool result = testString.IsNullOrEmpty(); // result will be true
    ///
    /// testString = "";
    /// result = testString.IsNullOrEmpty(); // result will be true
    ///
    /// testString = "Hello, World!";
    /// result = testString.IsNullOrEmpty(); // result will be false
    /// </code>
    /// </example>
    public static bool IsNullOrEmpty(this string value) 
        => string.IsNullOrEmpty(value);

    /// <summary>
    /// Determines whether the specified string is null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="value">The string to test.</param>
    /// <returns>
    ///   <c>true</c> if the specified string is null, empty, or consists only of white-space characters; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrWhiteSpace(this string value) 
        => string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Converts the first character of the specified string to uppercase using the rules of the current culture.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>
    /// A copy of the specified string with the first character converted to uppercase using the rules of the current culture.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the input object is not a string.</exception>
    public static string ToUppercaseFirst(this string value)
    {
        // Ensure the string value is not null
        Guard.AgainstNull(() => value);

        if (value.Length == 0)
        {
            return value;
        }

        return char.ToUpper(value[0], CultureInfo.CurrentCulture) + value.Substring(1);
    }

    /// <summary>
    /// Converts the first character of the specified string to lowercase using the rules of the current culture.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>
    /// A copy of the specified string with the first character converted to lowercase using the rules of the current culture.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the input object is not a string.</exception>
    public static string ToLowerCaseFirst(this string value)
    {
        // Ensure the string value is not null
        Guard.AgainstNull(() => value);

        if (value.Length == 0)
        {
            return value;
        }

        return char.ToLower(value[0], CultureInfo.CurrentCulture) + value.Substring(1);
    }

    /// <summary>
    /// Compares two strings using the specified comparison options.
    /// </summary>
    /// <param name="value">The first string to compare.</param>
    /// <param name="strB">The second string to compare.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
    /// <returns>A signed integer that indicates the relative values of value and strB.</returns>
    public static int Compare(this string value, string strB, StringComparison comparisonType)
    {
        return string.Compare(value, strB, comparisonType);
    }

    /// <summary>
    /// Compares two strings using ordinal (binary) sort rules.
    /// </summary>
    /// <param name="value">The first string to compare.</param>
    /// <param name="strB">The second string to compare.</param>
    /// <returns>A signed integer that indicates the lexical relationship between the two comparands.</returns>
    public static int CompareOrdinal(this string value, string strB)
    {
        return string.CompareOrdinal(value, strB);
    }

    /// <summary>
    /// Replaces the format items in a specified string with the string representation of corresponding objects in a specified array.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
    public static string Format(this string format, params object[] args)
    {
        return string.Format(format, args);
    }
}
