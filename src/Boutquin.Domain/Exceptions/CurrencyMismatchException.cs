// Copyright (c) 2024-2026 Pierre G. Boutquin. All rights reserved.
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
//
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

namespace Boutquin.Domain.Exceptions;

using Boutquin.Domain.Enumerations;

/// <summary>
/// The exception that is thrown when an arithmetic or ordering operation is attempted between two
/// <see cref="Boutquin.Domain.ValueObjects.Money"/> values denominated in different
/// <see cref="Currency"/> values.
/// </summary>
/// <remarks>
/// <para><b>Why it throws rather than returns a failed result:</b> mixing currencies in
/// <c>+</c>, <c>-</c>, or an ordering comparison is a programming error, not an expected domain
/// failure. There is no meaningful result to return, so the operation fails fast with this
/// exception instead of silently coercing one currency into another. Expected, recoverable
/// failures elsewhere in the library are modeled with <see cref="Boutquin.Domain.Abstractions.Result"/>;
/// a currency mismatch is not one of them.</para>
/// <para><b>Equality is exempt:</b> comparing two <see cref="Boutquin.Domain.ValueObjects.Money"/>
/// values for equality never throws — money in different currencies is simply not equal. Only
/// operations that must agree on a currency to be meaningful (addition, subtraction, ordering)
/// raise this exception.</para>
/// <para><b>Common mistake:</b> adding amounts pulled from different sources without first
/// converting them to a common currency. Convert explicitly (via an exchange-rate operation in a
/// higher layer) before combining.</para>
/// </remarks>
public sealed class CurrencyMismatchException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CurrencyMismatchException"/> class for an
    /// operation attempted between two differing currencies, building a message that names both.
    /// </summary>
    /// <param name="expected">The currency of the left/target operand — the currency the operation
    /// required the other operand to share.</param>
    /// <param name="actual">The currency of the right/other operand that did not match
    /// <paramref name="expected"/>.</param>
    public CurrencyMismatchException(Currency expected, Currency actual)
        : base($"This operation cannot be performed between {expected} and {actual}.")
    {
        Expected = expected;
        Actual = actual;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrencyMismatchException"/> class with a
    /// specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public CurrencyMismatchException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrencyMismatchException"/> class with a
    /// specified error message and a reference to the inner exception that is the cause of this
    /// exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="inner">The exception that is the cause of the current exception, or a null
    /// reference if no inner exception is specified.</param>
    public CurrencyMismatchException(string message, Exception inner)
        : base(message, inner)
    {
    }

    /// <summary>
    /// The currency of the left/target operand — the currency the failed operation required both
    /// operands to share.
    /// </summary>
    /// <value>The <see cref="Currency"/> of the first operand, or <see cref="Currency.Unspecified"/>
    /// when the exception was constructed from a message rather than a currency pair.</value>
    public Currency Expected { get; }

    /// <summary>
    /// The currency of the right/other operand that did not match <see cref="Expected"/>.
    /// </summary>
    /// <value>The <see cref="Currency"/> of the second operand, or <see cref="Currency.Unspecified"/>
    /// when the exception was constructed from a message rather than a currency pair.</value>
    public Currency Actual { get; }
}
