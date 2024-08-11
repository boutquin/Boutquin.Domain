// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
    public static readonly Error None = new(string.Empty, string.Empty);

    /// <summary>
    /// Represents an error caused by a null value.
    /// </summary>
    /// <remarks>
    /// This static member can be used when a null value is provided where it is not allowed.
    /// </remarks>
    public static readonly Error NullValue = new("Error.NullValue", "Null value was provided");
}
