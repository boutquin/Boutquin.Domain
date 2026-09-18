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

namespace Boutquin.Domain.ValueObjects;

using System.Globalization;
using System.Text.Json.Serialization;

using Boutquin.Domain.Enumerations;
using Boutquin.Domain.Exceptions;

// Money is a foundation value object: an amount paired with its denomination so that no Boutquin.*
// library ever passes a bare decimal whose currency lives only in a variable name. It lives in
// Boutquin.Domain so pricing, market-data, trading, and tax components share one canonical money
// type with currency-safe arithmetic.

/// <summary>
/// An immutable amount of money in a single <see cref="Currency"/>. Arithmetic and ordering that
/// would mix currencies fail fast with <see cref="CurrencyMismatchException"/>; equality never
/// throws — money in different currencies is simply not equal.
/// </summary>
/// <remarks>
/// <para><b>Pattern:</b> Value Object — defined entirely by its attributes, compared structurally,
/// and immutable. It carries no identity of its own. <b>Reference:</b> Value Object (Evans,
/// <i>Domain-Driven Design</i>); the Money type (Fowler, <i>Patterns of Enterprise Application
/// Architecture</i>).</para>
/// <para><b>Equality:</b> structural over (<see cref="Amount"/>, <see cref="Currency"/>),
/// synthesized by the record struct. <c>5 USD</c> equals <c>5 USD</c> and does <i>not</i> equal
/// <c>5 EUR</c> — comparing different currencies for equality returns <see langword="false"/>, it
/// never throws. This is the deliberate distinction from currency-sensitive <i>ordering</i>
/// (<c>&lt;</c>, <c>&gt;</c>, <see cref="CompareTo(Money)"/>), which does throw on a mismatch
/// because there is no meaningful order between two currencies.</para>
/// <para><b>Currency safety:</b> A value created by the parameterized constructor always has a defined
/// ISO 4217 denomination other than <see cref="Currency.Unspecified"/>. <c>+</c> and <c>-</c> between two <see cref="Money"/> values, and
/// every ordering comparison between them, require the same <see cref="Currency"/> and throw
/// <see cref="CurrencyMismatchException"/> otherwise. Scaling by a <see cref="decimal"/>
/// (<c>*</c>, <c>/</c>) and adding/subtracting a bare <see cref="decimal"/> keep the existing currency.
/// Because .NET permits <see langword="default"/>(<see cref="Money"/>), currency-dependent operations
/// on a default value throw <see cref="InvalidOperationException"/>. Structural equality and hashing
/// remain available for default values.</para>
/// <para><b>Why decimal:</b> monetary amounts are exact base-10 quantities; <see cref="decimal"/>
/// avoids the binary-floating-point representation error of <see cref="double"/>. This type does
/// not impose a minor-unit scale — a consumer that needs cents-precision quantization calls
/// <see cref="Round(Money,int)"/> or <see cref="Round(Money,int,MidpointRounding)"/> at its boundary.</para>
/// <para><b>Concurrency:</b> immutable; freely shareable across threads.</para>
/// </remarks>
/// <example>
/// <code>
/// var price = new Money(19.99m, Currency.USD);
/// var withTax = price * 1.13m;                 // 22.5887 USD (same currency)
/// var total = withTax + new Money(5m, Currency.USD);
/// var rounded = Money.Round(total, 2);          // 27.59 USD
///
/// // Mixing currencies in arithmetic throws:
/// // var bad = price + new Money(5m, Currency.EUR); // CurrencyMismatchException
///
/// // Equality across currencies is false, not an exception:
/// bool same = new Money(5m, Currency.USD) == new Money(5m, Currency.EUR); // false
/// </code>
/// </example>
/// <seealso cref="Currency"/>
/// <seealso cref="CurrencyMismatchException"/>
/// <seealso cref="Boutquin.Domain.Extensions.CurrencyExtensions"/>
public readonly record struct Money
    : IComparable<Money>, IComparable
{
    /// <summary>
    /// Gets the monetary amount.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Gets the ISO 4217 denomination.
    /// </summary>
    public Currency Currency { get; }

    /// <summary>
    /// Initializes a monetary amount in a defined ISO 4217 denomination.
    /// </summary>
    /// <param name="Amount">The monetary amount.</param>
    /// <param name="Currency">The denomination, which must be defined and not
    /// <see cref="Currency.Unspecified"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="Currency"/> is
    /// <see cref="Currency.Unspecified"/> or is not a defined <see cref="Currency"/> value.</exception>
    [JsonConstructor]
    public Money(decimal Amount, Currency Currency)
    {
        ValidateCurrencyArgument(Currency, nameof(Currency));
        this.Amount = Amount;
        this.Currency = Currency;
    }

    /// <summary>
    /// Deconstructs this value into its amount and denomination.
    /// </summary>
    /// <param name="Amount">The monetary amount.</param>
    /// <param name="Currency">The ISO 4217 denomination.</param>
    public void Deconstruct(out decimal Amount, out Currency Currency)
    {
        Amount = this.Amount;
        Currency = this.Currency;
    }

    /// <summary>
    /// Returns a new <see cref="Money"/> with the same <see cref="Currency"/> as this instance and
    /// the supplied amount.
    /// </summary>
    /// <param name="amount">The amount for the new value.</param>
    /// <returns>A <see cref="Money"/> denominated in this instance's currency with
    /// <paramref name="amount"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when this value has an undefined currency,
    /// including <see langword="default"/>(<see cref="Money"/>).</exception>
    public Money ToAmount(decimal amount)
    {
        RequireValidCurrency(this);
        return new Money(amount, Currency);
    }

    #region Arithmetic

    /// <summary>
    /// Adds a <see cref="decimal"/> to a <see cref="Money"/>, preserving its currency.
    /// </summary>
    /// <param name="d">The amount to add.</param>
    /// <param name="m">The money to add to.</param>
    /// <returns>A <see cref="Money"/> in <paramref name="m"/>'s currency with amount
    /// <c>m.Amount + d</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="m"/> has an undefined
    /// currency, including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="OverflowException">Thrown when the sum exceeds the range of
    /// <see cref="decimal"/>.</exception>
    public static Money operator +(decimal d, Money m)
    {
        RequireValidCurrency(m);
        return new Money(m.Amount + d, m.Currency);
    }

    /// <summary>
    /// Adds a <see cref="decimal"/> to a <see cref="Money"/>, preserving its currency.
    /// </summary>
    /// <param name="m">The money to add to.</param>
    /// <param name="d">The amount to add.</param>
    /// <returns>A <see cref="Money"/> in <paramref name="m"/>'s currency with amount
    /// <c>m.Amount + d</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="m"/> has an undefined
    /// currency, including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="OverflowException">Thrown when the sum exceeds the range of
    /// <see cref="decimal"/>.</exception>
    public static Money operator +(Money m, decimal d) => d + m;

    /// <summary>
    /// Adds two <see cref="Money"/> values of the same currency.
    /// </summary>
    /// <param name="m1">The first operand; its currency is required of <paramref name="m2"/>.</param>
    /// <param name="m2">The second operand.</param>
    /// <returns>A <see cref="Money"/> equal to the sum, in the shared currency.</returns>
    /// <exception cref="InvalidOperationException">Thrown when either operand has an undefined currency,
    /// including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="CurrencyMismatchException">Thrown when <paramref name="m1"/> and
    /// <paramref name="m2"/> are denominated in different currencies.</exception>
    /// <exception cref="OverflowException">Thrown when the sum exceeds the range of
    /// <see cref="decimal"/>.</exception>
    public static Money operator +(Money m1, Money m2)
    {
        RequireSameCurrency(m1, m2);
        return m1 + m2.Amount;
    }

    /// <summary>
    /// Subtracts a <see cref="decimal"/> from a <see cref="Money"/>, preserving its currency.
    /// </summary>
    /// <param name="m">The money to subtract from.</param>
    /// <param name="d">The amount to subtract.</param>
    /// <returns>A <see cref="Money"/> in <paramref name="m"/>'s currency with amount
    /// <c>m.Amount - d</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="m"/> has an undefined
    /// currency, including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="OverflowException">Thrown when the difference exceeds the range of
    /// <see cref="decimal"/>.</exception>
    public static Money operator -(Money m, decimal d)
    {
        RequireValidCurrency(m);
        return new Money(m.Amount - d, m.Currency);
    }

    /// <summary>
    /// Subtracts a <see cref="Money"/> from a <see cref="decimal"/>, preserving the money's currency.
    /// </summary>
    /// <param name="d">The amount to subtract from.</param>
    /// <param name="m">The money to subtract.</param>
    /// <returns>A <see cref="Money"/> in <paramref name="m"/>'s currency with amount
    /// <c>d - m.Amount</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="m"/> has an undefined
    /// currency, including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="OverflowException">Thrown when the difference exceeds the range of
    /// <see cref="decimal"/>.</exception>
    /// <remarks>Deliberately asymmetric with <see cref="op_Subtraction(Money,decimal)"/>: unlike
    /// <c>+</c> and <c>*</c>, subtraction is not commutative, so <c>d - m</c> and <c>m - d</c> differ
    /// in sign (<c>20 - 5 USD = 15 USD</c>, but <c>5 USD - 20 = -15 USD</c>).</remarks>
    public static Money operator -(decimal d, Money m)
    {
        RequireValidCurrency(m);
        return new Money(d - m.Amount, m.Currency);
    }

    /// <summary>
    /// Negates a <see cref="Money"/> value, preserving its currency.
    /// </summary>
    /// <param name="m">The money to negate.</param>
    /// <returns>A <see cref="Money"/> in <paramref name="m"/>'s currency with amount
    /// <c>-m.Amount</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="m"/> has an undefined
    /// currency, including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <remarks>Unlike the other arithmetic operators, unary negation of a <see cref="decimal"/> never
    /// overflows: <see cref="decimal.MinValue"/> and <see cref="decimal.MaxValue"/> are exact negatives
    /// of one another, so no <see cref="OverflowException"/> case exists here.</remarks>
    public static Money operator -(Money m)
    {
        RequireValidCurrency(m);
        return new Money(-m.Amount, m.Currency);
    }

    /// <summary>
    /// Subtracts one <see cref="Money"/> from another of the same currency.
    /// </summary>
    /// <param name="m1">The minuend; its currency is required of <paramref name="m2"/>.</param>
    /// <param name="m2">The subtrahend.</param>
    /// <returns>A <see cref="Money"/> equal to the difference, in the shared currency.</returns>
    /// <exception cref="InvalidOperationException">Thrown when either operand has an undefined currency,
    /// including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="CurrencyMismatchException">Thrown when <paramref name="m1"/> and
    /// <paramref name="m2"/> are denominated in different currencies.</exception>
    /// <exception cref="OverflowException">Thrown when the difference exceeds the range of
    /// <see cref="decimal"/>.</exception>
    public static Money operator -(Money m1, Money m2)
    {
        RequireSameCurrency(m1, m2);
        return m1 - m2.Amount;
    }

    /// <summary>
    /// Scales a <see cref="Money"/> by a <see cref="decimal"/> multiplier, preserving its currency.
    /// </summary>
    /// <param name="m">The money to scale.</param>
    /// <param name="d">The multiplier (e.g., a tax or quantity factor).</param>
    /// <returns>A <see cref="Money"/> in <paramref name="m"/>'s currency with amount
    /// <c>m.Amount * d</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="m"/> has an undefined
    /// currency, including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="OverflowException">Thrown when the product exceeds the range of
    /// <see cref="decimal"/>.</exception>
    public static Money operator *(Money m, decimal d)
    {
        RequireValidCurrency(m);
        return new Money(m.Amount * d, m.Currency);
    }

    /// <summary>
    /// Scales a <see cref="Money"/> by a <see cref="decimal"/> multiplier, preserving its currency.
    /// Commutative counterpart of <see cref="op_Multiply(Money, decimal)"/> so both
    /// <c>m * d</c> and <c>d * m</c> compile.
    /// </summary>
    /// <param name="d">The multiplier (e.g., a tax or quantity factor).</param>
    /// <param name="m">The money to scale.</param>
    /// <returns>A <see cref="Money"/> in <paramref name="m"/>'s currency with amount
    /// <c>m.Amount * d</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="m"/> has an undefined
    /// currency, including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="OverflowException">Thrown when the product exceeds the range of
    /// <see cref="decimal"/>.</exception>
    public static Money operator *(decimal d, Money m) => m * d;

    /// <summary>
    /// Divides a <see cref="Money"/> by a <see cref="decimal"/> divisor, preserving its currency.
    /// </summary>
    /// <param name="m">The money to divide.</param>
    /// <param name="d">The divisor. Must not be zero.</param>
    /// <returns>A <see cref="Money"/> in <paramref name="m"/>'s currency with amount
    /// <c>m.Amount / d</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="m"/> has an undefined
    /// currency, including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="DivideByZeroException">Thrown when <paramref name="d"/> is zero.</exception>
    /// <exception cref="OverflowException">Thrown when the quotient exceeds the range of
    /// <see cref="decimal"/>.</exception>
    public static Money operator /(Money m, decimal d)
    {
        RequireValidCurrency(m);
        return new Money(m.Amount / d, m.Currency);
    }

    /// <summary>Adds a <see cref="decimal"/> to this value, preserving its currency.</summary>
    /// <param name="d">The amount to add.</param>
    /// <returns>A <see cref="Money"/> with amount <c>this.Amount + d</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when this value has an undefined currency,
    /// including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="OverflowException">Thrown when the sum exceeds the range of
    /// <see cref="decimal"/>.</exception>
    public Money Add(decimal d) => this + d;

    /// <summary>Subtracts a <see cref="decimal"/> from this value, preserving its currency.</summary>
    /// <param name="d">The amount to subtract.</param>
    /// <returns>A <see cref="Money"/> with amount <c>this.Amount - d</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when this value has an undefined currency,
    /// including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="OverflowException">Thrown when the difference exceeds the range of
    /// <see cref="decimal"/>.</exception>
    public Money Subtract(decimal d) => this - d;

    /// <summary>Scales this value by a <see cref="decimal"/> multiplier, preserving its currency.</summary>
    /// <param name="d">The multiplier.</param>
    /// <returns>A <see cref="Money"/> with amount <c>this.Amount * d</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when this value has an undefined currency,
    /// including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="OverflowException">Thrown when the product exceeds the range of
    /// <see cref="decimal"/>.</exception>
    public Money Multiply(decimal d) => this * d;

    /// <summary>Divides this value by a <see cref="decimal"/> divisor, preserving its currency.</summary>
    /// <param name="d">The divisor. Must not be zero.</param>
    /// <returns>A <see cref="Money"/> with amount <c>this.Amount / d</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when this value has an undefined currency,
    /// including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="DivideByZeroException">Thrown when <paramref name="d"/> is zero.</exception>
    /// <exception cref="OverflowException">Thrown when the quotient exceeds the range of
    /// <see cref="decimal"/>.</exception>
    public Money Divide(decimal d) => this / d;

    /// <summary>Adds a same-currency <see cref="Money"/> to this value.</summary>
    /// <param name="m">The money to add; must share this value's currency.</param>
    /// <returns>A <see cref="Money"/> equal to <c>this + m</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when this value or <paramref name="m"/> has
    /// an undefined currency, including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="CurrencyMismatchException">Thrown when <paramref name="m"/> is a different
    /// currency than this value.</exception>
    /// <exception cref="OverflowException">Thrown when the sum exceeds the range of
    /// <see cref="decimal"/>.</exception>
    public Money Add(Money m) => this + m;

    /// <summary>Subtracts a same-currency <see cref="Money"/> from this value.</summary>
    /// <param name="m">The money to subtract; must share this value's currency.</param>
    /// <returns>A <see cref="Money"/> equal to <c>this - m</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when this value or <paramref name="m"/> has
    /// an undefined currency, including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="CurrencyMismatchException">Thrown when <paramref name="m"/> is a different
    /// currency than this value.</exception>
    /// <exception cref="OverflowException">Thrown when the difference exceeds the range of
    /// <see cref="decimal"/>.</exception>
    public Money Subtract(Money m) => this - m;

    #endregion

    #region Rounding

    /// <summary>
    /// Rounds a <see cref="Money"/> amount to the nearest integer using banker's rounding
    /// (<see cref="MidpointRounding.ToEven"/>).
    /// </summary>
    /// <param name="m">The money to round.</param>
    /// <returns>A <see cref="Money"/> in <paramref name="m"/>'s currency with the rounded amount.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="m"/> has an undefined
    /// currency, including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <seealso cref="decimal.Round(decimal)"/>
    public static Money Round(Money m)
    {
        RequireValidCurrency(m);
        return new Money(decimal.Round(m.Amount), m.Currency);
    }

    /// <summary>
    /// Rounds a <see cref="Money"/> amount to a specified number of decimal places using banker's
    /// rounding (<see cref="MidpointRounding.ToEven"/>).
    /// </summary>
    /// <param name="m">The money to round.</param>
    /// <param name="decimals">The number of decimal places in the result (e.g., <c>2</c> for a
    /// minor-unit currency).</param>
    /// <returns>A <see cref="Money"/> in <paramref name="m"/>'s currency rounded to
    /// <paramref name="decimals"/> places.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="m"/> has an undefined
    /// currency, including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="decimals"/> is
    /// negative or greater than 28.</exception>
    /// <seealso cref="decimal.Round(decimal,int)"/>
    public static Money Round(Money m, int decimals)
    {
        RequireValidCurrency(m);
        return new Money(decimal.Round(m.Amount, decimals), m.Currency);
    }

    /// <summary>
    /// Rounds a <see cref="Money"/> amount to the nearest integer, choosing the midpoint rule.
    /// </summary>
    /// <param name="m">The money to round.</param>
    /// <param name="mode">How to round an amount that is exactly halfway between two integers.</param>
    /// <returns>A <see cref="Money"/> in <paramref name="m"/>'s currency with the rounded amount.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="m"/> has an undefined
    /// currency, including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <seealso cref="decimal.Round(decimal,MidpointRounding)"/>
    public static Money Round(Money m, MidpointRounding mode)
    {
        RequireValidCurrency(m);
        return new Money(decimal.Round(m.Amount, mode), m.Currency);
    }

    /// <summary>
    /// Rounds a <see cref="Money"/> amount to a specified number of decimal places, choosing the
    /// midpoint rule.
    /// </summary>
    /// <param name="m">The money to round.</param>
    /// <param name="decimals">The number of decimal places in the result.</param>
    /// <param name="mode">How to round an amount exactly halfway between two representable values.</param>
    /// <returns>A <see cref="Money"/> in <paramref name="m"/>'s currency rounded to
    /// <paramref name="decimals"/> places under <paramref name="mode"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="m"/> has an undefined
    /// currency, including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="decimals"/> is
    /// negative or greater than 28.</exception>
    /// <seealso cref="decimal.Round(decimal,int,MidpointRounding)"/>
    public static Money Round(Money m, int decimals, MidpointRounding mode)
    {
        RequireValidCurrency(m);
        return new Money(decimal.Round(m.Amount, decimals, mode), m.Currency);
    }

    #endregion

    #region Ordering

    /// <summary>
    /// Compares this value to another <see cref="Money"/> of the same currency by amount.
    /// </summary>
    /// <param name="other">The money to compare to; must share this value's currency.</param>
    /// <returns>A signed number: negative if this amount precedes <paramref name="other"/>'s, zero
    /// if equal, positive if it follows.</returns>
    /// <exception cref="InvalidOperationException">Thrown when this value or <paramref name="other"/> has
    /// an undefined currency, including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="CurrencyMismatchException">Thrown when <paramref name="other"/> is a
    /// different currency — two currencies have no defined order.</exception>
    /// <remarks><para><b>Complexity:</b> O(1). Ordering is currency-sensitive on purpose: unlike
    /// equality (which returns <see langword="false"/> across currencies), there is no meaningful
    /// "less than" between <c>USD</c> and <c>EUR</c>, so a mismatch is a programming error.</para>
    /// <para><b>Sorting a mixed-currency collection:</b> because a cross-currency comparison throws,
    /// sorting a sequence that contains more than one currency (e.g. <see cref="List{T}.Sort()"/> over a
    /// <c>List&lt;Money&gt;</c>) will throw <see cref="CurrencyMismatchException"/> mid-sort and may leave
    /// the collection partially reordered. Partition by <see cref="Currency"/> first, or sort each
    /// single-currency group independently.</para></remarks>
    public int CompareTo(Money other)
    {
        RequireSameCurrency(this, other);
        return Amount.CompareTo(other.Amount);
    }

    /// <summary>
    /// Compares this value to another object for ordering.
    /// </summary>
    /// <param name="obj">The object to compare to, or <see langword="null"/>.</param>
    /// <returns>A positive number when <paramref name="obj"/> is <see langword="null"/> (every value
    /// follows <see langword="null"/>); otherwise the result of <see cref="CompareTo(Money)"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when this value, or a <see cref="Money"/>
    /// supplied in <paramref name="obj"/>, has an undefined currency, including
    /// <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="obj"/> is not a
    /// <see cref="Money"/>.</exception>
    /// <exception cref="CurrencyMismatchException">Thrown when <paramref name="obj"/> is a
    /// <see cref="Money"/> of a different currency.</exception>
    public int CompareTo(object? obj)
    {
        RequireValidCurrency(this);

        return obj switch
        {
            null => 1,
            Money money => CompareTo(money),
            _ => throw new ArgumentException($"Object must be of type {nameof(Money)}.", nameof(obj)),
        };
    }

    /// <summary>
    /// Returns whether <paramref name="m1"/> orders before <paramref name="m2"/>.
    /// </summary>
    /// <param name="m1">The left operand.</param>
    /// <param name="m2">The right operand; must share <paramref name="m1"/>'s currency.</param>
    /// <returns><see langword="true"/> if <paramref name="m1"/>'s amount is less than
    /// <paramref name="m2"/>'s.</returns>
    /// <exception cref="InvalidOperationException">Thrown when either operand has an undefined currency,
    /// including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="CurrencyMismatchException">Thrown when the two values are different
    /// currencies.</exception>
    public static bool operator <(Money m1, Money m2) => m1.CompareTo(m2) < 0;

    /// <summary>
    /// Returns whether <paramref name="m1"/> orders after <paramref name="m2"/>.
    /// </summary>
    /// <param name="m1">The left operand.</param>
    /// <param name="m2">The right operand; must share <paramref name="m1"/>'s currency.</param>
    /// <returns><see langword="true"/> if <paramref name="m1"/>'s amount is greater than
    /// <paramref name="m2"/>'s.</returns>
    /// <exception cref="InvalidOperationException">Thrown when either operand has an undefined currency,
    /// including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="CurrencyMismatchException">Thrown when the two values are different
    /// currencies.</exception>
    public static bool operator >(Money m1, Money m2) => m1.CompareTo(m2) > 0;

    /// <summary>
    /// Returns whether <paramref name="m1"/> orders at or before <paramref name="m2"/>.
    /// </summary>
    /// <param name="m1">The left operand.</param>
    /// <param name="m2">The right operand; must share <paramref name="m1"/>'s currency.</param>
    /// <returns><see langword="true"/> if <paramref name="m1"/>'s amount is less than or equal to
    /// <paramref name="m2"/>'s.</returns>
    /// <exception cref="InvalidOperationException">Thrown when either operand has an undefined currency,
    /// including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="CurrencyMismatchException">Thrown when the two values are different
    /// currencies.</exception>
    public static bool operator <=(Money m1, Money m2) => m1.CompareTo(m2) <= 0;

    /// <summary>
    /// Returns whether <paramref name="m1"/> orders at or after <paramref name="m2"/>.
    /// </summary>
    /// <param name="m1">The left operand.</param>
    /// <param name="m2">The right operand; must share <paramref name="m1"/>'s currency.</param>
    /// <returns><see langword="true"/> if <paramref name="m1"/>'s amount is greater than or equal to
    /// <paramref name="m2"/>'s.</returns>
    /// <exception cref="InvalidOperationException">Thrown when either operand has an undefined currency,
    /// including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <exception cref="CurrencyMismatchException">Thrown when the two values are different
    /// currencies.</exception>
    public static bool operator >=(Money m1, Money m2) => m1.CompareTo(m2) >= 0;

    #endregion

    /// <summary>
    /// Returns the amount and currency as a culture-invariant string, for example <c>"19.99 USD"</c>.
    /// </summary>
    /// <returns>The invariant-culture amount, a space, and the currency code.</returns>
    /// <exception cref="InvalidOperationException">Thrown when this value has an undefined currency,
    /// including <see langword="default"/>(<see cref="Money"/>).</exception>
    /// <remarks>The format is stable for logging and diagnostics, not localized for end-user
    /// display; a UI layer should format the amount with the user's culture and currency symbol.</remarks>
    public override string ToString()
    {
        RequireValidCurrency(this);
        return string.Concat(Amount.ToString(CultureInfo.InvariantCulture), " ", Currency.ToString());
    }

    /// <summary>
    /// Throws <see cref="CurrencyMismatchException"/> when two values are not the same currency.
    /// </summary>
    private static void RequireSameCurrency(Money m1, Money m2)
    {
        RequireValidCurrency(m1);
        RequireValidCurrency(m2);

        if (m1.Currency != m2.Currency)
        {
            throw new CurrencyMismatchException(m1.Currency, m2.Currency);
        }
    }

    private static void ValidateCurrencyArgument(Currency currency, string parameterName)
    {
        if (!IsValidCurrency(currency))
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                currency,
                "Money requires a defined ISO 4217 currency other than Currency.Unspecified.");
        }
    }

    private static void RequireValidCurrency(Money money)
    {
        if (!IsValidCurrency(money.Currency))
        {
            throw new InvalidOperationException(
                "Money operations require a defined ISO 4217 currency other than Currency.Unspecified.");
        }
    }

    private static bool IsValidCurrency(Currency currency)
        => currency != Currency.Unspecified && Enum.IsDefined(currency);
}
