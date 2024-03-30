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
