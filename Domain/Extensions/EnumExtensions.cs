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

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Boutquin.Domain.Helpers;

namespace Boutquin.Domain.Extensions;

/// <summary>
/// EnumExtensions class provides extension methods to work with enums.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class EnumExtensions
{
    /// <summary>
    /// Retrieves the description of an enum value, if provided using the DescriptionAttribute.
    /// If no description is provided, the method returns the name of the enum value as a string.
    /// </summary>
    /// <typeparam name="T">The type of the enum.</typeparam>
    /// <param name="enumValue">The enum value for which the description should be retrieved.</param>
    /// <returns>
    /// The description from the DescriptionAttribute if available, or the name of the enum value as a string.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when the provided type is not an enum.</exception>
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
        // Check if the provided type is an enum.
        Guard.AgainstNonEnumType<T>();

        // Get the field information for the given enum value.
        var field = enumValue.GetType().GetField(enumValue.ToString()) ?? throw new ArgumentException("The provided enum value is not a valid member of the enum type.");

        // Get the DescriptionAttribute if it exists on the field.
        var attribute = field.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;

        // Return the description if available, or the name of the enum value as a string.
        return attribute?.Description ?? enumValue.ToString();
    }
}
