// Copyright (c) 2024-2026 Pierre G. Boutquin. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License").
//   You may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//
//   See the License for the specific language governing permissions and
//   limitations under the License.
//

namespace Boutquin.Domain.Extensions;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Helpers;

/// <summary>
/// Provides extension methods for the <see cref="string"/> class.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class StringExtensions
{
    /// <summary>
    /// Determines whether the specified string is null or empty.
    /// </summary>
    /// <param name="value">The string to test. May be <see langword="null"/>.</param>
    /// <returns>
    ///   <c>true</c> if the specified string is null or empty; otherwise, <c>false</c>. When it returns
    ///   <see langword="false"/>, <paramref name="value"/> is guaranteed non-null (<c>[NotNullWhen(false)]</c>),
    ///   so the compiler flow-narrows it at the call site.
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
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
        => string.IsNullOrEmpty(value);

    /// <summary>
    /// Determines whether the specified string is null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="value">The string to test. May be <see langword="null"/>.</param>
    /// <returns>
    ///   <c>true</c> if the specified string is null, empty, or consists only of white-space characters;
    ///   otherwise, <c>false</c>. When it returns <see langword="false"/>, <paramref name="value"/> is
    ///   guaranteed non-null (<c>[NotNullWhen(false)]</c>).
    /// </returns>
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? value)
        => string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Converts the first character of the specified string to uppercase using the rules of the
    /// <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>
    /// A copy of the specified string with the first character converted to uppercase using invariant rules.
    /// </returns>
    /// <remarks>
    /// Casing is performed with <see cref="CultureInfo.InvariantCulture"/> so the result is stable across
    /// locales — avoiding surprises such as the Turkish dotted/dotless-I mapping that
    /// <see cref="CultureInfo.CurrentCulture"/> would apply. Use
    /// <see cref="ToUpperCaseFirst(string, CultureInfo)"/> when culture-specific casing is required.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static string ToUpperCaseFirst(this string value)
        => value.ToUpperCaseFirst(CultureInfo.InvariantCulture);

    /// <summary>
    /// Converts the first character of the specified string to uppercase using the rules of the
    /// specified culture.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <param name="culture">The culture whose casing rules are applied to the first character.</param>
    /// <returns>
    /// A copy of the specified string with the first character converted to uppercase using
    /// <paramref name="culture"/>'s rules.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> or
    /// <paramref name="culture"/> is null.</exception>
    public static string ToUpperCaseFirst(this string value, CultureInfo culture)
    {
        // Ensure the string value and culture are not null
        Guard.AgainstNull(() => value);
        Guard.AgainstNull(() => culture);

        if (value.Length == 0)
        {
            return value;
        }

        // Case the first Unicode scalar (Rune), not the first UTF-16 code unit, so a leading
        // supplementary character (emoji, historic script) encoded as a surrogate pair is cased as a
        // whole rather than left unchanged or split into mangled halves.
        var firstRune = value.EnumerateRunes().First();
        var cased = Rune.ToUpper(firstRune, culture);
        return string.Concat(cased.ToString(), value.AsSpan(firstRune.Utf16SequenceLength));
    }

    /// <summary>
    /// Converts the first character of the specified string to lowercase using the rules of the
    /// <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>
    /// A copy of the specified string with the first character converted to lowercase using invariant rules.
    /// </returns>
    /// <remarks>
    /// Casing is performed with <see cref="CultureInfo.InvariantCulture"/> so the result is stable across
    /// locales. Use <see cref="ToLowerCaseFirst(string, CultureInfo)"/> when culture-specific casing is
    /// required.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static string ToLowerCaseFirst(this string value)
        => value.ToLowerCaseFirst(CultureInfo.InvariantCulture);

    /// <summary>
    /// Converts the first character of the specified string to lowercase using the rules of the
    /// specified culture.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <param name="culture">The culture whose casing rules are applied to the first character.</param>
    /// <returns>
    /// A copy of the specified string with the first character converted to lowercase using
    /// <paramref name="culture"/>'s rules.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> or
    /// <paramref name="culture"/> is null.</exception>
    public static string ToLowerCaseFirst(this string value, CultureInfo culture)
    {
        // Ensure the string value and culture are not null
        Guard.AgainstNull(() => value);
        Guard.AgainstNull(() => culture);

        if (value.Length == 0)
        {
            return value;
        }

        // Case the first Unicode scalar (Rune), not the first UTF-16 code unit — see ToUpperCaseFirst.
        var firstRune = value.EnumerateRunes().First();
        var cased = Rune.ToLower(firstRune, culture);
        return string.Concat(cased.ToString(), value.AsSpan(firstRune.Utf16SequenceLength));
    }

    /// <summary>
    /// Compares two strings using the specified comparison options.
    /// </summary>
    /// <param name="value">The first string to compare. May be <see langword="null"/>, which sorts before any non-null string.</param>
    /// <param name="strB">The second string to compare. May be <see langword="null"/>.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
    /// <returns>A signed integer that indicates the relative values of value and strB.</returns>
    public static int Compare(this string? value, string? strB, StringComparison comparisonType)
        => string.Compare(value, strB, comparisonType);

    /// <summary>
    /// Compares two strings using ordinal (binary) sort rules.
    /// </summary>
    /// <param name="value">The first string to compare. May be <see langword="null"/>, which sorts before any non-null string.</param>
    /// <param name="strB">The second string to compare. May be <see langword="null"/>.</param>
    /// <returns>A signed integer that indicates the lexical relationship between the two comparands.</returns>
    public static int CompareOrdinal(this string? value, string? strB)
        => string.CompareOrdinal(value, strB);

    /// <summary>
    /// Replaces the format items in a specified string with the string representation of corresponding objects in a specified array.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
    /// <remarks>
    /// This overload formats with <see cref="CultureInfo.CurrentCulture"/>, so number and date output is
    /// locale-dependent. Use <see cref="Format(string, IFormatProvider, object[])"/> with
    /// <see cref="CultureInfo.InvariantCulture"/> for locale-stable output (e.g. machine-readable strings).
    /// </remarks>
    public static string Format(this string format, params object[] args)
        => string.Format(CultureInfo.CurrentCulture, format, args);

    /// <summary>
    /// Replaces the format items in a specified string with the string representation of corresponding
    /// objects, using the supplied format provider.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <returns>A copy of format in which the format items have been replaced by the string
    /// representation of the corresponding objects in args, formatted per <paramref name="provider"/>.</returns>
    public static string Format(this string format, IFormatProvider provider, params object[] args)
        => string.Format(provider, format, args);
}
