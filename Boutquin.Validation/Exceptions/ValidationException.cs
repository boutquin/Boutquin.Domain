// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Boutquin.Validation.Exceptions;

using Domain.Helpers;
using FluentValidation.Results;

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
        => string.Join(Environment.NewLine, failures.Select(f => f.ErrorMessage));
}
