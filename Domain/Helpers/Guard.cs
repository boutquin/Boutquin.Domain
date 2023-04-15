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
    /// Checks if the given condition is true and throws the specified exception if it is.
    /// </summary>
    /// <example>
    /// <code>
    /// public void SetQuantity(int quantity)
    /// {
    ///     Guard.Against(quantity <= 0).With<ArgumentException>();
    ///     Console.WriteLine($"Quantity is set to: {quantity}");
    /// }
    /// </code>
    /// </example>
    /// <param name="condition">The condition to check.</param>
    /// <returns>An instance of GuardCondition to chain with With&lt;TException&gt; method.</returns>
    public static GuardCondition Against(bool condition)
    {
        return new GuardCondition(condition);
    }

    /// <summary>
    /// Checks if the given reference type value is null and throws an ArgumentNullException if it is.
    /// This overload accepts a Func&lt;T&gt; that returns the value and extracts its name using nameof.
    /// </summary>
    /// <example>
    /// <code>
    /// public void PrintList(List<string> items)
    /// {
    ///     Guard.AgainstNull(() => items);
    ///     foreach (var item in items)
    ///     {
    ///         Console.WriteLine(item);
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <typeparam name="T">The type of the value being checked. Must be a reference type.</typeparam>
    /// <param name="valueExpression">A Func&lt;T&gt; that returns the value to check for null.</param>
    /// <exception cref="ArgumentNullException">Thrown when the given value is null.</exception>
    public static void AgainstNull<T>(Func<T> valueExpression) where T : class
    {
        var value = valueExpression();
        var paramName = GetParameterName(valueExpression);
        AgainstNull(value, paramName);
    }

    /// <summary>
    /// Checks if the given given reference type value is null and throws an ArgumentNullException if it is.
    /// </summary>
    /// <typeparam name="T">The type of the value being checked. Must be a reference type.</typeparam>
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
    /// Checks if the given string value is null or empty and throws an ArgumentNullException if it is.
    /// This overload accepts a Func&lt;string&gt; that returns the value and extracts its name using nameof.
    /// </summary>
    /// <example>
    /// <code>
    /// public void PrintMessage(string message)
    /// {
    ///     Guard.AgainstNullOrEmpty(() => message);
    ///     Console.WriteLine(message);
    /// }
    /// </code>
    /// </example>
    /// <param name="valueExpression">A Func&lt;string&gt; that returns the value to check for null or empty.</param>
    /// <exception cref="ArgumentNullException">Thrown when the given string value is null or empty.</exception>
    public static void AgainstNullOrEmpty(Func<string> valueExpression)
    {
        var value = valueExpression();
        var paramName = GetParameterName(valueExpression);
        AgainstNullOrEmpty(value, paramName);
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
    /// Checks if the given string value is null, empty, or contains only whitespace characters and throws an ArgumentNullException if it is.
    /// This overload accepts a Func&lt;string&gt; that returns the value and extracts its name using nameof.
    /// </summary>
    /// <example>
    /// <code>
    /// public void PrintMessage(string message)
    /// {
    ///     Guard.AgainstNullOrWhiteSpace(() => message);
    ///     Console.WriteLine(message);
    /// }
    /// </code>
    /// </example>
    /// <param name="valueExpression">A Func&lt;string&gt; that returns the value to check for null, empty, or whitespace.</param>
    /// <exception cref="ArgumentNullException">Thrown when the given string value is null, empty, or contains only whitespace characters.</exception>
    public static void AgainstNullOrWhiteSpace(Func<string> valueExpression)
    {
        var value = valueExpression();
        var paramName = GetParameterName(valueExpression);
        AgainstNullOrWhiteSpace(value, paramName);
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
    /// Checks if the given string exceeds the specified maximum length
    /// and throws an ArgumentException if it does.
    /// </summary>
    /// <param name="value">The string to check for exceeding the maximum length.</param>
    /// <param name="paramName">The name of the parameter that will be used in the exception message.</param>
    /// <param name="maxLength">The maximum length allowed for the string.</param>
    /// <exception cref="ArgumentException">Thrown when the given string exceeds the maximum length.</exception>
    /// <example>
    /// <code>
    /// public void PrintGreeting(string name, int maxLength)
    /// {
    ///     Guard.AgainstOverflow(name, nameof(name), maxLength);
    ///     Console.WriteLine($"Hello, {name}!");
    /// }
    ///
    /// class Program
    /// {
    ///     static void Main(string[] args)
    ///     {
    ///         PrintGreeting("John", 10); // Works fine
    ///         PrintGreeting("John Doe", 5); // Throws ArgumentException: "Parameter 'name' exceeds the maximum length of 5."
    ///     }
    /// }
    /// </code>
    /// </example>
    public static void AgainstOverflow(string value, string paramName, int maxLength)
    {
        if (value.Length > maxLength)
        {
            throw new ArgumentException($"Parameter '{paramName}' exceeds the maximum length of {maxLength}.", paramName);
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
    ///     Guard.AgainstNullOrEmptyAndOverflow(name, nameof(name), maxLength);
    ///     Console.WriteLine($"Hello, {name}!");
    /// }
    /// </code>
    /// </example>
    public static void AgainstNullOrEmptyAndOverflow(string value, string paramName, int maxLength)
    {
        AgainstNullOrEmpty(value, paramName);            
        AgainstOverflow(value, paramName, maxLength);
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
    ///     Guard.AgainstNullOrWhiteSpaceAndOverflow(name, nameof(name), maxLength);
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
    public static void AgainstNullOrWhiteSpaceAndOverflow(string value, string paramName, int maxLength)
    {
        AgainstNullOrWhiteSpace(value, paramName);
        AgainstOverflow(value, paramName, maxLength);
    }

    /// <summary>
    /// Checks if the given numeric value is negative and throws an ArgumentOutOfRangeException if it is.
    /// </summary>
    /// <typeparam name="T">The type of the value. Must be a numeric type.</typeparam>
    /// <param name="value">The numeric value to check for negativity.</param>
    /// <param name="paramName">The name of the parameter that will be used in the exception message.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the given value is negative.</exception>
    /// <example>
    /// <code>
    /// public void SetAge(int age)
    /// {
    ///     Guard.AgainstNegative(age, nameof(age));
    ///     Console.WriteLine($"Age is set to: {age}");
    /// }
    /// </code>
    /// </example>
    public static void AgainstNegative<T>(T value, string paramName) where T : IComparable<T>
    {
        var zero = default(T);
        if (value.CompareTo(zero) < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"Parameter '{paramName}' cannot be negative.");
        }
    }

    /// <summary>
    /// Checks if the given numeric value is negative or zero and throws an ArgumentOutOfRangeException if it is.
    /// </summary>
    /// <typeparam name="T">The type of the value. Must be a numeric type.</typeparam>
    /// <param name="value">The numeric value to check for negativity or zero.</param>
    /// <param name="paramName">The name of the parameter that will be used in the exception message.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the given value is negative or zero.</exception>
    /// <example>
    /// <code>
    /// public void SetQuantity(int quantity)
    /// {
    ///     Guard.AgainstNegativeOrZero(quantity, nameof(quantity));
    ///     Console.WriteLine($"Quantity is set to: {quantity}");
    /// }
    /// </code>
    /// </example>
    public static void AgainstNegativeOrZero<T>(T value, string paramName) where T : IComparable<T>
    {
        var zero = default(T);
        if (value.CompareTo(zero) <= 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"Parameter '{paramName}' cannot be negative or zero.");
        }
    }

    /// <summary>
    /// Checks if the given enum value is defined and throws an ArgumentOutOfRangeException if it is not.
    /// </summary>
    /// <typeparam name="T">The type of the enum value.</typeparam>
    /// <param name="value">The enum value to check if it is defined.</param>
    /// <param name="paramName">The name of the parameter that will be used in the exception message.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the given enum value is not defined.</exception>
    /// <example>
    /// <code>
    /// public enum Colors { Red, Green, Blue }
    ///
    /// public void SetColor(Colors color)
    /// {
    ///     Guard.AgainstUndefinedEnumValue(color, nameof(color));
    ///     Console.WriteLine($"Color is set to: {color}");
    /// }
    /// </code>
    /// </example>
    public static void AgainstUndefinedEnumValue<T>(T value, string paramName) where T : Enum
    {
        if (!Enum.IsDefined(typeof(T), value))
        {
            var enumTypeName = typeof(T).Name;
            throw new ArgumentOutOfRangeException(paramName, $"Parameter '{paramName}' has an undefined value '{value}' for enum '{enumTypeName}'.");
        }
    }

    /// <summary>
    /// Checks if the given TimeSpan is within the specified range, inclusive.
    /// </summary>
    /// <param name="value">The TimeSpan to check.</param>
    /// <param name="min">The minimum valid value, inclusive.</param>
    /// <param name="max">The maximum valid value, inclusive.</param>
    /// <param name="paramName">The name of the parameter that will be used in the exception message.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the given TimeSpan is not within the specified range.</exception>
    /// <example>
    /// This sample shows how to call the <see cref="AgainstOutOfRange"/> method.
    /// <code>
    /// class Example
    /// {
    ///     public void SetStartTime(TimeSpan startTime)
    ///     {
    ///         Guard.AgainstOutOfRange(startTime, TimeSpan.Zero, TimeSpan.FromHours(24), nameof(startTime));
    ///         // ... rest of the method
    ///     }
    /// }
    /// </code>
    /// </example>
    public static void AgainstOutOfRange(TimeSpan value, TimeSpan min, TimeSpan max, string paramName)
    {
        if (value < min || value > max)
        {
            throw new ArgumentOutOfRangeException(paramName, $"The value of {paramName} must be between {min} and {max}.");
        }
    }

    /// <summary>
    /// A helper class used to chain the Guard.Against method with the With&lt;TException&gt; method.
    /// </summary>
    public class GuardCondition
    {
        private readonly bool _condition;

        /// <summary>
        /// Initializes a new instance of the GuardCondition class.
        /// </summary>
        /// <remarks>
        /// The constructor is marked as internal to prevent direct instantiation and is only used by the Guard class.
        /// </remarks>
        /// <param name="condition">The condition to check.</param>
        internal GuardCondition(bool condition)
        {
            _condition = condition;
        }

        /// <summary>
        /// Throws the specified exception if the condition is true.
        /// </summary>
        /// <typeparam name="TException">The type of the exception to throw.</typeparam>
        /// <exception cref="TException">Thrown when the condition is true.</exception>
        public void With<TException>() where TException : Exception, new()
        {
            if (_condition)
            {
                throw new TException();
            }
        }
    }

    /// <summary>
    /// Retrieves the parameter name from the given Func&lt;T&gt; expression.
    /// </summary>
    /// <remarks>
    /// This method might not work in all cases, as it relies on the specific implementation of the
    /// compiler-generated closure classes for the lambda expressions.
    /// </remarks>
    /// <typeparam name="T">The type of the value. Must be a reference type.</typeparam>
    /// <param name="valueExpression">A Func&lt;T&gt; that returns the value whose name to retrieve.</param>
    /// <returns>The parameter name extracted from the Func&lt;T&gt; expression.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the parameter name cannot be extracted from the expression.</exception>
    private static string GetParameterName<T>(Func<T> valueExpression)
    {
        // Check if the expression's target is null
        if (valueExpression.Target == null)
        {
            throw new InvalidOperationException("Could not extract the parameter name from the expression.");
        }

        // Try to find the field with the same type as the value
        var fieldInfo = valueExpression.Target.GetType().GetFields().FirstOrDefault(field => field.FieldType == typeof(T)) 
            ?? throw new InvalidOperationException("Could not extract the parameter name from the expression.");

        // Return the name of the field
        return fieldInfo.Name;
    }
}
