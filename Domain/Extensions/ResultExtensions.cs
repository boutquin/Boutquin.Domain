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

namespace Boutquin.Domain.Extensions;

using System;
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
    ///     onFailure: error => $"Operation failed: {error.Message}");
    /// // The variable 'message' will contain a success or failure message based on the operation result.
    /// </example>
    public static T Match<T>(
        this Result result,
        Func<T> onSuccess,
        Func<Error, T> onFailure) =>
        result.IsSuccess ? onSuccess() : onFailure(result.Error);
}
