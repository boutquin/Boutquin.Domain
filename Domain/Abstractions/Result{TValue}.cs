// Copyright (c) 2023-2024 Pierre G. Boutquin. All rights reserved.
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
namespace Boutquin.Domain.Abstractions;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents the outcome of an operation that returns a value, which can be either success or failure.
/// </summary>
/// <typeparam name="TValue">The type of value returned in the case of a successful operation.</typeparam>
/// <remarks>
/// <para>
/// The Result&lt;TValue&gt; class is an extension of the <see cref="Result"/> class for operations that return a value. 
/// In addition to indicating success or failure, this class encapsulates a value that is returned when the operation is successful.
/// </para>
/// <para>
/// This class should be used as a return type in methods where you need to return a value and also indicate the success or failure status of the operation.
/// </para>
/// <para>
/// The class provides a property <c>Value</c> to access the returned value. It ensures that the value can only be accessed if the operation was successful,
/// otherwise, an <see cref="InvalidOperationException"/> is thrown. This behavior enforces the check of the operation status before attempting to access the result.
/// </para>
/// <para>
/// The class also includes an implicit conversion operator from <c>TValue?</c> to <c>Result&lt;TValue&gt;</c> for convenience, allowing a value to be directly returned
/// as a successful result.
/// </para>
/// </remarks>
/// <example>
/// This example demonstrates how to use the Result&lt;TValue&gt; class to attempt parsing a string into an integer:
/// <code>
/// public static Result&lt;int&gt; TryParseInt(string input)
/// {
///     if (int.TryParse(input, out int result))
///     {
///         return Result&lt;int&gt;.Success(result);
///     }
/// 
///     return Result&lt;int&gt;.Failure(new Error("ParseError", "Failed to parse input as integer."));
/// }
/// 
/// // Usage
/// var result = TryParseInt("123");
/// if (result.IsSuccess)
/// {
///     Console.WriteLine($"Parsed value: {result.Value}");
/// }
/// else
/// {
///     Console.WriteLine($"Error: {result.Error.Name}");
/// }
/// </code>
/// </example>
public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class.
    /// </summary>
    /// <param name="value">The value associated with a successful operation.</param>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="error">The error associated with a failed operation.</param>
    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error) =>
        _value = value;

    /// <summary>
    /// Gets the value of the operation if it was successful.
    /// </summary>
    /// <value>
    /// The value of the operation.
    /// </value>
    /// <exception cref="InvalidOperationException">
    /// Thrown if attempting to access the value of a failed result.
    /// </exception>
    [NotNull]
    public TValue Value 
        => IsSuccess
            ? _value!
            : throw new InvalidOperationException("The value of a failure result cannot be accessed.");

    /// <summary>
    /// Implicitly converts a value to a successful Result&lt;TValue&gt;.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <remarks>
    /// This conversion simplifies returning a successful result from methods.
    /// </remarks>
    public static implicit operator Result<TValue>(TValue? value) => Create(value);
}
