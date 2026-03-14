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

namespace Boutquin.Domain.Exceptions;

/// <summary>
/// Base class for domain exceptions that map to specific HTTP status codes.
/// </summary>
/// <remarks>
/// Exceptions deriving from this class carry their HTTP status code and title,
/// allowing middleware to handle them generically without a per-type switch statement.
/// </remarks>
public abstract class DomainException : Exception
{
    /// <summary>
    /// Gets the HTTP status code associated with this exception.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Gets the human-readable title for the error response.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="title">The human-readable title.</param>
    protected DomainException(int statusCode, string title)
    {
        StatusCode = statusCode;
        Title = title;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class with a message.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="title">The human-readable title.</param>
    /// <param name="message">The error message.</param>
    protected DomainException(int statusCode, string title, string message)
        : base(message)
    {
        StatusCode = statusCode;
        Title = title;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class with a message and inner exception.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="title">The human-readable title.</param>
    /// <param name="message">The error message.</param>
    /// <param name="inner">The inner exception.</param>
    protected DomainException(int statusCode, string title, string message, Exception inner)
        : base(message, inner)
    {
        StatusCode = statusCode;
        Title = title;
    }
}
