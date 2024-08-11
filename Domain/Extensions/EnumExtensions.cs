// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Boutquin.Domain.Extensions;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Helpers;

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
