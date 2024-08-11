// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Boutquin.Domain.Exceptions;

/// <summary>
/// Contains constants for exception messages.
/// </summary>
public static class ExceptionMessages
{
    public const string EmptyOrNullArray = "Input array must not be empty or null.";
    public const string InsufficientDataForSampleCalculation = "Input array must have at least two elements for sample calculation.";
}
