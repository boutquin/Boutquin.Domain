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

using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

/// <summary>
/// EnumExtensions class provides extension methods to work with enums.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class EnumExtensions
{
    // Caches the resolved description per (enum type, member name). GetField + attribute reflection is
    // expensive and the result is immutable for a given member, so it is memoized for hot-path callers
    // (e.g. Currency.GetDescription() in formatting/logging). Bounded by the number of distinct enum
    // members ever queried.
    private static readonly ConcurrentDictionary<(Type EnumType, string MemberName), string> s_descriptionCache = new();
    /// <summary>
    /// Retrieves the description of an enum value, if provided using the DescriptionAttribute.
    /// If no description is provided, the method returns the name of the enum value as a string.
    /// </summary>
    /// <typeparam name="T">The type of the enum.</typeparam>
    /// <param name="enumValue">The enum value for which the description should be retrieved.</param>
    /// <returns>
    /// The description from the DescriptionAttribute if available, or the name of the enum value as a string.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="enumValue"/> is not a single
    /// named member of its enum type — e.g. a <c>[Flags]</c> combination such as <c>A | B</c>, which has
    /// no backing field.</exception>
    /// <example>
    /// <code>
    /// public enum Colors
    /// {
    ///     [Description("Bright Red Color")]
    ///     Red,
    ///     Blue,
    ///     Green
    /// }
    ///
    /// class Program
    /// {
    ///     static void Main(string[] args)
    ///     {
    ///         string redDescription = Colors.Red.GetDescription(); // "Bright Red Color"
    ///         string blueDescription = Colors.Blue.GetDescription(); // "Blue"
    ///     }
    /// }
    /// </code>
    /// </example>
    public static string GetDescription<T>(this T enumValue) where T : Enum
    {
        // The `where T : Enum` constraint already guarantees T is an enum at compile time, so no
        // runtime non-enum guard is needed. enumValue.ToString() yields the member name for a single
        // defined value, or a comma-joined list ("A, B") for a [Flags] combination that has no field.
        var memberName = enumValue.ToString();

        // Key on enumValue.GetType() (the runtime enum type), not typeof(T): when the receiver is
        // typed as the base System.Enum (e.g. `Enum e = TestColor.Red;`), typeof(T) is System.Enum,
        // whose GetField(memberName) misses and would throw for a perfectly valid member.
        return s_descriptionCache.GetOrAdd(
            (enumValue.GetType(), memberName),
            static key =>
            {
                var (enumType, name) = key;
                var field = enumType.GetField(name)
                    ?? throw new ArgumentException(
                        $"'{name}' is not a single named member of enum '{enumType.Name}' (flag combinations are not supported).",
                        nameof(enumValue));

                var attribute = field.GetCustomAttribute<DescriptionAttribute>();
                return attribute?.Description ?? name;
            });
    }
}
