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
using Boutquin.Domain.Enumerations;
using Boutquin.Domain.ValueObjects;

namespace Boutquin.UnitTests.Enumerations;

public sealed class CurrencyTests
{
    [Theory]
    [InlineData(Currency.USD, 840)]
    [InlineData(Currency.EUR, 978)]
    [InlineData(Currency.JPY, 392)]
    [InlineData(Currency.GBP, 826)]
    [InlineData(Currency.CAD, 124)]
    public void NumericValue_EqualsIso4217NumericCode(Currency currency, int numericCode)
        => ((int)currency).Should().Be(numericCode);

    [Fact]
    public void Unspecified_IsDefaultZero()
    {
        ((int)Currency.Unspecified).Should().Be(0);
        default(Currency).Should().Be(Currency.Unspecified);
    }

    [Theory]
    [InlineData(Currency.USD, "United States dollar")]
    [InlineData(Currency.EUR, "Euro")]
    [InlineData(Currency.JPY, "Japanese yen")]
    public void GetDescription_ReturnsIso4217Name(Currency currency, string expected)
        => currency.GetDescription().Should().Be(expected);

    [Fact]
    public void GetDescription_RepairedMojibakeNames_AreClean()
    {
        // These three lost characters in the legacy source; verify they were repaired
        // and carry no replacement marker.
        Currency.PLN.GetDescription().Should().Be("Polish zloty");
        Currency.TOP.GetDescription().Should().Be("Tongan pa'anga");
        Currency.VND.GetDescription().Should().Be("Vietnamese dong");

        Currency.PLN.GetDescription().Should().NotContain("?");
        Currency.VND.GetDescription().Should().NotContain("?");
    }

    [Fact]
    public void GetDescription_NonAsciiNames_PreserveAccentedCharacters()
    {
        // Guard against encoding regressions on accented ISO 4217 names.
        Currency.MNT.GetDescription().Should().Be("Mongolian tögrög");
        Currency.NIO.GetDescription().Should().Be("Nicaraguan córdoba");
        Currency.PYG.GetDescription().Should().Be("Paraguayan guaraní");
        Currency.UYW.GetDescription().Should().Be("Unidad previsional");

        Currency.MNT.GetDescription().Should().NotContain("?").And.NotContain("�");
    }

    [Fact]
    public void Amount_FactoryExtension_ProducesMoney()
    {
        var money = Currency.EUR.Amount(42.50m);

        money.Should().Be(new Money(42.50m, Currency.EUR));
    }
}
