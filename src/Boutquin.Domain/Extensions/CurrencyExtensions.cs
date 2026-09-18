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

namespace Boutquin.Domain.Extensions;

using Boutquin.Domain.Enumerations;
using Boutquin.Domain.ValueObjects;

/// <summary>
/// Extension methods for <see cref="Currency"/> that read as a fluent factory for
/// <see cref="Money"/>.
/// </summary>
/// <seealso cref="Money"/>
public static class CurrencyExtensions
{
    /// <summary>
    /// Creates a <see cref="Money"/> in this currency with the supplied amount, so a call site reads
    /// as <c>Currency.USD.Amount(19.99m)</c>.
    /// </summary>
    /// <param name="currency">The denomination of the resulting money. Must be a defined
    /// <see cref="Currency"/> value other than <see cref="Currency.Unspecified"/>.</param>
    /// <param name="amount">The monetary amount.</param>
    /// <returns>A <see cref="Money"/> with <paramref name="amount"/> denominated in
    /// <paramref name="currency"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="currency"/> is
    /// <see cref="Currency.Unspecified"/> or is not a defined <see cref="Currency"/> value.</exception>
    /// <example>
    /// <code>
    /// var price = Currency.EUR.Amount(42.50m); // 42.50 EUR
    /// </code>
    /// </example>
    public static Money Amount(this Currency currency, decimal amount)
    {
        if (currency == Currency.Unspecified || !Enum.IsDefined(currency))
        {
            throw new ArgumentOutOfRangeException(
                nameof(currency),
                currency,
                "Money requires a defined ISO 4217 currency other than Currency.Unspecified.");
        }

        return new Money(amount, currency);
    }
}
