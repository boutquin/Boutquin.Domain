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

namespace Boutquin.Domain.Helpers;

// ReSharper disable UnusedMember.Global

using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Exceptions;

/// <summary>
/// The Guard class provides a set of static methods to simplify the validation of method arguments and other conditions.
/// It helps prevent bugs by throwing exceptions when conditions are not met.
/// </summary>
/// <remarks>
/// The Guard class is designed to make it easier to implement guard clauses and other validation checks
/// in your code. It provides a fluent API that allows for simple and expressive validation code.
///
/// When using the Guard class, you can use the static methods to check conditions, and then use
/// the With methods to throw a specific exception if the condition is not met.
///
/// The Guard class supports a wide range of validation scenarios, including null or empty strings,
/// out-of-range values, invalid enum values, and more.
///
/// <para>
/// <strong>Performance note:</strong> The expression-based overloads (e.g., <c>Guard.AgainstNull(() =&gt; value)</c>)
/// compile the expression tree on every invocation to extract both the value and parameter name. This provides
/// excellent developer ergonomics but has measurable overhead. For performance-sensitive hot paths, prefer the
/// overloads that accept the value and parameter name directly (e.g., <c>Guard.AgainstNullOrWhiteSpace(value, nameof(value))</c>).
/// </para>
/// </remarks>
/// <example>
/// This sample shows how to use the Guard class to validate method arguments:
/// <code>
/// public void SetName(string name)
/// {
///     Guard.AgainstNullOrWhiteSpace(name, nameof(name));
///     // ... rest of the method
/// }
/// </code>
/// </example>
public static class Guard
{
    /// <summary>
    /// Checks if the given condition is true and throws the specified exception if it is.
    /// </summary>
    /// <example>
    /// <code>
    /// public void SetQuantity(int quantity)
    /// {
    ///     Guard.Against(quantity &lt;= 0)
    ///         .With&lt;ArgumentException&gt;();
    ///     Console.WriteLine($"Quantity is set to: {quantity}");
    /// }
    /// </code>
    /// </example>
    /// <param name="condition">The condition to check.</param>
    /// <returns>An instance of GuardCondition to chain with With&lt;TException&gt; method.</returns>
    public static GuardCondition Against(bool condition)
        => new(condition);

    /// <summary>
    /// Checks if the given reference type value is null and throws an <see cref="ArgumentNullException"/> if it is.
    /// This overload accepts an <see cref="Expression{TDelegate}"/> wrapping a function that returns the value and extracts its name from the expression tree.
    /// </summary>
    /// <typeparam name="T">The type of the value being checked. Must be a reference type.</typeparam>
    /// <param name="valueExpression">An <see cref="Expression{TDelegate}"/> wrapping a function that returns the value to check for null.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the given value is null (violation of Guard logic).
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the parameter name cannot be extracted from the <paramref name="valueExpression"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// public void PrintList(List&lt;string&gt; items)
    /// {
    ///     Guard.AgainstNull(() => items);
    ///     foreach (var item in items)
    ///     {
    ///         Console.WriteLine(item);
    ///     }
    /// }
    /// </code>
    /// </example>
    public static void AgainstNull<T>(Expression<Func<T>> valueExpression) where T : class
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
    /// Checks if the given value type is its default value and throws an <see cref="ArgumentException"/> if it is.
    /// This overload accepts an <see cref="Expression{TDelegate}"/> wrapping a function that returns the value and extracts its name from the expression tree.
    /// </summary>
    /// <typeparam name="T">The type of the value being checked. Must be a value type.</typeparam>
    /// <param name="valueExpression">An <see cref="Expression{TDelegate}"/> wrapping a function that returns the value to check for default.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the given value is its default value (violation of Guard logic).
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the parameter name cannot be extracted from the <paramref name="valueExpression"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// public void ProcessData(int data)
    /// {
    ///     Guard.AgainstDefault(() => data);
    ///     // ... rest of the method
    /// }
    /// </code>
    /// </example>
    public static void AgainstDefault<T>(Expression<Func<T>> valueExpression) where T : struct
    {
        // Get the value and extract the parameter name from the expression
        var (value, paramName) = ExtractParameterInfo(valueExpression);

        // Check if the given value is the default value for its type
        if (EqualityComparer<T>.Default.Equals(value, default(T)))
        {
            throw new ArgumentException($"Parameter '{paramName}' cannot be the default value.", paramName);
        }
    }

    /// <summary>
    /// Checks if the given value is either null (for reference types) or default (for value types) and throws an appropriate exception if it is.
    /// </summary>
    /// <typeparam name="T">The type of the value being checked. This overload is intended for
    /// <em>nullable value types</em> (e.g. <c>int?</c>) and reference types: for a
    /// <see cref="Nullable{T}"/> it rejects both <c>null</c> <em>and</em> a present value equal to the
    /// <em>underlying</em> type's default (so <c>(int?)0</c> is rejected as the default value); for a
    /// reference type, <c>null</c> and <c>default</c> coincide so only the
    /// <see cref="ArgumentNullException"/> branch applies; for a non-nullable value type the <c>null</c>
    /// branch can never fire and the check reduces to the default-value check.</typeparam>
    /// <param name="valueExpression">An <see cref="Expression{TDelegate}"/> wrapping a function that returns the value to check.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the given value is null (reference types and a null <see cref="Nullable{T}"/>).
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the given value is the default value — for a value type, for a non-nullable value
    /// type, or for a <see cref="Nullable{T}"/> whose present value equals the underlying type's default
    /// (e.g. <c>(int?)0</c>).
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the parameter name cannot be extracted from the <paramref name="valueExpression"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// public void ProcessData(int data)
    /// {
    ///     Guard.AgainstNullOrDefault(() => data);
    ///     // ... rest of the method
    /// }
    /// </code>
    /// </example>
    public static void AgainstNullOrDefault<T>(Expression<Func<T>> valueExpression)
    {
        // Get the value and extract the parameter name from the expression
        var (value, paramName) = ExtractParameterInfo(valueExpression);

        if (value is null)
        {
            throw new ArgumentNullException(paramName, $"Parameter '{paramName}' cannot be null.");
        }

        // For a Nullable<U>, EqualityComparer<T>.Default.Equals(value, default) compares the present
        // value against a null Nullable and can never fire — so (int?)0 silently passed. Compare the
        // unwrapped value against the UNDERLYING type's default instead, so (int?)0 is rejected exactly
        // as a bare int 0 would be (DE-4).
        var underlyingType = Nullable.GetUnderlyingType(typeof(T));
        if (underlyingType is not null)
        {
            // default(U) is zero-initialized even when U declares a parameterless constructor.
            // Activator would run that constructor, changing the comparison value (and its effects).
            var underlyingDefault = RuntimeHelpers.GetUninitializedObject(underlyingType);
            if (Equals(value, underlyingDefault))
            {
                throw new ArgumentException($"Parameter '{paramName}' cannot be the default value.", paramName);
            }

            return;
        }

        if (EqualityComparer<T>.Default.Equals(value, default))
        {
            throw new ArgumentException($"Parameter '{paramName}' cannot be the default value.", paramName);
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if the specified enumerable is null or empty.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
    /// <param name="enumerableExpression">An expression returning the enumerable to be checked.</param>
    /// <exception cref="ArgumentException">Thrown if the enumerable is null or empty.</exception>
    /// <remarks>
    /// <b>Re-enumeration caveat:</b> the emptiness check calls <see cref="Enumerable.Any{TSource}(IEnumerable{TSource})"/>,
    /// which enumerates at least the first element of <paramref name="enumerableExpression"/>'s result. For
    /// a one-shot enumerable backed by a forward-only iterator (e.g. a C# iterator method invoked directly,
    /// or a stream reader projected through <c>Select</c>), that element is consumed here and is not seen
    /// again by the caller's own subsequent enumeration. Pass a re-enumerable sequence (an array, a
    /// materialized <see cref="List{T}"/>) when the caller needs to iterate the checked value afterward.
    /// </remarks>
    /// <example>
    /// <code>
    /// public void ProcessData(IEnumerable&lt;string&gt; data)
    /// {
    ///     Guard.AgainstEmptyOrNullEnumerable(() => data);
    ///     // ... rest of the method
    /// }
    /// </code>
    /// </example>
    public static void AgainstEmptyOrNullEnumerable<T>(Expression<Func<IEnumerable<T>>> enumerableExpression)
    {
        // Get the enumerable value and extract the parameter name from the expression
        var (enumerable, paramName) = ExtractParameterInfo(enumerableExpression);

        if (enumerable != null && enumerable.Any())
        {
            return;
        }

        throw new ArgumentException($"Parameter '{paramName}' must have at least one element.", paramName);
    }

    /// <summary>
    /// Checks if the given array is null or empty and throws an <see cref="EmptyOrNullArrayException"/>
    /// (an <see cref="ArgumentException"/> subtype) if it is.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="valueExpression">A function expression returning the array to check for null or empty.</param>
    /// <exception cref="EmptyOrNullArrayException">Thrown when the given array is null or empty. Derives
    /// from <see cref="ArgumentException"/>.</exception>
    /// <example>
    /// This sample shows how to call the <see cref="AgainstEmptyOrNullArray{T}"/> method.
    /// <code>
    /// public void ProcessData(int[] data)
    /// {
    ///     Guard.AgainstEmptyOrNullArray(() => data);
    ///     // ... rest of the method
    /// }
    /// </code>
    /// </example>
    public static void AgainstEmptyOrNullArray<T>(Expression<Func<T[]>> valueExpression)
    {
        // Get the array value and extract the parameter name from the expression
        var (value, paramName) = ExtractParameterInfo(valueExpression);

        // Guard logic
        if (value == null || value.Length == 0)
        {
            throw new EmptyOrNullArrayException($"Parameter '{paramName}' cannot be null or an empty array.");
        }
    }

    /// <summary>
    /// Throws an <see cref="EmptyOrNullCollectionException"/> if the specified collection is null or empty.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collectionExpression">An expression returning the collection to be checked.</param>
    /// <exception cref="EmptyOrNullCollectionException">Thrown if the collection is null or empty.</exception>
    /// <example>
    /// <code>
    /// public void SomeMethod(List&lt;int&gt; inputList)
    /// {
    ///     Guard.AgainstEmptyOrNullCollection(() => inputList);
    ///     // ... continue processing ...
    /// }
    /// </code>
    /// </example>
    public static void AgainstEmptyOrNullCollection<T>(Expression<Func<ICollection<T>>> collectionExpression)
    {
        var (collection, paramName) = ExtractParameterInfo(collectionExpression);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (collection != null && collection.Count != 0)
        {
            return;
        }

        throw new EmptyOrNullCollectionException($"Parameter '{paramName}' cannot be null or an empty collection.");
    }

    /// <summary>
    /// Throws an <see cref="EmptyOrNullDictionaryException"/> if the specified dictionary is null or empty.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <param name="dictionaryExpression">An expression returning the dictionary to be checked.</param>
    /// <exception cref="EmptyOrNullDictionaryException">Thrown if the dictionary is null or empty.</exception>
    /// <example>
    /// <code>
    /// public void SomeMethod(Dictionary&lt;int, string&gt; inputDict)
    /// {
    ///     Guard.AgainstEmptyOrNullDictionary(() => inputDict);
    ///     // ... continue processing ...
    /// }
    /// </code>
    /// </example>
    public static void AgainstEmptyOrNullDictionary<TKey, TValue>(Expression<Func<IDictionary<TKey, TValue>>> dictionaryExpression) where TKey : notnull
    {
        var (dictionary, paramName) = ExtractParameterInfo(dictionaryExpression);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (dictionary != null && dictionary.Count != 0)
        {
            return;
        }

        throw new EmptyOrNullDictionaryException($"Parameter '{paramName}' cannot be null or an empty dictionary.");
    }

    /// <summary>
    /// Throws an <see cref="EmptyOrNullDictionaryException"/> if the specified dictionary is null or empty.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <param name="dictionaryExpression">An expression returning the dictionary to be checked.</param>
    /// <exception cref="EmptyOrNullDictionaryException">Thrown if the dictionary is null or empty.</exception>
    /// <example>
    /// <code>
    /// public void SomeMethod(Dictionary&lt;int, string&gt; inputDict)
    /// {
    ///     Guard.AgainstEmptyOrNullReadOnlyDictionary(() => inputDict);
    ///     // ... continue processing ...
    /// }
    /// </code>
    /// </example>
    public static void AgainstEmptyOrNullReadOnlyDictionary<TKey, TValue>(Expression<Func<IReadOnlyDictionary<TKey, TValue>>> dictionaryExpression) where TKey : notnull
    {
        var (dictionary, paramName) = ExtractParameterInfo(dictionaryExpression);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (dictionary != null && dictionary.Count != 0)
        {
            return;
        }

        throw new EmptyOrNullDictionaryException($"Parameter '{paramName}' cannot be null or an empty dictionary.");
    }

    /// <summary>
    /// Checks if the given string value is null or empty and throws an <see cref="ArgumentException"/> if it is.
    /// This overload accepts a Func&lt;string&gt; that returns the value and extracts its name using nameof.
    /// </summary>
    /// <param name="valueExpression">A Func&lt;string&gt; that returns the value to check for null or empty.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the given string value is null or empty (violation of Guard logic).
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the parameter name cannot be extracted from the <paramref name="valueExpression"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// public void PrintMessage(string message)
    /// {
    ///     Guard.AgainstNullOrEmpty(() => message);
    ///     Console.WriteLine(message);
    /// }
    /// </code>
    /// </example>
    public static void AgainstNullOrEmpty(Expression<Func<string>> valueExpression)
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
    /// Checks if the given string value is null, empty, or contains only whitespace characters and throws an <see cref="ArgumentException"/> if it is.
    /// </summary>
    /// <param name="valueExpression">A Func&lt;string&gt; that returns the value to check for null, empty, or whitespace.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the given string value is null, empty, or contains only whitespace characters (violation of Guard logic).
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the parameter name cannot be extracted from the <paramref name="valueExpression"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// public void PrintMessage(string message)
    /// {
    ///     Guard.AgainstNullOrWhiteSpace(() => message);
    ///     Console.WriteLine(message);
    /// }
    /// </code>
    /// </example>
    public static void AgainstNullOrWhiteSpace(Expression<Func<string>> valueExpression)
    {
        // Get the value and extract the parameter name from the expression
        var (value, paramName) = ExtractParameterInfo(valueExpression);

        // Check if the given string value is null, empty, or contains only whitespace characters
        ThrowIfNullOrWhiteSpace(value, paramName);
    }

    /// <summary>
    /// Ensures that the length of the string returned by the given <see cref="Func{T}"/> expression is less than or equal to the specified maxLength.
    /// </summary>
    /// <param name="valueExpression">An <see cref="Expression{TDelegate}"/> wrapping a function that returns the string value to be checked.</param>
    /// <param name="maxLength">The maximum length allowed for the string value. Must be greater than zero.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the string returned by <paramref name="valueExpression"/> is null (violation of Guard logic).
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the length of the string value exceeds the specified <paramref name="maxLength"/>,
    /// or when <paramref name="maxLength"/> is less than or equal to zero (violation of Guard logic).
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the parameter name cannot be extracted from the <paramref name="valueExpression"/>.
    /// </exception>
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
    public static void AgainstOverflow(Expression<Func<string>> valueExpression, int maxLength)
    {
        // Ensure maxLength is greater than zero
        AgainstNegativeOrZero(() => maxLength);

        // Get the value and extract the parameter name from the expression
        var (value, paramName) = ExtractParameterInfo(valueExpression);

        // A null string would surface as a NullReferenceException inside ThrowIfOverflow (value.Length).
        // Reject it as a clear ArgumentException instead. (AgainstNullOrEmptyAndOverflow guards null first.)
        if (value is null)
        {
            throw new ArgumentException($"Parameter '{paramName}' cannot be null.", paramName);
        }

        // Check if the length of the string value exceeds the maxLength
        ThrowIfOverflow(value, paramName, maxLength);
    }

    /// <summary>
    /// Checks if the string returned by the given <see cref="Func{T}"/> expression is null or empty
    /// (rejected with an <see cref="ArgumentException"/>) or exceeds the specified maximum length
    /// (rejected with an <see cref="ArgumentOutOfRangeException"/>).
    /// </summary>
    /// <param name="valueExpression">An <see cref="Expression{TDelegate}"/> wrapping a function that returns the string to check for null, empty, or exceeding the maximum length.</param>
    /// <param name="maxLength">The maximum length allowed for the string.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the given string is null or empty (violation of Guard logic).
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the string exceeds the maximum length — message
    /// <c>"The length of the parameter '{name}' cannot exceed {maxLength} characters."</c> — or when
    /// <paramref name="maxLength"/> is negative or zero.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the parameter name cannot be extracted from the <paramref name="valueExpression"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// public void PrintGreeting(string name, int maxLength)
    /// {
    ///     Guard.AgainstNullOrEmptyAndOverflow(() => name, maxLength);
    ///     Console.WriteLine($"Hello, {name}!");
    /// }
    /// </code>
    /// </example>
    public static void AgainstNullOrEmptyAndOverflow(Expression<Func<string>> valueExpression, int maxLength)
    {
        // Ensure maxLength is greater than zero
        AgainstNegativeOrZero(() => maxLength);

        // Get the value and extract the parameter name from the expression
        var (value, paramName) = ExtractParameterInfo(valueExpression);

        // Check if the given string value is null or empty
        ThrowIfNullOrEmpty(value, paramName);
        // Check if the length of the string value exceeds the maxLength
        ThrowIfOverflow(value, paramName, maxLength);
    }

    /// <summary>
    /// Checks if the string returned by the given <see cref="Func{T}"/> expression is null, empty, or
    /// consists only of white-space characters (rejected with an <see cref="ArgumentException"/>) or
    /// exceeds the specified maximum length (rejected with an <see cref="ArgumentOutOfRangeException"/>).
    /// </summary>
    /// <param name="valueExpression">An <see cref="Expression{TDelegate}"/> wrapping a function that returns the string to check for null, empty, white-space characters, or exceeding the maximum length.</param>
    /// <param name="maxLength">The maximum length allowed for the string.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the given string is null, empty, or consists only of white-space characters (violation of Guard logic).
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the string exceeds the maximum length — message
    /// <c>"The length of the parameter '{name}' cannot exceed {maxLength} characters."</c> — or when
    /// <paramref name="maxLength"/> is negative or zero.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the parameter name cannot be extracted from the <paramref name="valueExpression"/>.
    /// </exception>
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
    ///         PrintGreeting("John Doe", 5); // Throws ArgumentOutOfRangeException: "The length of the parameter 'name' cannot exceed 5 characters."
    ///     }
    /// }
    /// </code>
    /// </example>
    public static void AgainstNullOrWhiteSpaceAndOverflow(Expression<Func<string>> valueExpression, int maxLength)
    {
        // Ensure maxLength is greater than zero
        AgainstNegativeOrZero(() => maxLength);

        // Get the value and extract the parameter name from the expression
        var (value, paramName) = ExtractParameterInfo(valueExpression);

        // Check if the given string value is null, empty, or contains only whitespace characters
        ThrowIfNullOrWhiteSpace(value, paramName);
        // Check if the length of the string value exceeds the maxLength
        ThrowIfOverflow(value, paramName, maxLength);
    }

    /// <summary>
    /// Checks if the value returned by the given <see cref="Func{T}"/> expression is negative and throws an <see cref="ArgumentOutOfRangeException"/> if it is.
    /// </summary>
    /// <typeparam name="T">The type of the value. Must be a comparable value type.</typeparam>
    /// <param name="valueExpression">An <see cref="Expression{TDelegate}"/> wrapping a function that returns the value to check for negativity.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the given value is negative (violation of Guard logic).
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the parameter name cannot be extracted from the <paramref name="valueExpression"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// public void SetLength(int length)
    /// {
    ///     Guard.AgainstNegative(() => length);
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
    public static void AgainstNegative<T>(Expression<Func<T>> valueExpression) where T : struct, IComparable<T>
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
    /// <param name="valueExpression">An <see cref="Expression{TDelegate}"/> wrapping a function that returns the value to be checked.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the value is negative or zero (violation of Guard logic).
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the parameter name cannot be extracted from the <paramref name="valueExpression"/>.
    /// </exception>
    /// <example>
    /// This sample shows how to call the <see cref="AgainstNegativeOrZero{T}(Expression{Func{T}})"/> method.
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
    public static void AgainstNegativeOrZero<T>(Expression<Func<T>> valueExpression) where T : struct, IComparable<T>
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
    /// Checks if the type parameter T is an enum and throws an ArgumentException if it is not.
    /// </summary>
    /// <typeparam name="T">The type to check if it is an enum.</typeparam>
    /// <exception cref="ArgumentException">Thrown when the type parameter T is not an enum.</exception>
    /// <example>
    /// <code>
    /// public void EnumOperation&lt;T&gt;() where T : Enum
    /// {
    ///     Guard.AgainstNonEnumType&lt;T&gt;();
    ///     // ... rest of the method
    /// }
    /// </code>
    /// </example>
    public static void AgainstNonEnumType<T>()
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException($"The type parameter '{typeof(T).Name}' must be an enum.");
        }
    }

    /// <summary>
    /// Checks if the enum value returned by the given <see cref="Func{T}"/> expression is defined and throws an <see cref="ArgumentOutOfRangeException"/> if it is not.
    /// </summary>
    /// <typeparam name="T">The type of the enum value.</typeparam>
    /// <param name="valueExpression">An <see cref="Expression{TDelegate}"/> wrapping a function that returns the enum value to check if it is defined.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the given enum value is not defined (violation of Guard logic).
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the parameter name cannot be extracted from the <paramref name="valueExpression"/>.
    /// </exception>
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
    public static void AgainstUndefinedEnumValue<T>(Expression<Func<T>> valueExpression) where T : Enum
    {
        // Get the value and extract the parameter name from the expression
        var (value, paramName) = ExtractParameterInfo(valueExpression);

        if (Enum.IsDefined(typeof(T), value))
        {
            return;
        }

        var enumTypeName = typeof(T).Name;
        throw new ArgumentOutOfRangeException(paramName, $"Parameter '{paramName}' has an undefined value '{value}' for enum '{enumTypeName}'.");
    }

    /// <summary>
    /// Checks if the value returned by the given <see cref="Func{T}"/> expression is within the specified range, inclusive.
    /// </summary>
    /// <typeparam name="T">The type of the value. Must be a comparable value type.</typeparam>
    /// <param name="valueExpression">An <see cref="Expression{TDelegate}"/> wrapping a function that returns the value to check for range.</param>
    /// <param name="min">The minimum valid value, inclusive.</param>
    /// <param name="max">The maximum valid value, inclusive.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the given value is not within the specified range (violation of Guard logic).
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the given <paramref name="max"/> value is not greater than the given <paramref name="min"/> value.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the parameter name cannot be extracted from the <paramref name="valueExpression"/>.
    /// </exception>
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
    public static void AgainstOutOfRange<T>(Expression<Func<T>> valueExpression, T min, T max) where T : struct, IComparable<T>
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
    public sealed class GuardCondition
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
        /// <typeparam name="TException">The type of the exception to throw. The exception type must have a parameterless constructor.</typeparam>
        /// <example>
        /// <code>
        /// public sealed class CustomException : Exception
        /// {
        ///     public CustomException() : base("An error occurred.")
        ///     {
        ///     }
        /// }
        ///
        /// class Example
        /// {
        ///     public void ProcessData(int data)
        ///     {
        ///         Guard.Against(data &lt; 0)
        ///              .With&lt;CustomException&gt;();
        ///
        ///         // ... rest of the method
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// Throws the caller-supplied <typeparamref name="TException"/> when the Guard condition is
        /// true, constructed via its parameterless constructor. A generic type parameter has no fixed
        /// <c>cref</c> target, so this contract is stated here in prose rather than as an
        /// <c>&lt;exception&gt;</c> tag.
        /// </remarks>
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
        /// If the exceptionMessage is null, empty, or contains only whitespace characters, an <see cref="ArgumentException"/> will be thrown.
        /// Throws the caller-supplied <typeparamref name="TException"/> when the Guard condition is true,
        /// constructed with <paramref name="exceptionMessage"/>. A generic type parameter has no fixed
        /// <c>cref</c> target, so this contract is stated here in prose rather than as an
        /// <c>&lt;exception&gt;</c> tag.
        /// </remarks>
        /// <typeparam name="TException">The type of the exception to throw.</typeparam>
        /// <param name="exceptionMessage">The message for the exception.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the <paramref name="exceptionMessage"/> is null, empty, or contains only whitespace characters.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the exception type does not have a constructor that accepts a single string parameter 
        /// or when the constructor fails to create a valid instance with the provided exception message.
        /// </exception>
        /// <exception cref="TargetInvocationException">
        /// Thrown when the constructor of the specified exception type throws an exception. The actual 
        /// exception thrown by the constructor can be found in the InnerException property.
        /// </exception>
        /// <example>
        /// <code>
        /// public sealed class CustomException : Exception
        /// {
        ///     public CustomException(string message)
        ///         : base(message)
        ///     {
        ///     }
        /// }
        ///
        /// class Example
        /// {
        ///     public void ProcessData(int data)
        ///     {
        ///         Guard.Against(data &lt; 0)
        ///              .With&lt;CustomException&gt;("Data cannot be negative.");
        ///
        ///         // ... rest of the method
        ///     }
        /// }
        /// </code>
        /// </example>
        public void With<TException>(string exceptionMessage) where TException : Exception
        {
            // Validate parameters. Use the CallerArgumentExpression overload (no lambda compilation)
            // since exceptionMessage is a plain parameter, not an Expression.
            AgainstNullOrWhiteSpace(exceptionMessage, nameof(exceptionMessage));

            // Guard logic
            if (!_condition)
            {
                return;
            }

            TException exception;
            try
            {
#pragma warning disable CS8600
                exception = (TException)Activator.CreateInstance(typeof(TException), exceptionMessage);
#pragma warning restore CS8600
            }
            catch (MissingMethodException)
            {
                throw new InvalidOperationException(
                    $"The exception type '{typeof(TException).FullName}' must have a constructor that accepts a single string parameter.");
            }
            catch (TargetInvocationException tie)
            {
                if (tie.InnerException is not null)
                {
                    // Preserve the constructor exception's original stack trace rather than resetting
                    // it to this re-throw site (which `throw tie.InnerException;` would do).
                    ExceptionDispatchInfo.Capture(tie.InnerException).Throw();
                }
                throw;
            }

            // Check if the created exception instance is null
            if (exception == null)
            {
                throw new InvalidOperationException(
                    $"The exception type '{typeof(TException).FullName}' failed to create a valid instance with the provided exception message.");
            }

            // Throw the created exception
            throw exception;
        }

        /// <summary>
        /// Throws the specified exception with a formatted message if the condition is true, using the provided exception message and format arguments.
        /// </summary>
        /// <remarks>
        /// If the exceptionMessage is null, empty, or contains only whitespace characters, an <see cref="ArgumentException"/> will be thrown.
        /// Throws the caller-supplied <typeparamref name="TException"/> when the Guard condition is true,
        /// constructed with the formatted exception message. A generic type parameter has no fixed
        /// <c>cref</c> target, so this contract is stated here in prose rather than as an
        /// <c>&lt;exception&gt;</c> tag.
        /// </remarks>
        /// <typeparam name="TException">The type of the exception to throw.</typeparam>
        /// <param name="exceptionMessage">The format string for the exception message.</param>
        /// <param name="args">The arguments to format the exception message.</param>
        /// <exception cref="FormatException">
        /// Thrown when the format string or the arguments provided are invalid.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified exception type doesn't have a constructor that accepts a single string parameter        
        /// or when the constructor fails to create a valid instance with the provided arguments.
        /// </exception>
        /// <exception cref="TargetInvocationException">
        /// Thrown when the constructor of the specified exception type throws an exception. The actual 
        /// exception thrown by the constructor can be found in the InnerException property.
        /// </exception>
        /// <example>
        /// <code>
        /// public sealed class CustomException : Exception
        /// {
        ///     public CustomException(string message)
        ///         : base(message)
        ///     {
        ///     }
        /// }
        ///
        /// class Example
        /// {
        ///     public void ProcessData(int data)
        ///     {
        ///         Guard.Against(data &lt; 0)
        ///              .With&lt;CustomException&gt;("Data cannot be negative. Value: {0}", data);
        ///
        ///         // ... rest of the method
        ///     }
        /// }
        /// </code>
        /// </example>        
        public void With<TException>(string exceptionMessage, params object[] args) where TException : Exception
        {
            // Check if the exceptionMessage is null, empty, or contains only whitespace characters.
            // Use the CallerArgumentExpression overload (no lambda compilation) since exceptionMessage
            // is a plain parameter, not an Expression.
            AgainstNullOrWhiteSpace(exceptionMessage, nameof(exceptionMessage));

            // A passing guard (condition false) must be a complete no-op. Return BEFORE formatting so a
            // malformed format string cannot throw FormatException when no violation occurred.
            if (!_condition)
            {
                return;
            }

            // Format the exception message
            string formattedMessage;
            try
            {
                formattedMessage = string.Format(CultureInfo.InvariantCulture, exceptionMessage, args);
            }
            catch (FormatException formatException)
            {
                throw new FormatException("The format string or the arguments provided are invalid.", formatException);
            }

            // Try to create an instance of the specified exception type with the formatted message
            TException exception;
            try
            {
#pragma warning disable CS8600
                exception = (TException)Activator.CreateInstance(typeof(TException), formattedMessage);
#pragma warning restore CS8600
            }
            catch (MissingMethodException)
            {
                throw new InvalidOperationException(
                    $"The exception type '{typeof(TException).FullName}' must have a constructor that accepts a single string parameter.");
            }
            catch (TargetInvocationException tie)
            {
                if (tie.InnerException is not null)
                {
                    // Preserve the constructor exception's original stack trace rather than resetting
                    // it to this re-throw site (which `throw tie.InnerException;` would do).
                    ExceptionDispatchInfo.Capture(tie.InnerException).Throw();
                }
                throw;
            }

            // Check if the created exception instance is null
            if (exception == null)
            {
                throw new InvalidOperationException(
                    $"The exception type '{typeof(TException).FullName}' failed to create a valid instance with the provided parameters.");
            }

            // Throw the created exception
            throw exception;
        }

        /// <summary>
        /// Throws the specified exception if the condition is true, constructing it from the provided
        /// constructor arguments.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If no arguments are provided, the method will attempt to use the parameterless constructor of the specified exception type.
        /// </para>
        /// <para>
        /// This method is named <c>WithArgs</c> rather than <c>With</c> to avoid an overload-resolution
        /// trap: a call such as <c>WithArgs&lt;T&gt;("msg", 1001)</c> is unambiguous, whereas the former
        /// <c>With&lt;T&gt;(params object[])</c> could never be selected for that call — C# preferred the
        /// more specific <see cref="With{TException}(string, object[])"/> (string-first) overload, so the
        /// multi-argument constructor was never reached.
        /// </para>
        /// <para>
        /// Throws the caller-supplied <typeparamref name="TException"/> when the Guard condition is true,
        /// constructed from <paramref name="args"/>. A generic type parameter has no fixed <c>cref</c>
        /// target, so this contract is stated here in prose rather than as an <c>&lt;exception&gt;</c> tag.
        /// </para>
        /// </remarks>
        /// <typeparam name="TException">The type of the exception to throw.</typeparam>
        /// <param name="args">The arguments for the exception constructor.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified exception type doesn't have a constructor that matches the provided arguments,
        /// or when the constructor fails to create a valid instance with the provided arguments.
        /// </exception>
        /// <exception cref="TargetInvocationException">
        /// Thrown when the constructor of the specified exception type throws an exception. The actual
        /// exception thrown by the constructor can be found in the InnerException property.
        /// </exception>
        /// <example>
        /// <code>
        /// public sealed class CustomException : Exception
        /// {
        ///     public CustomException(string message, int errorCode)
        ///         : base(message)
        ///     {
        ///         ErrorCode = errorCode;
        ///     }
        ///
        ///     public int ErrorCode { get; }
        /// }
        ///
        /// class Example
        /// {
        ///     public void ProcessData(int data)
        ///     {
        ///         Guard.Against(data &lt; 0)
        ///              .WithArgs&lt;CustomException&gt;("Data cannot be negative.", 1001);
        ///
        ///         // ... rest of the method
        ///     }
        /// }
        /// </code>
        /// </example>
        public void WithArgs<TException>(params object[] args) where TException : Exception
        {
            if (!_condition)
            {
                return;
            }

            // Try to create an instance of the specified exception type with the provided arguments
            TException exception;
            try
            {
#pragma warning disable CS8600
                exception = (TException)Activator.CreateInstance(typeof(TException), args);
#pragma warning restore CS8600
            }
            catch (MissingMethodException)
            {
                throw new InvalidOperationException(
                    $"The exception type '{typeof(TException).FullName}' must have a constructor that matches the provided arguments.");
            }
            catch (TargetInvocationException tie)
            {
                if (tie.InnerException is not null)
                {
                    // Preserve the constructor exception's original stack trace rather than resetting
                    // it to this re-throw site (which `throw tie.InnerException;` would do).
                    ExceptionDispatchInfo.Capture(tie.InnerException).Throw();
                }
                throw;
            }

            // Check if the created exception instance is null
            if (exception == null)
            {
                throw new InvalidOperationException(
                    $"The exception type '{typeof(TException).FullName}' failed to create a valid instance with the provided arguments.");
            }

            // Throw the created exception
            throw exception;
        }
    }

    /// <summary>
    /// Extracts the parameter value and name from a given expression.
    /// This method uses expression trees to parse and evaluate the provided lambda,
    /// enabling extraction of both the value and the name of the member (field or property) being accessed.
    /// </summary>
    /// <typeparam name="T">The type of the value expected from the expression.</typeparam>
    /// <param name="valueExpression">The expression from which to extract value and parameter name.</param>
    /// <returns>A tuple containing the value and the name of the parameter accessed in the expression.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the expression does not represent a direct access to a member or the expression is null.</exception>
    private static (T value, string paramName) ExtractParameterInfo<T>(Expression<Func<T>> valueExpression)
    {
        // Check if the provided expression is null, which is not allowed
        if (valueExpression == null)
        {
            throw new InvalidOperationException("Expression cannot be null.");
        }

        // Attempt to find a MemberExpression within the expression tree.
        // A MemberExpression represents accessing a field or property.
        var memberExpr = valueExpression.Body as MemberExpression;

        // If the body of the expression is not a MemberExpression, it means the expression does not directly access a member.
        // We throw an exception because the method is designed to work only with direct member access expressions.
        if (memberExpr == null)
        {
            throw new InvalidOperationException("Could not extract the parameter information from the expression. The expression must directly access a member (property or field).");
        }

        // Compile the expression to a delegate and invoke it to evaluate and retrieve the actual value.
        // Compiling the expression allows us to execute it and obtain the runtime value of the member it accesses.
        var compiledExpression = valueExpression.Compile();
        var value = compiledExpression();

        // Return the evaluated value along with the name of the member being accessed.
        // This name is used to identify the parameter in error messages or other logging.
        return (value, memberExpr.Member.Name);
    }

    // ─── Zero-overhead overloads using CallerArgumentExpression ───────────────
    // These overloads extract the parameter name at compile-time, avoiding the
    // expression tree compilation overhead of the Expression<Func<T>> overloads.
    // Prefer these in performance-sensitive hot paths.

    /// <summary>
    /// Checks if the given reference type value is null and throws an <see cref="ArgumentNullException"/> if it is.
    /// This zero-overhead overload extracts the parameter name at compile time.
    /// </summary>
    /// <typeparam name="T">The type of the value being checked. Must be a reference type.</typeparam>
    /// <param name="value">The value to check for null.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
    /// <example>
    /// <code>
    /// public void PrintList(List&lt;string&gt; items)
    /// {
    ///     Guard.AgainstNull(items);
    ///     foreach (var item in items) { Console.WriteLine(item); }
    /// }
    /// </code>
    /// </example>
    public static void AgainstNull<T>(T? value, [CallerArgumentExpression(nameof(value))] string paramName = "") where T : class
    {
        if (value is null)
        {
            throw new ArgumentNullException(paramName, $"Parameter '{paramName}' cannot be null.");
        }
    }

    /// <summary>
    /// Checks if the given value type is its default value and throws an <see cref="ArgumentException"/> if it is.
    /// This zero-overhead overload extracts the parameter name at compile time.
    /// </summary>
    /// <typeparam name="T">The type of the value being checked. Must be a value type.</typeparam>
    /// <param name="value">The value to check for default.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <exception cref="ArgumentException">Thrown when the value is its default value.</exception>
    public static void AgainstDefault<T>(T value, [CallerArgumentExpression(nameof(value))] string paramName = "") where T : struct
    {
        if (EqualityComparer<T>.Default.Equals(value, default))
        {
            throw new ArgumentException($"Parameter '{paramName}' cannot be the default value.", paramName);
        }
    }

    /// <summary>
    /// Checks if the given string is null or empty and throws an <see cref="ArgumentException"/> if it is.
    /// This zero-overhead overload extracts the parameter name at compile time.
    /// </summary>
    /// <param name="value">The string to check for null or empty.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <exception cref="ArgumentException">Thrown when the string is null or empty.</exception>
    public static void AgainstNullOrEmpty(string? value, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException($"Parameter '{paramName}' cannot be null or empty.", paramName);
        }
    }

    /// <summary>
    /// Checks if the given string is null, empty, or consists only of white-space characters
    /// and throws an <see cref="ArgumentException"/> if it is.
    /// This zero-overhead overload extracts the parameter name at compile time.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <exception cref="ArgumentException">Thrown when the string is null, empty, or whitespace.</exception>
    public static void AgainstNullOrWhiteSpace(string? value, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"Parameter '{paramName}' cannot be null, empty or contain only white-space characters.", paramName);
        }
    }

    /// <summary>
    /// Checks if the given value is negative and throws an <see cref="ArgumentOutOfRangeException"/> if it is.
    /// This zero-overhead overload extracts the parameter name at compile time.
    /// </summary>
    /// <typeparam name="T">The type of the value. Must be a comparable value type.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is negative.</exception>
    public static void AgainstNegative<T>(T value, [CallerArgumentExpression(nameof(value))] string paramName = "") where T : struct, IComparable<T>
    {
        if (value.CompareTo(default) < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"Parameter '{paramName}' cannot be negative.");
        }
    }

    /// <summary>
    /// Checks if the given value is negative or zero and throws an <see cref="ArgumentOutOfRangeException"/> if it is.
    /// This zero-overhead overload extracts the parameter name at compile time.
    /// </summary>
    /// <typeparam name="T">The type of the value. Must be a comparable value type.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is negative or zero.</exception>
    public static void AgainstNegativeOrZero<T>(T value, [CallerArgumentExpression(nameof(value))] string paramName = "") where T : struct, IComparable<T>
    {
        if (value.CompareTo(default) <= 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"Parameter '{paramName}' cannot be negative or zero.");
        }
    }

    /// <summary>
    /// Checks if the given value is within the specified range and throws an <see cref="ArgumentOutOfRangeException"/> if it is not.
    /// This zero-overhead overload extracts the parameter name at compile time.
    /// </summary>
    /// <typeparam name="T">The type of the value. Must be a comparable value type.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="min">The minimum valid value, inclusive.</param>
    /// <param name="max">The maximum valid value, inclusive.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is out of range.</exception>
    /// <exception cref="ArgumentException">Thrown when min is not less than max.</exception>
    public static void AgainstOutOfRange<T>(T value, T min, T max, [CallerArgumentExpression(nameof(value))] string paramName = "") where T : struct, IComparable<T>
    {
        if (min.CompareTo(max) >= 0)
        {
            throw new ArgumentException($"The value of parameter 'max' {max} must be greater than the value of parameter 'min' {min}");
        }

        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"The value of {paramName} must be between {min} and {max}.");
        }
    }

    // ─── Private helpers (used by Expression-based overloads) ──────────────────

    /// <summary>
    /// Checks if the given string is null or empty and throws an <see cref="ArgumentException"/> if it is.
    /// </summary>
    private static void ThrowIfNullOrEmpty(string value, string paramName)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException($"Parameter '{paramName}' cannot be null or empty.", paramName);
        }
    }

    /// <summary>
    /// Checks if the given string is null, empty, or consists only of white-space characters
    /// and throws an <see cref="ArgumentException"/> if it is.
    /// </summary>
    private static void ThrowIfNullOrWhiteSpace(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"Parameter '{paramName}' cannot be null, empty or contain only white-space characters.", paramName);
        }
    }

    /// <summary>
    /// Checks if the given string exceeds the specified maximum length
    /// and throws an <see cref="ArgumentOutOfRangeException"/> if it does.
    /// </summary>
    private static void ThrowIfOverflow(string value, string paramName, int maxLength)
    {
        if (value.Length > maxLength)
        {
            throw new ArgumentOutOfRangeException(paramName, $"The length of the parameter '{paramName}' cannot exceed {maxLength} characters.");
        }
    }
}
