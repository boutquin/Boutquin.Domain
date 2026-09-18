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

using Boutquin.Domain.Enumerations;
using Boutquin.Domain.ValueObjects;

namespace Boutquin.UnitTests.ValueObjects;

/// <summary>
/// Tests pinning the documented <see cref="Money.Round(Money, int)"/> upper-bound contract and the
/// mixed-currency ordering behavior surfaced in review.
/// </summary>
public sealed class MoneyFixTests
{
    private static Money Usd(decimal amount) => new(amount, Currency.USD);
    private static Money Eur(decimal amount) => new(amount, Currency.EUR);

    [Fact]
    public void Round_With29Decimals_ThrowsArgumentOutOfRangeException()
    {
        var act = () => Money.Round(Usd(1m), 29);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Round_With28Decimals_DoesNotThrow()
    {
        var act = () => Money.Round(Usd(1m), 28);

        act.Should().NotThrow();
    }

    [Fact]
    public void Sort_MixedCurrencyList_ThrowsCurrencyMismatch()
    {
        var list = new List<Money> { Usd(1m), Eur(1m) };

        var act = () => list.Sort();

        // List.Sort surfaces a comparer exception either directly or wrapped in an
        // InvalidOperationException; either way a CurrencyMismatchException must be in the chain.
        var thrown = act.Should().Throw<Exception>().Which;
        var isMismatchInChain =
            thrown is CurrencyMismatchException || thrown.InnerException is CurrencyMismatchException;
        isMismatchInChain.Should().BeTrue("a cross-currency comparison has no defined order");
    }
}
