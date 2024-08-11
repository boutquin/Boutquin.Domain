// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Boutquin.UnitTests.Domain;

/// <summary>
/// Represents a custom exception used for testing purposes in the GuardTests class.
/// </summary>
public sealed class CustomException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not null, the current exception is raised in a catch block that handles the inner exception.</param>
    public CustomException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
