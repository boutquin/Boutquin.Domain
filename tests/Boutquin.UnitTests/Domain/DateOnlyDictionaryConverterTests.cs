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

using System.Text.Json;
using Boutquin.Domain.Converters;

namespace Boutquin.UnitTests.Domain;

/// <summary>
/// Tests for <see cref="DateOnlyDictionaryConverterFactory"/>: round-trip fidelity, explicit null-value
/// rejection, and the token-type guard that turns malformed JSON into a clear <see cref="JsonException"/>.
/// </summary>
public sealed class DateOnlyDictionaryConverterTests
{
    private static readonly JsonSerializerOptions s_options = CreateOptions();

    private static JsonSerializerOptions CreateOptions(bool registerDateOnlyConverter = true)
    {
        var options = new JsonSerializerOptions();
        if (registerDateOnlyConverter)
        {
            options.Converters.Add(new DateOnlyConverter());
        }

        options.Converters.Add(new DateOnlyDictionaryConverterFactory());
        return options;
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Dictionary_RoundTrips(bool registerDateOnlyConverter)
    {
        var options = CreateOptions(registerDateOnlyConverter);
        var dict = new Dictionary<DateOnly, int>
        {
            [new DateOnly(2026, 1, 1)] = 1,
            [new DateOnly(2026, 12, 31)] = 2,
        };

        var json = JsonSerializer.Serialize(dict, options);
        var parsed = JsonSerializer.Deserialize<Dictionary<DateOnly, int>>(json, options);

        parsed.Should().BeEquivalentTo(dict);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SortedDictionary_RoundTrips(bool registerDateOnlyConverter)
    {
        var options = CreateOptions(registerDateOnlyConverter);
        var dict = new SortedDictionary<DateOnly, string>
        {
            [new DateOnly(2026, 5, 1)] = "may",
            [new DateOnly(2026, 1, 1)] = "jan",
        };

        var json = JsonSerializer.Serialize(dict, options);
        var parsed = JsonSerializer.Deserialize<SortedDictionary<DateOnly, string>>(json, options);

        parsed.Should().BeEquivalentTo(dict);
        parsed!.Keys.Should().BeInAscendingOrder();
    }

    [Fact]
    public void Read_NullValue_ThrowsJsonException()
    {
        var act = () => JsonSerializer.Deserialize<Dictionary<DateOnly, string>>(
            "{\"2026-01-01\":null}", s_options);

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void Read_MalformedJsonArray_ThrowsJsonException()
    {
        // An array where an object was expected: the token-type guard must reject the non-PropertyName
        // token with a JsonException instead of an opaque InvalidOperationException from GetString().
        var act = () => JsonSerializer.Deserialize<Dictionary<DateOnly, int>>("[1,2]", s_options);

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void Read_BareScalar_ThrowsJsonException_MatchingSortedConverter()
    {
        // D-05: a bare-scalar payload must throw a JsonException naming the found token, NOT silently
        // yield an empty dictionary. The unsorted converter must match the SortedDictionary converter's
        // StartObject-guard behavior on the identical input.
        var actUnsorted = () => JsonSerializer.Deserialize<Dictionary<DateOnly, int>>("5", s_options);
        var actSorted = () => JsonSerializer.Deserialize<SortedDictionary<DateOnly, int>>("5", s_options);

        actUnsorted.Should().Throw<JsonException>().WithMessage("*Number*");
        actSorted.Should().Throw<JsonException>().WithMessage("*Number*");
    }
}
