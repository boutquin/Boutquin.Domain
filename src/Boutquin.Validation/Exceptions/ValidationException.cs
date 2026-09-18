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

namespace Boutquin.Validation.Exceptions;

using FluentValidation.Results;

/// <summary>
/// The exception that is thrown when validation fails in the FluentValidation library.
/// </summary>
public sealed class ValidationException : Exception
{
    /// <summary>
    /// Gets the collection of validation failures.
    /// </summary>
    /// <remarks>
    /// The failures are materialized into a private list at construction time, so this collection
    /// is safe to enumerate repeatedly and is not affected by mutation of the caller's source.
    /// </remarks>
    public IReadOnlyList<ValidationFailure> Errors { get; }

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
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="failures"/> is <c>null</c>.</exception>
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this(Materialize(failures))
    {
    }

    /// <summary>
    /// Private constructor that receives an already-materialized, non-null failure list. Building
    /// the base message from the materialized list (rather than the raw <c>IEnumerable</c>) is what
    /// guarantees the source sequence is enumerated exactly once and that a <c>null</c> argument is
    /// surfaced as <see cref="ArgumentNullException"/> by <see cref="Materialize"/> before any
    /// enumeration is attempted.
    /// </summary>
    /// <param name="failures">The materialized, non-null collection of validation failures.</param>
    private ValidationException(IReadOnlyList<ValidationFailure> failures)
        : base(BuildErrorMessage(failures))
    {
        Errors = failures;
    }

    /// <summary>
    /// Validates that <paramref name="failures"/> is not <c>null</c> and materializes it into a
    /// private list, enumerating the source exactly once.
    /// </summary>
    /// <param name="failures">The collection of validation failures supplied by the caller.</param>
    /// <returns>A private, non-null list copy of <paramref name="failures"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="failures"/> is <c>null</c>.</exception>
    private static IReadOnlyList<ValidationFailure> Materialize(IEnumerable<ValidationFailure> failures)
    {
        ArgumentNullException.ThrowIfNull(failures);
        return [.. failures];
    }

    /// <summary>
    /// Builds an error message by concatenating the error messages of the validation failures.
    /// </summary>
    /// <param name="failures">The collection of validation failures.</param>
    /// <returns>A string containing the concatenated error messages.</returns>
    private static string BuildErrorMessage(IReadOnlyList<ValidationFailure> failures)
        => string.Join(Environment.NewLine, failures.Select(f => f.ErrorMessage));
}
