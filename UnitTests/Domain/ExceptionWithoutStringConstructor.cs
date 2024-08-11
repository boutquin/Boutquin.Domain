// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Boutquin.UnitTests.Domain;

/// <summary>
/// Represents an exception without a constructor accepting a single string parameter, used for testing purposes in the GuardTests class.
/// </summary>
public sealed class ExceptionWithoutStringConstructor : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionWithoutStringConstructor"/> class.
    /// </summary>
    public ExceptionWithoutStringConstructor()
    {
    }
}
