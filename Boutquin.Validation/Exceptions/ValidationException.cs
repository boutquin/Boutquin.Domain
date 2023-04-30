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

using System;
using Boutquin.Domain.Helpers;
using FluentValidation.Results;

namespace Boutquin.Validation.Exceptions;

/// <summary>
/// The exception that is thrown when validation fails in the FluentValidation library.
/// </summary>
public sealed class ValidationException : Exception
{
    /// <summary>
    /// Gets the collection of validation failures.
    /// </summary>
    public IEnumerable<ValidationFailure> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with a collection of validation failures.
    /// </summary>
    /// <param name="failures">The collection of validation failures.</param>
    /// <example>
    /// <code>
    /// var validationResult = validator.Validate(user);
    /// if (!validationResult.IsValid)
    /// {
    ///     throw new ValidationException(validationResult.Errors);
    /// }
    /// </code>
    /// </example>
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base(BuildErrorMessage(failures))
    {
        Guard.AgainstNull(() => failures);

        Errors = failures;
    }

    /// <summary>
    /// Builds an error message by concatenating the error messages of the validation failures.
    /// </summary>
    /// <param name="failures">The collection of validation failures.</param>
    /// <returns>A string containing the concatenated error messages.</returns>
    private static string BuildErrorMessage(IEnumerable<ValidationFailure> failures)
    {
        return string.Join(Environment.NewLine, failures.Select(f => f.ErrorMessage));
    }
}
