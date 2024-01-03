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

/// <summary>
/// Represents an error within the domain.
/// </summary>
/// <param name="Code">A unique error code identifying the type of error.</param>
/// <param name="Name">A descriptive name for the error.</param>
/// <remarks>
/// <para>
/// The Error record is used to encapsulate information about errors that occur within the domain.
/// Each error is identified by a unique code and a human-readable name that describes the error.
/// This record is immutable, meaning that once an error is created, it cannot be modified.
/// </para>
/// <para>
/// The Error record is often used in scenarios where exceptions are not suitable or where
/// non-exceptional error handling is preferred. It can be returned from methods to indicate
/// that an error occurred, allowing the caller to handle the error appropriately.
/// </para>
/// <para>
/// The record includes predefined static instances for common error scenarios, such as
/// 'None' for no error and 'NullValue' for null value errors.
/// </para>
/// </remarks>
/// <example>
/// Example usage:
/// <code>
/// Error myError = Error.NullValue;
/// if (someCondition)
/// {
///     myError = new Error("Error.Custom", "A custom error occurred");
/// }
/// </code>
/// </example>
public record Error(string Code, string Name)
{
    /// <summary>
    /// Represents no error.
    /// </summary>
    /// <remarks>
    /// This static member can be used to represent a lack of errors, or a successful operation.
    /// </remarks>
    public static Error None = new(string.Empty, string.Empty);

    /// <summary>
    /// Represents an error caused by a null value.
    /// </summary>
    /// <remarks>
    /// This static member can be used when a null value is provided where it is not allowed.
    /// </remarks>
    public static Error NullValue = new("Error.NullValue", "Null value was provided");
}
