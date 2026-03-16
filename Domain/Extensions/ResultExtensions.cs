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

using System;
using System.Threading.Tasks;
using Abstractions;

/// <summary>
/// Provides extension methods for the Result type.
/// This class enhances the functionality of the Result type, allowing for more expressive and concise handling
/// of success and failure scenarios.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Executes one of two provided functions based on the success or failure of the Result.
    /// </summary>
    /// <typeparam name="T">The type of the return value of the functions.</typeparam>
    /// <param name="result">The Result instance on which the extension method is called.</param>
    /// <param name="onSuccess">The function to execute if the Result indicates success.</param>
    /// <param name="onFailure">The function to execute if the Result indicates failure, taking an Error as a parameter.</param>
    /// <returns>The return value of either the onSuccess or onFailure function.</returns>
    /// <example>
    /// // Example usage of the Match method:
    /// var myResult = PerformOperation();
    /// var message = myResult.Match(
    ///     onSuccess: () => "Operation succeeded.",
    ///     onFailure: error => $"Operation failed: {error.Name}");
    /// // The variable 'message' will contain a success or failure message based on the operation result.
    /// </example>
    public static T Match<T>(
        this Result result,
        Func<T> onSuccess,
        Func<Error, T> onFailure) =>
        result.IsSuccess ? onSuccess() : onFailure(result.Error);

    /// <summary>
    /// Executes one of two provided functions based on the success or failure of a Result that carries a value.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the functions.</typeparam>
    /// <typeparam name="TValue">The type of the value contained in the Result.</typeparam>
    /// <param name="result">The Result&lt;TValue&gt; instance on which the extension method is called.</param>
    /// <param name="onSuccess">The function to execute if the Result indicates success, receiving the value as a parameter.</param>
    /// <param name="onFailure">The function to execute if the Result indicates failure, taking an Error as a parameter.</param>
    /// <returns>The return value of either the onSuccess or onFailure function.</returns>
    /// <example>
    /// <code>
    /// var result = Divide(10, 2);
    /// var message = result.Match(
    ///     onSuccess: value => $"Result: {value}",
    ///     onFailure: error => $"Error: {error.Name}");
    /// </code>
    /// </example>
    public static TResult Match<TResult, TValue>(
        this Result<TValue> result,
        Func<TValue, TResult> onSuccess,
        Func<Error, TResult> onFailure) =>
        result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);

    /// <summary>
    /// Asynchronously evaluates a Result and returns a value based on success or failure.
    /// </summary>
    /// <typeparam name="T">The type of the return value.</typeparam>
    /// <param name="resultTask">The asynchronous Result to evaluate.</param>
    /// <param name="onSuccess">The function to execute if the Result indicates success.</param>
    /// <param name="onFailure">The function to execute if the Result indicates failure.</param>
    /// <returns>The return value of either the onSuccess or onFailure function.</returns>
    public static async ValueTask<T> MatchAsync<T>(
        this ValueTask<Result> resultTask,
        Func<Task<T>> onSuccess,
        Func<Error, Task<T>> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        var result = await resultTask.ConfigureAwait(false);
        return result.IsSuccess
            ? await onSuccess().ConfigureAwait(false)
            : await onFailure(result.Error).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously evaluates a Result{TValue} and returns a value based on success or failure.
    /// </summary>
    /// <typeparam name="T">The type of the return value.</typeparam>
    /// <typeparam name="TValue">The type of the value contained in the Result.</typeparam>
    /// <param name="resultTask">The asynchronous Result to evaluate.</param>
    /// <param name="onSuccess">The function to execute if the Result indicates success, receiving the value.</param>
    /// <param name="onFailure">The function to execute if the Result indicates failure.</param>
    /// <returns>The return value of either the onSuccess or onFailure function.</returns>
    public static async ValueTask<T> MatchAsync<T, TValue>(
        this ValueTask<Result<TValue>> resultTask,
        Func<TValue, Task<T>> onSuccess,
        Func<Error, Task<T>> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        var result = await resultTask.ConfigureAwait(false);
        return result.IsSuccess
            ? await onSuccess(result.Value).ConfigureAwait(false)
            : await onFailure(result.Error).ConfigureAwait(false);
    }

    /// <summary>
    /// Transforms the value of a successful Result into a new Result with a different value type.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <param name="result">The Result to transform.</param>
    /// <param name="func">The transformation function to apply to the value.</param>
    /// <returns>A new Result containing the transformed value, or the original error on failure.</returns>
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> func)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(func);

        return result.IsSuccess
            ? Result.Success(func(result.Value))
            : Result.Failure<TOut>(result.Error);
    }

    /// <summary>
    /// Chains a Result-returning operation onto a successful Result.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <param name="result">The Result to bind.</param>
    /// <param name="func">The function that returns a new Result.</param>
    /// <returns>The Result of the chained operation, or the original error on failure.</returns>
    public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> func)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(func);

        return result.IsSuccess
            ? func(result.Value)
            : Result.Failure<TOut>(result.Error);
    }

    /// <summary>
    /// Asynchronously chains a Result-returning operation onto a successful Result.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <param name="resultTask">The asynchronous Result to bind.</param>
    /// <param name="func">The async function that returns a new Result.</param>
    /// <returns>The Result of the chained operation, or the original error on failure.</returns>
    public static async ValueTask<Result<TOut>> BindAsync<TIn, TOut>(
        this Task<Result<TIn>> resultTask,
        Func<TIn, Task<Result<TOut>>> func)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        ArgumentNullException.ThrowIfNull(func);

        var result = await resultTask.ConfigureAwait(false);
        return result.IsSuccess
            ? await func(result.Value).ConfigureAwait(false)
            : Result.Failure<TOut>(result.Error);
    }

    /// <summary>
    /// Executes a side-effect action on a successful Result without altering the result.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the Result.</typeparam>
    /// <param name="result">The Result to tap.</param>
    /// <param name="action">The action to execute on the value if the Result is successful.</param>
    /// <returns>The original Result, unchanged.</returns>
    public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(action);

        if (result.IsSuccess)
        {
            action(result.Value);
        }

        return result;
    }

    /// <summary>
    /// Asynchronously executes a side-effect action on a successful Result.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the Result.</typeparam>
    /// <param name="resultTask">The asynchronous Result to tap.</param>
    /// <param name="action">The async action to execute on the value if the Result is successful.</param>
    /// <returns>The original Result, unchanged.</returns>
    public static async ValueTask<Result<T>> TapAsync<T>(
        this ValueTask<Result<T>> resultTask,
        Func<T, Task> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        var result = await resultTask.ConfigureAwait(false);
        if (result.IsSuccess)
        {
            await action(result.Value).ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// Executes a side-effect action on a failed Result without altering the result.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the Result.</typeparam>
    /// <param name="result">The Result to inspect.</param>
    /// <param name="action">The action to execute on the error if the Result is a failure.</param>
    /// <returns>The original Result, unchanged.</returns>
    public static Result<T> OnFailure<T>(this Result<T> result, Action<Error> action)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(action);

        if (!result.IsSuccess)
        {
            action(result.Error);
        }

        return result;
    }
}
