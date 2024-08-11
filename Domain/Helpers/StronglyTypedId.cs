// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Boutquin.Domain.Helpers;

/// <summary>
/// Provides a base record for creating strongly typed identifiers across the domain.
/// This abstraction is meant to enhance type safety and clarity within the codebase
/// by wrapping primitive types used for identifiers (e.g., Guid, int) in more descriptive types.
/// </summary>
/// <typeparam name="TValue">The underlying value type of the identifier (e.g., Guid, int).</typeparam>
/// <remarks>
/// The StronglyTypedId record enforces that the TValue must be non-nullable, ensuring
/// that identifiers cannot be unintentionally missing or set to a default null state.
/// </remarks>
/// <example>
/// // Example of defining a strongly typed identifier for a User entity
/// public record UserId(Guid Value) : StronglyTypedId&lt;Guid&gt;(Value);
///
/// // Using the strongly typed ID:
/// var userId = new UserId(Guid.NewGuid());
/// Console.WriteLine(userId); // Outputs: {guid}
/// </example>
public abstract record StronglyTypedId<TValue>(TValue Value)
    where TValue : notnull
{
    /// <summary>
    /// Provides a simplified string representation of the strongly typed ID, returning only its value.
    /// This override focuses on representing the identifier's value directly, making it ideal for logging,
    /// debugging, or displaying the identifier in a user interface.
    /// </summary>
    /// <returns>A string that represents the value of the strongly typed ID.</returns>
    public sealed override string ToString()
        => $"{Value}";
}
