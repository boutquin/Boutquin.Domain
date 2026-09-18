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

namespace Boutquin.UnitTests.Enumerations;

/// <summary>
/// Data-driven coverage over the <em>entire</em> <see cref="Currency"/> catalogue. The catalogue was
/// transcribed from ISO 4217 source data that historically suffered mojibake corruption; a single
/// missing or mangled description in any of the ~170 members would otherwise ship unnoticed. This
/// asserts every member resolves to a non-empty, marker-free description.
/// </summary>
public sealed class CurrencyCatalogueTests
{
    public static TheoryData<Currency> AllCurrencies()
    {
        var data = new TheoryData<Currency>();
        foreach (var currency in Enum.GetValues<Currency>())
        {
            data.Add(currency);
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(AllCurrencies))]
    public void GetDescription_ForEveryMember_IsNonEmptyAndMarkerFree(Currency currency)
    {
        var description = currency.GetDescription();

        description.Should().NotBeNullOrWhiteSpace();
        description.Should().NotContain("�"); // Unicode replacement character (mojibake marker)
        description.Should().NotContain("?");       // ASCII fallback marker for lost characters
    }

    [Fact]
    public void NumericCodes_AreUniqueAcrossCatalogue()
    {
        // AC-4.2: the enum's backing numeric value is documented as the ISO 4217 numeric code, which a
        // consumer reads directly off the enum value without a lookup table. A transcription error that
        // assigned the same numeric code to two different alphabetic members would silently make one of
        // them unreachable by numeric round-trip and break that "read the code directly" contract.
        var duplicates = Enum.GetValues<Currency>()
            .GroupBy(c => (int)c)
            .Where(g => g.Count() > 1)
            .ToList();

        duplicates.Should().BeEmpty();
    }
}
