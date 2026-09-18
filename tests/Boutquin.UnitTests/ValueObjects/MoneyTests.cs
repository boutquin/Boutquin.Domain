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
using System.Globalization;
using System.Text.Json;

using Boutquin.Domain.Enumerations;
using Boutquin.Domain.ValueObjects;

namespace Boutquin.UnitTests.ValueObjects;

public sealed class MoneyTests
{
    private static Money Usd(decimal a) => new(a, Currency.USD);
    private static Money Eur(decimal a) => new(a, Currency.EUR);

    [Fact]
    public void Constructor_SetsAmountAndCurrency()
    {
        var m = new Money(19.99m, Currency.USD);

        m.Amount.Should().Be(19.99m);
        m.Currency.Should().Be(Currency.USD);
    }

    [Theory]
    [InlineData(Currency.Unspecified)]
    [InlineData((Currency)(-1))]
    public void Constructor_InvalidCurrency_ThrowsArgumentOutOfRange(Currency currency)
    {
        var act = () => _ = new Money(19.99m, currency);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName(nameof(Money.Currency));
    }

    [Fact]
    public void Deconstruct_PreservesPositionalRecordParameterNamesAndOrder()
    {
        var money = new Money(19.99m, Currency.USD);

        money.Deconstruct(Amount: out var amount, Currency: out var currency);

        amount.Should().Be(19.99m);
        currency.Should().Be(Currency.USD);
    }

    [Fact]
    public void JsonSerializer_ValidMoney_RoundTrips()
    {
        var money = Usd(19.99m);

        var json = JsonSerializer.Serialize(money);
        var result = JsonSerializer.Deserialize<Money>(json);

        result.Should().Be(money);
    }

    [Theory]
    [InlineData("""{"Amount":19.99,"Currency":0}""")]
    [InlineData("""{"Amount":19.99,"Currency":-1}""")]
    [InlineData("""{"Amount":19.99}""")]
    public void JsonSerializer_InvalidOrMissingCurrency_ThrowsArgumentOutOfRange(string json)
    {
        var act = () => JsonSerializer.Deserialize<Money>(json);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName(nameof(Money.Currency));
    }

    [Fact]
    public void DefaultMoney_CurrencyDependentOperations_ThrowInvalidOperation()
    {
        var money = default(Money);
        var other = default(Money);
        Action[] operations =
        [
            () => _ = money.ToAmount(1m),
            () => _ = money + 1m,
            () => _ = 1m + money,
            () => _ = money - 1m,
            () => _ = 1m - money,
            () => _ = -money,
            () => _ = money * 2m,
            () => _ = 2m * money,
            () => _ = money / 2m,
            () => _ = money + other,
            () => _ = money - other,
            () => _ = money.Add(1m),
            () => _ = money.Subtract(1m),
            () => _ = money.Multiply(2m),
            () => _ = money.Divide(2m),
            () => _ = money.Add(other),
            () => _ = money.Subtract(other),
            () => _ = Money.Round(money),
            () => _ = Money.Round(money, 2),
            () => _ = Money.Round(money, MidpointRounding.ToEven),
            () => _ = Money.Round(money, 2, MidpointRounding.ToEven),
            () => _ = money.CompareTo(other),
            () => _ = money.CompareTo(null),
            () => _ = money < other,
            () => _ = money > other,
            () => _ = money <= other,
            () => _ = money >= other,
            () => _ = money.ToString(),
        ];

        foreach (var operation in operations)
        {
            operation.Should().Throw<InvalidOperationException>();
        }
    }

    [Fact]
    public void ValidMoney_WithDefaultSecondOperand_CurrencyDependentOperationsThrowInvalidOperation()
    {
        var money = Usd(10m);
        var other = default(Money);
        Action[] operations =
        [
            () => _ = money + other,
            () => _ = money - other,
            () => _ = money.Add(other),
            () => _ = money.Subtract(other),
            () => _ = money.CompareTo(other),
            () => _ = money.CompareTo((object)other),
            () => _ = money < other,
            () => _ = money > other,
            () => _ = money <= other,
            () => _ = money >= other,
        ];

        foreach (var operation in operations)
        {
            operation.Should().Throw<InvalidOperationException>();
        }
    }

    [Fact]
    public void DefaultMoney_EqualityAndHashing_DoNotThrow()
    {
        var money = default(Money);
        var other = default(Money);

        Action[] operations =
        [
            () => _ = money == other,
            () => _ = money != other,
            () => _ = money.Equals(other),
            () => _ = money.Equals((object)other),
            () => _ = money.GetHashCode(),
        ];

        foreach (var operation in operations)
        {
            operation.Should().NotThrow();
        }
    }

    [Fact]
    public void ToAmount_KeepsCurrency_ReplacesAmount()
    {
        var m = Usd(10m).ToAmount(42m);

        m.Should().Be(Usd(42m));
    }

    // --- Equality (structural, never throws across currencies) ---

    [Fact]
    public void Equality_SameAmountAndCurrency_AreEqual()
    {
        Usd(5m).Should().Be(Usd(5m));
        (Usd(5m) == Usd(5m)).Should().BeTrue();
        Usd(5m).GetHashCode().Should().Be(Usd(5m).GetHashCode());
    }

    [Fact]
    public void Equality_DifferentCurrency_NotEqual_DoesNotThrow()
    {
        (Usd(5m) == Eur(5m)).Should().BeFalse();
        (Usd(5m) != Eur(5m)).Should().BeTrue();
        Usd(5m).Equals(Eur(5m)).Should().BeFalse();
    }

    [Fact]
    public void Equality_DifferentAmount_NotEqual()
    {
        Usd(5m).Should().NotBe(Usd(6m));
    }

    // --- Arithmetic ---

    [Fact]
    public void Add_Decimal_BothOperandOrders_PreserveCurrency()
    {
        (Usd(10m) + 5m).Should().Be(Usd(15m));
        (5m + Usd(10m)).Should().Be(Usd(15m));
    }

    [Fact]
    public void Subtract_Decimal_PreservesCurrency()
        => (Usd(10m) - 3m).Should().Be(Usd(7m));

    [Fact]
    public void Multiply_And_Divide_Decimal_PreserveCurrency()
    {
        (Usd(10m) * 1.5m).Should().Be(Usd(15m));
        (Usd(10m) / 4m).Should().Be(Usd(2.5m));
    }

    [Fact]
    public void Divide_ByZero_Throws()
    {
        var act = () => Usd(10m) / 0m;

        act.Should().Throw<DivideByZeroException>();
    }

    [Fact]
    public void Add_SameCurrency_Sums()
        => (Usd(10m) + Usd(5m)).Should().Be(Usd(15m));

    [Fact]
    public void Subtract_SameCurrency_Differences()
        => (Usd(10m) - Usd(4m)).Should().Be(Usd(6m));

    [Fact]
    public void Add_DifferentCurrency_ThrowsWithBothCurrencies()
    {
        var act = () => Usd(10m) + Eur(5m);

        act.Should().Throw<CurrencyMismatchException>()
            .Which.Expected.Should().Be(Currency.USD);
    }

    [Fact]
    public void Subtract_DifferentCurrency_Throws()
    {
        var act = () => Usd(10m) - Eur(5m);

        act.Should().Throw<CurrencyMismatchException>()
            .Which.Actual.Should().Be(Currency.EUR);
    }

    [Fact]
    public void InstanceMethods_MirrorOperators()
    {
        Usd(10m).Add(5m).Should().Be(Usd(15m));
        Usd(10m).Subtract(5m).Should().Be(Usd(5m));
        Usd(10m).Multiply(2m).Should().Be(Usd(20m));
        Usd(10m).Divide(2m).Should().Be(Usd(5m));
        Usd(10m).Add(Usd(5m)).Should().Be(Usd(15m));
        Usd(10m).Subtract(Usd(5m)).Should().Be(Usd(5m));
    }

    [Fact]
    public void InstanceAdd_DifferentCurrency_Throws()
    {
        var act = () => Usd(10m).Add(Eur(5m));

        act.Should().Throw<CurrencyMismatchException>();
    }

    // --- Rounding ---

    [Fact]
    public void Round_ToNearestInteger_UsesBankersRounding()
    {
        // 2.5 and 3.5 both round to even (2 and 4) under ToEven.
        Money.Round(Usd(2.5m)).Should().Be(Usd(2m));
        Money.Round(Usd(3.5m)).Should().Be(Usd(4m));
    }

    [Fact]
    public void Round_ToDecimals_QuantizesAndKeepsCurrency()
        => Money.Round(Usd(22.5887m), 2).Should().Be(Usd(22.59m));

    [Fact]
    public void Round_AwayFromZero_OverridesMidpointRule()
    {
        Money.Round(Usd(2.5m), MidpointRounding.AwayFromZero).Should().Be(Usd(3m));
        Money.Round(Usd(2.345m), 2, MidpointRounding.AwayFromZero).Should().Be(Usd(2.35m));
    }

    [Fact]
    public void Round_NegativeDecimals_Throws()
    {
        var act = () => Money.Round(Usd(1m), -1);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    // --- Ordering ---

    [Fact]
    public void Comparison_SameCurrency_OrdersByAmount()
    {
        (Usd(5m) < Usd(10m)).Should().BeTrue();
        (Usd(10m) > Usd(5m)).Should().BeTrue();
        (Usd(5m) <= Usd(5m)).Should().BeTrue();
        (Usd(5m) >= Usd(5m)).Should().BeTrue();
    }

    [Fact]
    public void Comparison_DifferentCurrency_Throws()
    {
        var act = () => Usd(5m) < Eur(10m);

        act.Should().Throw<CurrencyMismatchException>();
    }

    [Fact]
    public void CompareTo_Money_ReturnsSign()
    {
        Usd(5m).CompareTo(Usd(10m)).Should().BeNegative();
        Usd(10m).CompareTo(Usd(5m)).Should().BePositive();
        Usd(5m).CompareTo(Usd(5m)).Should().Be(0);
    }

    [Fact]
    public void CompareTo_Money_DifferentCurrency_Throws()
    {
        var act = () => Usd(5m).CompareTo(Eur(5m));

        act.Should().Throw<CurrencyMismatchException>();
    }

    [Fact]
    public void CompareTo_Object_Null_ReturnsPositive()
        => Usd(5m).CompareTo(null).Should().Be(1);

    [Fact]
    public void CompareTo_Object_WrongType_Throws()
    {
        var act = () => Usd(5m).CompareTo("not money");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CompareTo_Object_BoxedMoney_ReturnsSign()
    {
        object boxed = Usd(10m);

        Usd(5m).CompareTo(boxed).Should().BeNegative();
        Usd(10m).CompareTo((object)Usd(10m)).Should().Be(0);
    }

    [Fact]
    public void Sort_SameCurrency_OrdersAscending()
    {
        var list = new List<Money> { Usd(3m), Usd(1m), Usd(2m) };

        list.Sort();

        list.Should().ContainInOrder(Usd(1m), Usd(2m), Usd(3m));
    }

    // --- ToString ---

    [Fact]
    public void ToString_IsInvariantAmountThenCode()
    {
        // Format on a dedicated thread whose culture uses a comma decimal separator, proving invariance
        // WITHOUT mutating the shared process/ambient culture (which would race parallel-running tests).
        string? formatted = null;
        var thread = new Thread(() =>
        {
            CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
            formatted = Usd(1234.5m).ToString();
        });
        thread.Start();
        thread.Join();

        formatted.Should().Be("1234.5 USD");
    }

    [Fact]
    public void Multiply_DecimalTimesMoney_IsCommutativeWithMoneyTimesDecimal()
    {
        var price = Usd(19.99m);

        (2m * price).Should().Be(price * 2m);
        (2m * price).Amount.Should().Be(39.98m);
        (2m * price).Currency.Should().Be(Currency.USD);
    }

    [Theory]
    [InlineData(Currency.Unspecified)]
    [InlineData((Currency)(-1))]
    public void Amount_OnInvalidCurrency_ThrowsArgumentOutOfRange(Currency currency)
    {
        var act = () => currency.Amount(100m);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName(nameof(currency));
    }

    [Fact]
    public void Amount_OnValidCurrency_ReturnsMoneyWithGivenAmountAndCurrency()
    {
        // AC-4.3: the success path of CurrencyExtensions.Amount, complementing the Unspecified-guard
        // test above.
        var money = Currency.EUR.Amount(42.50m);

        money.Should().Be(new Money(42.50m, Currency.EUR));
    }

    // --- AC-4.2: decimal.MaxValue overflow ---

    [Fact]
    public void Add_AtDecimalMaxValue_ThrowsOverflowException()
    {
        var act = () => Usd(decimal.MaxValue) + Usd(decimal.MaxValue);

        act.Should().Throw<OverflowException>();
    }

    [Fact]
    public void Add_Decimal_AtDecimalMaxValue_ThrowsOverflowException()
    {
        // Pins the operator +(Money, decimal) overload directly (round-2 finding
        // F-correctness-contracts-01: its <exception cref="OverflowException"> doc tag was
        // added without a direct test — the Money+Money overflow test above only exercises
        // this overload transitively via m1 + m2.Amount).
        var act = () => Usd(decimal.MaxValue) + 1m;

        act.Should().Throw<OverflowException>();
    }

    [Fact]
    public void Subtract_AtDecimalMinValue_ThrowsOverflowException()
    {
        var act = () => Usd(decimal.MinValue) - Usd(1m);

        act.Should().Throw<OverflowException>();
    }

    [Fact]
    public void Multiply_AtDecimalMaxValue_ThrowsOverflowException()
    {
        var act = () => Usd(decimal.MaxValue) * 2m;

        act.Should().Throw<OverflowException>();
    }

    [Fact]
    public void Divide_AtDecimalMaxValue_BySmallDivisor_ThrowsOverflowException()
    {
        // decimal.MaxValue / 0.5m exceeds the range of decimal — division overflows just like
        // +, -, and * (F-L2-03: this exception path was previously undocumented on the / operator).
        var act = () => Usd(decimal.MaxValue) / 0.5m;

        act.Should().Throw<OverflowException>();
    }

    [Fact]
    public void Add_DecimalNamedMethod_AtDecimalMaxValue_ThrowsOverflowException()
    {
        // O-1: pins Money.Add(decimal) directly (not just its `this + d` delegate target) so the
        // newly-added <exception cref="OverflowException"> doc tag on the named method is backed
        // by a test that calls the named method, not only the operator it forwards to.
        var act = () => Usd(decimal.MaxValue).Add(1m);

        act.Should().Throw<OverflowException>();
    }

    [Fact]
    public void Subtract_DecimalNamedMethod_AtDecimalMinValue_ThrowsOverflowException()
    {
        // O-1: pins Money.Subtract(decimal) directly, mirroring Add_DecimalNamedMethod above.
        var act = () => Usd(decimal.MinValue).Subtract(1m);

        act.Should().Throw<OverflowException>();
    }

    [Fact]
    public void Multiply_DecimalNamedMethod_AtDecimalMaxValue_ThrowsOverflowException()
    {
        // O-1: pins Money.Multiply(decimal) directly, mirroring Add_DecimalNamedMethod above.
        var act = () => Usd(decimal.MaxValue).Multiply(2m);

        act.Should().Throw<OverflowException>();
    }

    [Fact]
    public void NegativeAmounts_Arithmetic_PreservesSignAndCurrency()
    {
        (Usd(-5m) + Usd(3m)).Should().Be(Usd(-2m));
        (Usd(2m) - Usd(5m)).Should().Be(Usd(-3m));
        (Usd(-4m) * 2m).Should().Be(Usd(-8m));
    }

    [Fact]
    public void ZeroAmount_EqualsAndComparesCorrectly()
    {
        Usd(0m).Should().Be(Usd(0m));
        Usd(0m).CompareTo(Usd(0m)).Should().Be(0);
        (Usd(0m) < Usd(0m)).Should().BeFalse();
        (Usd(-1m) < Usd(0m)).Should().BeTrue();
    }

    [Fact]
    public void Round_NegativeMidpoint_UsesBankersRounding()
    {
        // decimal banker's rounding: -2.5 → -2 (nearest even), -3.5 → -4.
        Money.Round(Usd(-2.5m)).Should().Be(Usd(-2m));
        Money.Round(Usd(-3.5m)).Should().Be(Usd(-4m));
    }

    [Fact]
    public void Round_WithMidpointRoundingOverload_HonorsMode()
    {
        Money.Round(Usd(2.5m), MidpointRounding.AwayFromZero).Should().Be(Usd(3m));
        Money.Round(Usd(2.5m), MidpointRounding.ToEven).Should().Be(Usd(2m));
    }

    [Fact]
    public void Subtract_Instance_DifferentCurrency_ThrowsWithBothCurrencies()
    {
        var act = () => Usd(5m).Subtract(Eur(3m));

        act.Should().Throw<CurrencyMismatchException>()
            .Which.Should().Match<CurrencyMismatchException>(ex =>
                ex.Expected == Currency.USD && ex.Actual == Currency.EUR);
    }

    // --- DE-6 / AC-3.13: decimal - Money and unary -Money operators ---

    [Fact]
    public void Subtract_DecimalMinusMoney_PreservesCurrency()
        => (20m - Usd(5m)).Should().Be(Usd(15m));

    [Fact]
    public void Subtract_DecimalMinusMoney_IsNotCommutativeWithMoneyMinusDecimal()
    {
        // decimal - Money and Money - decimal are deliberately asymmetric (unlike + and *):
        // 20 - 5USD = 15 USD, but 5USD - 20 = -15 USD.
        (20m - Usd(5m)).Should().NotBe(Usd(5m) - 20m);
        (Usd(5m) - 20m).Should().Be(Usd(-15m));
    }

    [Fact]
    public void UnaryMinus_NegatesAmount_PreservesCurrency()
    {
        (-Usd(5m)).Should().Be(Usd(-5m));
        (-Usd(-5m)).Should().Be(Usd(5m));
        (-Usd(0m)).Should().Be(Usd(0m));
    }
}
