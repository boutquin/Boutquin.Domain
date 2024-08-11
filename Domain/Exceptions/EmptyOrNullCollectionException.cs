// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Boutquin.Domain.Exceptions;

/// <summary>
/// The exception that is thrown when a provided collection is null or empty.
/// </summary>
[Serializable]
public sealed class EmptyOrNullCollectionException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmptyOrNullCollectionException"/> class.
    /// </summary>
    public EmptyOrNullCollectionException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmptyOrNullCollectionException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public EmptyOrNullCollectionException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmptyOrNullCollectionException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="inner">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public EmptyOrNullCollectionException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
