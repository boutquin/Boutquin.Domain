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

namespace Boutquin.Domain.Abstractions;

/// <summary>
/// Represents an error within the domain.
/// </summary>
/// <param name="Code">A unique error code identifying the type of error.</param>
/// <param name="Name">A descriptive name for the error.</param>
/// <param name="ErrorType">The category of the error, mapped to an HTTP status code. Defaults to <see cref="ErrorType.None"/>.</param>
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
public record Error(string Code, string Name, ErrorType ErrorType = default)
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

    /// <summary>Creates a <see cref="ErrorType.BadRequest"/> (400) error.</summary>
    public static Error BadRequest(string code, string name) => new(code, name, ErrorType.BadRequest);

    /// <summary>Creates a <see cref="ErrorType.NotFound"/> (404) error.</summary>
    public static Error NotFound(string code, string name) => new(code, name, ErrorType.NotFound);

    /// <summary>Creates a <see cref="ErrorType.Conflict"/> (409) error.</summary>
    public static Error Conflict(string code, string name) => new(code, name, ErrorType.Conflict);

    /// <summary>Creates a <see cref="ErrorType.Unauthorized"/> (401) error.</summary>
    public static Error Unauthorized(string code, string name) => new(code, name, ErrorType.Unauthorized);

    /// <summary>Creates a <see cref="ErrorType.Forbidden"/> (403) error.</summary>
    public static Error Forbidden(string code, string name) => new(code, name, ErrorType.Forbidden);

    /// <summary>Creates a <see cref="ErrorType.RequestTimeout"/> (408) error.</summary>
    public static Error RequestTimeout(string code, string name) => new(code, name, ErrorType.RequestTimeout);

    /// <summary>Creates a <see cref="ErrorType.InternalServerError"/> (500) error.</summary>
    public static Error InternalServerError(string code, string name) => new(code, name, ErrorType.InternalServerError);
}
