// Copyright (c) 2024 Pierre G. Boutquin. All rights reserved.
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

namespace Boutquin.Domain.Abstractions;

/// <summary>
/// Represents the outcome of an operation, indicating whether it was successful or resulted in a failure.
/// </summary>
/// <remarks>
/// <para>
/// The Result class is a type that represents either success or failure of an operation. It can be used as a return type
/// in methods to provide more information than just a boolean value, encapsulating an error in case of failure.
/// </para>
/// <para>
/// This class allows methods to return a result without throwing exceptions for expected or non-critical failures,
/// thus supporting a more functional approach to error handling.
/// </para>
/// <para>
/// The class also provides factory methods to conveniently create instances representing success or failure.
/// </para>
/// <para>
/// The generic variant <see cref="Result{TValue}"/> extends this functionality for operations that return a value.
/// </para>
/// </remarks>
/// <example>
/// Example of using the Result class:
/// <code>
/// public Result PerformOperation()
/// {
///     try
///     {
///         // Operation logic
///         return Result.Success();
///     }
///     catch (Exception ex)
///     {
///         return Result.Failure(new Error("OperationFailed", ex.Message));
///     }
/// }
/// 
/// // Usage of the method
/// var result = PerformOperation();
/// if (result.IsSuccess)
/// {
///     Console.WriteLine("Operation succeeded.");
/// }
/// else
/// {
///     Console.WriteLine($"Operation failed: {result.Error.Name}");
/// }
/// </code>
/// </example>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="error">The error associated with a failed operation.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if an error is provided for a successful result or no error is provided for a failed result.
    /// </exception>
    protected Result(bool isSuccess, Error error)
    {
        switch (isSuccess)
        {
            case true when error != Error.None:
                throw new InvalidOperationException("A successful result should not have an error.");
            case false when error == Error.None:
                throw new InvalidOperationException("A failed result must have an error.");
            default:
                IsSuccess = isSuccess;
                Error = error;
                break;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error associated with the operation failure.
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// Creates a success result.
    /// </summary>
    /// <returns>A successful result.</returns>
    public static Result Success()
        => new(true, Error.None);

    /// <summary>
    /// Creates a failure result with the specified error.
    /// </summary>
    /// <param name="error">The error associated with the failure.</param>
    /// <returns>A failed result with the given error.</returns>
    public static Result Failure(Error error)
        => new(false, error);

    /// <summary>
    /// Creates a success result with the specified value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be included in the result.</typeparam>
    /// <param name="value">The value to be included in the result.</param>
    /// <returns>A <see cref="Result{TValue}"/> instance representing a successful operation with the specified value.</returns>
    /// <remarks>
    /// This method is used to create a success result for operations that also produce a value.
    /// It is particularly useful in scenarios where the method needs to return both the outcome of the operation (success)
    /// and the result of the operation (the value).
    /// </remarks>
    /// <example>
    /// <code>
    /// public Result&lt;int&gt; CalculateSum(int a, int b)
    /// {
    ///     return Result.Success(a + b);
    /// }
    /// </code>
    /// </example>
    public static Result<TValue> Success<TValue>(TValue value)
        => new(value, true, Error.None);

    /// <summary>
    /// Creates a failure result with the specified error for operations that also produce a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value that would have been produced if the operation had been successful.</typeparam>
    /// <param name="error">The error associated with the failure.</param>
    /// <returns>A <see cref="Result{TValue}"/> instance representing a failed operation with the specified error.</returns>
    /// <remarks>
    /// This method is used to create a failure result for operations that are expected to produce a value.
    /// It allows the method to return a uniform type (<see cref="Result{TValue}"/>) regardless of the outcome.
    /// The TValue type parameter in this context is typically used for type consistency in methods that may return a value.
    /// </remarks>
    /// <example>
    /// <code>
    /// public Result&lt;int&gt; Divide(int numerator, int denominator)
    /// {
    ///     if (denominator == 0)
    ///     {
    ///         return Result.Failure&lt;int&gt;(new Error("DivideByZero", "Denominator cannot be zero"));
    ///     }
    ///     return Result.Success(numerator / denominator);
    /// }
    /// </code>
    /// </example>
    public static Result<TValue> Failure<TValue>(Error error)
        => new(default, false, error);

    /// <summary>
    /// Creates a result based on the provided value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be included in the result.</typeparam>
    /// <param name="value">The value to be included in the result. Can be null.</param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> instance representing a successful operation if the value is not null;
    /// otherwise, a failed result with a default error indicating that a null value was provided.
    /// </returns>
    /// <remarks>
    /// This method simplifies the creation of a <see cref="Result{TValue}"/> instance by automatically
    /// determining the success or failure state based on the nullability of the provided value.
    /// It is particularly useful in scenarios where the presence of a value indicates success and
    /// its absence (null) indicates failure.
    /// </remarks>
    /// <example>
    /// <code>
    /// public Result&lt;string&gt; GetUserName(int userId)
    /// {
    ///     string userName = GetUserFromDatabase(userId); // Assume this method returns null if user not found
    ///     return Result.Create(userName);
    ///     // Returns a success result with the username if found, otherwise a failure result.
    /// }
    /// </code>
    /// </example>
    public static Result<TValue> Create<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}
