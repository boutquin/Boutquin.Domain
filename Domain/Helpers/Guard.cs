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
        => new(condition);

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
        // Get the value and extract the parameter name from the expression
        var (value, paramName) = ExtractParameterInfo(valueExpression);
        
        // Check if the given value is null
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
        // Get the value and extract the parameter name from the expression
        var (value, paramName) = ExtractParameterInfo(valueExpression);

        // Check if the given string value is null or empty
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
        // Get the value and extract the parameter name from the expression
        var (value, paramName) = ExtractParameterInfo(valueExpression);

        // Check if the given string value is null, empty, or contains only whitespace characters
        AgainstNullOrWhiteSpace(value, paramName);
    }

    /// <summary>
    /// Ensures that the length of the string returned by the given <see cref="Func{T}"/> expression is less than or equal to the specified maxLength.
    /// </summary>
    /// <param name="valueExpression">A <see cref="Func{T}"/> that returns the string value to be checked.</param>
    /// <param name="maxLength">The maximum length allowed for the string value. Must be greater than zero.</param>
    /// <example>
    /// This sample shows how to call the <see cref="AgainstOverflow"/> method.
    /// <code>
    /// class Example
    /// {
    ///     static void Main()
    ///     {
    ///         string name = "This is a very long string that exceeds the maximum length.";
    ///         int maxLength = 50;
    ///         Guard.AgainstOverflow(() => name, maxLength);
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the length of the string value exceeds the specified maxLength or when maxLength is less than or equal to zero.
    /// </exception>
    /// <exception cref="InvalidOperationException">Thrown when the parameter name cannot be extracted from the expression.</exception>
    public static void AgainstOverflow(Func<string> valueExpression, int maxLength)
    {
        // Ensure maxLength is greater than zero
        AgainstNegativeOrZero(() => maxLength);

        // Get the value and extract the parameter name from the expression
        var (value, paramName) = ExtractParameterInfo(valueExpression);

        // Check if the length of the string value exceeds the maxLength
        AgainstOverflow(value, paramName, maxLength);
    }

    /// <summary>
    /// Checks if the string returned by the given <see cref="Func{T}"/> expression is null, empty or exceeds the specified maximum length and throws an ArgumentException if it does.
    /// </summary>
    /// <param name="valueExpression">A <see cref="Func{T}"/> that returns the string to check for null, empty, or exceeding the maximum length.</param>
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
    public static void AgainstNullOrEmptyAndOverflow(Func<string> valueExpression, int maxLength)
    {
        // Ensure maxLength is greater than zero
        AgainstNegativeOrZero(() => maxLength);

        // Get the value and extract the parameter name from the expression
        var (value, paramName) = ExtractParameterInfo(valueExpression);

        // Check if the given string value is null or empty
        AgainstNullOrEmpty(value, paramName);
        // Check if the length of the string value exceeds the maxLength
        AgainstOverflow(value, paramName, maxLength);
    }

    /// <summary>
    /// Checks if the string returned by the given <see cref="Func{T}"/> expression is null, empty, consists only of white-space characters, or exceeds the specified maximum length
    /// and throws an ArgumentException if it does.
    /// </summary>
    /// <param name="valueExpression">A <see cref="Func{T}"/> that returns the string to check for null, empty, white-space characters, or exceeding the maximum length.</param>
    /// <param name="maxLength">The maximum length allowed for the string.</param>
    /// <exception cref="ArgumentException">Thrown when the given string is null, empty, consists only of white-space characters, or exceeds the maximum length.</exception>
    /// <example>
    /// <code>
    /// public void PrintGreeting(string name, int maxLength)
    /// {
    ///     Guard.AgainstNullOrWhiteSpaceAndOverflow(() => name, maxLength);
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
    /// <exception cref="InvalidOperationException">Thrown when the parameter name cannot be extracted from the expression.</exception>
    public static void AgainstNullOrWhiteSpaceAndOverflow(Func<string> valueExpression, int maxLength)
    {
        // Ensure maxLength is greater than zero
        AgainstNegativeOrZero(() => maxLength);

        // Get the value and extract the parameter name from the expression
        var (value, paramName) = ExtractParameterInfo(valueExpression);

        // Check if the given string value is null, empty, or contains only whitespace characters
        AgainstNullOrWhiteSpace(value, paramName);
        // Check if the length of the string value exceeds the maxLength
        AgainstOverflow(value, paramName, maxLength);
    }

    /// <summary>
    /// Checks if the value returned by the given <see cref="Func{T}"/> expression is negative and throws an ArgumentOutOfRangeException if it is.
    /// </summary>
    /// <typeparam name="T">The type of the value. Must be a comparable value type.</typeparam>
    /// <param name="valueExpression">A <see cref="Func{T}"/> that returns the value to check for negativity.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the given value is negative.</exception>
    /// <example>
    /// <code>
    /// public void SetLength(int length)
    /// {
    ///     Guard.AgainstNegative(() => length, nameof(length));
    ///     Console.WriteLine($"Length is set to: {length}");
    /// }
    ///
    /// class Program
    /// {
    ///     static void Main(string[] args)
    ///     {
    ///         SetLength(10); // Works fine
    ///         SetLength(-5); // Throws ArgumentOutOfRangeException: "Parameter 'length' cannot be negative."
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <exception cref="InvalidOperationException">Thrown when the parameter name cannot be extracted from the expression.</exception>
    public static void AgainstNegative<T>(Func<T> valueExpression) where T : IComparable<T>
    {
        // Get the value and extract the parameter name from the expression
        var (value, paramName) = ExtractParameterInfo(valueExpression);

        // Compare the value with zero
        var zero = default(T);
        if (value.CompareTo(zero) < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"Parameter '{paramName}' cannot be negative.");
        }
    }

    /// <summary>
    /// Ensures that the value returned by the given <see cref="Func{T}"/> expression is greater than zero.
    /// </summary>
    /// <typeparam name="T">The type of the value. Must be a comparable type.</typeparam>
    /// <param name="valueExpression">A <see cref="Func{T}"/> that returns the value to be checked.</param>
    /// <example>
    /// This sample shows how to call the <see cref="AgainstNegativeOrZero{T}"/> method.
    /// <code>
    /// class Example
    /// {
    ///     static void Main()
    ///     {
    ///         int quantity = -5;
    ///         Guard.AgainstNegativeOrZero(() => quantity);
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is negative or zero.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the parameter name cannot be extracted from the expression.</exception>
    public static void AgainstNegativeOrZero<T>(Func<T> valueExpression) where T : IComparable<T>
    {
        // Get the value and extract the parameter name from the expression
        var (value, paramName) = ExtractParameterInfo(valueExpression);

        // Compare the value with zero
        var zero = default(T);
        if (value.CompareTo(zero) <= 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"Parameter '{paramName}' cannot be negative or zero.");
        }
    }

    /// <summary>
    /// Checks if the enum value returned by the given <see cref="Func{T}"/> expression is defined and throws an ArgumentOutOfRangeException if it is not.
    /// </summary>
    /// <typeparam name="T">The type of the enum value.</typeparam>
    /// <param name="valueExpression">A <see cref="Func{T}"/> that returns the enum value to check if it is defined.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the given enum value is not defined.</exception>
    /// <example>
    /// <code>
    /// public enum Colors { Red, Green, Blue }
    ///
    /// public void SetColor(Colors color)
    /// {
    ///     Guard.AgainstUndefinedEnumValue(() => color);
    ///     Console.WriteLine($"Color is set to: {color}");
    /// }
    /// </code>
    /// </example>
    /// <exception cref="InvalidOperationException">Thrown when the parameter name cannot be extracted from the expression.</exception>
    public static void AgainstUndefinedEnumValue<T>(Func<T> valueExpression) where T : Enum
    {
        // Get the value and extract the parameter name from the expression
        var (value, paramName) = ExtractParameterInfo(valueExpression);

        if (!Enum.IsDefined(typeof(T), value))
        {
            var enumTypeName = typeof(T).Name;
            throw new ArgumentOutOfRangeException(paramName, $"Parameter '{paramName}' has an undefined value '{value}' for enum '{enumTypeName}'.");
        }
    }

    /// <summary>
    /// Checks if the value returned by the given <see cref="Func{T}"/> expression is within the specified range, inclusive.
    /// </summary>
    /// <typeparam name="T">The type of the value. Must be a comparable value type.</typeparam>
    /// <param name="valueExpression">A <see cref="Func{T}"/> that returns the value to check for range.</param>
    /// <param name="min">The minimum valid value, inclusive.</param>
    /// <param name="max">The maximum valid value, inclusive.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the given value is not within the specified range.</exception>
    /// <example>
    /// <code>
    /// public void SetPercentage(int percentage)
    /// {
    ///     Guard.AgainstOutOfRange(() => percentage, 0, 100);
    ///     Console.WriteLine($"Percentage is set to: {percentage}");
    /// }
    ///
    /// class Program
    /// {
    ///     static void Main(string[] args)
    ///     {
    ///         SetPercentage(50); // Works fine
    ///         SetPercentage(-10); // Throws ArgumentOutOfRangeException: "The value of 'percentage' must be between 0 and 100."
    ///         SetPercentage(110); // Throws ArgumentOutOfRangeException: "The value of 'percentage' must be between 0 and 100."
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <exception cref="InvalidOperationException">Thrown when the parameter name cannot be extracted from the expression.</exception>
    public static void AgainstOutOfRange<T>(Func<T> valueExpression, T min, T max) where T : IComparable<T>
    {
        // Validate parameters
        Against(min.CompareTo(max) >= 0)
            .With<ArgumentException>($"The value of parameter 'max' {max} must be greater than the value of parameter 'min' {min}");

        // Get the value and extract the parameter name from the expression
        var (value, paramName) = ExtractParameterInfo(valueExpression);

        // Compare the value with min and max
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
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
            => _condition = condition;

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

        /// <summary>
        /// Throws the specified exception with the provided message if the condition is true.
        /// </summary>
        /// <remarks>
        /// If the exceptionMessage is null, empty, or contains only whitespace characters,
        /// an ArgumentException will be thrown.
        /// </remarks>
        /// <typeparam name="TException">The type of the exception to throw.</typeparam>
        /// <param name="exceptionMessage">The message for the exception.</param>
        /// <exception cref="TException">Thrown when the condition is true, with the provided exception message.</exception>
        /// <exception cref="ArgumentException">Thrown when the exceptionMessage is null, empty, or contains only whitespace characters.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the specified exception type doesn't have a constructor that accepts a single string parameter.</exception>
        public void With<TException>(string exceptionMessage) where TException : Exception
        {
            // Validate parameters
            AgainstNullOrWhiteSpace(() => exceptionMessage);

            // Guard logic
            if (_condition)
            {
                TException exception;
                try
                {
                    exception = (TException)Activator.CreateInstance(typeof(TException), exceptionMessage);
                }
                catch (MissingMethodException)
                {
                    throw new InvalidOperationException(
                        $"The exception type '{typeof(TException).FullName}' must have a constructor that accepts a single string parameter.");
                }
                throw exception;
            }
        }

        /// <summary>
        /// Throws the specified exception with the provided message and inner exception if the condition is true.
        /// </summary>
        /// <remarks>
        /// If the exceptionMessage is null, empty, or contains only whitespace characters,
        /// an ArgumentException will be thrown. If the innerException is null, an ArgumentNullException will be thrown.
        /// </remarks>
        /// <typeparam name="TException">The type of the exception to throw.</typeparam>
        /// <param name="exceptionMessage">The message for the exception.</param>
        /// <param name="innerException">The inner exception to be included.</param>
        /// <exception cref="TException">Thrown when the condition is true, with the provided exception message and inner exception.</exception>
        /// <exception cref="ArgumentException">Thrown when the exceptionMessage is null, empty, or contains only whitespace characters.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the innerException is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the specified exception type doesn't have a constructor that accepts a single string parameter and an Exception parameter.</exception>
        public void With<TException>(string exceptionMessage, Exception innerException) where TException : Exception
        {
            // Validate parameters
            AgainstNullOrWhiteSpace(() => exceptionMessage);
            AgainstNull(() => innerException);

            // Guard logic
            if (_condition)
            {
                TException exception;
                try
                {
                    exception = (TException)Activator.CreateInstance(typeof(TException), exceptionMessage, innerException);
                }
                catch (MissingMethodException)
                {
                    throw new InvalidOperationException(
                        $"The exception type '{typeof(TException).FullName}' must have a constructor that accepts a single string parameter and an Exception parameter.");
                }
                throw exception;
            }
        }
    }

    /// <summary>
    /// Extracts the parameter value and parameter name from the given <see cref="Func{T}"/> expression.
    /// </summary>
    /// <typeparam name="T">The type of the value. Must be a reference type.</typeparam>
    /// <param name="valueExpression">A <see cref="Func{T}"/> that returns the value whose name and value to retrieve.</param>
    /// <returns>A tuple containing the extracted parameter value and parameter name.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the parameter name cannot be extracted from the expression.</exception>
    private static (T value, string paramName) ExtractParameterInfo<T>(Func<T> valueExpression)
    {
        // Check if the expression's target is null
        if (valueExpression.Target == null)
        {
            throw new InvalidOperationException("Could not extract the parameter information from the expression.");
        }

        // Get the value from the expression
        var value = valueExpression();

        // Try to find the field with the same type as the value
        var fieldInfo = valueExpression.Target.GetType().GetFields().FirstOrDefault(field => field.FieldType == typeof(T))
            ?? throw new InvalidOperationException("Could not extract the parameter information from the expression.");

        // Return the name of the field and the value as a tuple
        return (value, fieldInfo.Name);
    }

    /// <summary>
    /// Checks if the given string is null or empty and throws an ArgumentException if it is.
    /// </summary>
    /// <param name="value">The string to check for null or empty.</param>
    /// <param name="paramName">The name of the parameter that will be used in the exception message.</param>
    /// <exception cref="ArgumentException">Thrown when the given string is null or empty.</exception>
    private static void AgainstNullOrEmpty(string value, string paramName)
    {
        // Check if the given string is null or empty 
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
    private static void AgainstNullOrWhiteSpace(string value, string paramName)
    {
        // Check if the given string is null, empty, or consists only of white-space characters
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
    private static void AgainstOverflow(string value, string paramName, int maxLength)
    {
        // Check if the length of the string value exceeds the maxLength
        if (value.Length > maxLength)
        {
            // Throw an ArgumentOutOfRangeException if the length of the string value exceeds the maxLength
            throw new ArgumentOutOfRangeException(paramName, $"The length of the parameter '{paramName}' cannot exceed {maxLength} characters.");
        }
    }
}
