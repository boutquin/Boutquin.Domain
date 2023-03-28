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

namespace Boutquin.Domain.Helpers;

/// <summary>
/// The Guard class is a utility class that provides static methods to validate the preconditions for parameters
/// in a method. It helps to ensure that input values are valid and appropriate for the given context.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Checks if the given object is null and throws an ArgumentNullException if it is.
    /// </summary>
    /// <typeparam name="T">The type of the object being checked.</typeparam>
    /// <param name="value">The object to check for null.</param>
    /// <param name="paramName">The name of the parameter that will be used in the exception message.</param>
    /// <exception cref="ArgumentNullException">Thrown when the given object is null.</exception>
    /// <example>
    /// <code>
    /// public void PrintList(List<string> items)
    /// {
    ///     Guard.AgainstNull(items, nameof(items));
    ///     foreach (var item in items)
    ///     {
    ///         Console.WriteLine(item);
    ///     }
    /// }
    /// </code>
    /// </example>
    public static void AgainstNull<T>(T value, string paramName) where T : class
    {
        if (value == null)
        {
            throw new ArgumentNullException(paramName, $"Parameter '{paramName}' cannot be null.");
        }
    }

    /// <summary>
    /// Checks if the given string is null or empty and throws an ArgumentException if it is.
    /// </summary>
    /// <param name="value">The string to check for null or empty.</param>
    /// <param name="paramName">The name of the parameter that will be used in the exception message.</param>
    /// <exception cref="ArgumentException">Thrown when the given string is null or empty.</exception>
    /// <example>
    /// <code>
    /// public void PrintGreeting(string name)
    /// {
    ///     Guard.AgainstNullOrEmpty(name, nameof(name));
    ///     Console.WriteLine($"Hello, {name}!");
    /// }
    /// </code>
    /// </example>
    public static void AgainstNullOrEmpty(string value, string paramName)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException($"Parameter '{paramName}' cannot be null or empty.", paramName);
        }
    }

    /// <summary>
    /// Checks if the given string is null, empty, or consists only of white-space characters
    /// and throws an ArgumentException if it is.
    /// </summary>
    /// <param name="value">The string to check for null, empty or white-space characters.</param>
    /// <param name="paramName">The name of the parameter that will be used in the exception message.</param>
    /// <exception cref="ArgumentException">Thrown when the given string is null, empty or consists only of white-space characters.</exception>
    /// <example>
    /// <code>
    /// public void PrintGreeting(string name)
    /// {
    ///     Guard.AgainstNullOrWhiteSpace(name, nameof(name));
    ///     Console.WriteLine($"Hello, {name}!");
    /// }
    ///
    /// class Program
    /// {
    ///     static void Main(string[] args)
    ///     {
    ///         PrintGreeting("John"); // Works fine
    ///         PrintGreeting(null); // Throws ArgumentException: "Parameter 'name' cannot be null, empty or contain only white-space characters."
    ///         PrintGreeting(""); // Throws ArgumentException: "Parameter 'name' cannot be null, empty or contain only white-space characters."
    ///         PrintGreeting("   "); // Throws ArgumentException: "Parameter 'name' cannot be null, empty or contain only white-space characters."
    ///     }
    /// }
    /// </code>
    /// </example>
    public static void AgainstNullOrWhiteSpace(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"Parameter '{paramName}' cannot be null, empty or contain only white-space characters.", paramName);
        }
    }

    /// <summary>
    /// Checks if the given string is null, empty or exceeds the specified maximum length and throws an ArgumentException if it does.
    /// </summary>
    /// <param name="value">The string to check for null, empty, or exceeding the maximum length.</param>
    /// <param name="paramName">The name of the parameter that will be used in the exception message.</param>
    /// <param name="maxLength">The maximum length allowed for the string.</param>
    /// <exception cref="ArgumentException">Thrown when the given string is null, empty, or exceeds the maximum length.</exception>
    /// <example>
    /// <code>
    /// public void PrintGreeting(string name, int maxLength)
    /// {
    ///     Guard.AgainstNullOrEmpty(name, nameof(name), maxLength);
    ///     Console.WriteLine($"Hello, {name}!");
    /// }
    /// </code>
    /// </example>
    public static void AgainstNullOrEmpty(string value, string paramName, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException($"Parameter '{paramName}' cannot be null or empty.", paramName);
        }
            
        if (value.Length > maxLength)
        {
            throw new ArgumentException($"Parameter '{paramName}' exceeds the maximum length of {maxLength}.", paramName);
        }
    }

    /// <summary>
    /// Checks if the given string is null, empty, consists only of white-space characters, or exceeds the specified maximum length
    /// and throws an ArgumentException if it does.
    /// </summary>
    /// <param name="value">The string to check for null, empty, white-space characters, or exceeding the maximum length.</param>
    /// <param name="paramName">The name of the parameter that will be used in the exception message.</param>
    /// <param name="maxLength">The maximum length allowed for the string.</param>
    /// <exception cref="ArgumentException">Thrown when the given string is null, empty, consists only of white-space characters, or exceeds the maximum length.</exception>
    /// <example>
    /// <code>
    /// public void PrintGreeting(string name, int maxLength)
    /// {
    ///     Guard.AgainstNullOrWhiteSpace(name, nameof(name), maxLength);
    ///     Console.WriteLine($"Hello, {name}!");
    /// }
    ///
    /// class Program
    /// {
    ///     static void Main(string[] args)
    ///     {
    ///         PrintGreeting("John", 10); // Works fine
    ///         PrintGreeting(null, 10); // Throws ArgumentException: "Parameter 'name' cannot be null, empty or contain only white-space characters."
    ///         PrintGreeting("", 10); // Throws ArgumentException: "Parameter 'name' cannot be null, empty or contain only white-space characters."
    ///         PrintGreeting("   ", 10); // Throws ArgumentException: "Parameter 'name' cannot be null, empty or contain only white-space characters."
    ///         PrintGreeting("John Doe", 5); // Throws ArgumentException: "Parameter 'name' exceeds the maximum length of 5."
    ///     }
    /// }
    /// </code>
    /// </example>
    public static void AgainstNullOrWhiteSpace(string value, string paramName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"Parameter '{paramName}' cannot be null, empty or contain only white-space characters.", paramName);
        }

        if (value.Length > maxLength)
        {
            throw new ArgumentException($"Parameter '{paramName}' exceeds the maximum length of {maxLength}.", paramName);
        }
    }
}
