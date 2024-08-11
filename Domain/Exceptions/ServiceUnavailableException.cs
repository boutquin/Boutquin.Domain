// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Boutquin.Domain.Exceptions;

/// <summary>
/// The exception that is thrown when the server is temporarily unable to handle the request.
/// </summary>
[Serializable]
public sealed class ServiceUnavailableException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceUnavailableException"/> class.
    /// </summary>
    public ServiceUnavailableException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceUnavailableException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public ServiceUnavailableException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceUnavailableException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="inner">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public ServiceUnavailableException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
