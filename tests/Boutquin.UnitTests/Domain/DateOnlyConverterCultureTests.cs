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

using System.Globalization;
using System.Text.Json;
using Boutquin.Domain.Converters;

namespace Boutquin.UnitTests.Domain;

/// <summary>
/// Tests that <see cref="DateOnlyConverter"/> reads with the invariant culture, mirroring its writer,
/// so the round-trip is stable under a non-invariant ambient culture.
/// </summary>
public sealed class DateOnlyConverterCultureTests
{
    private static readonly JsonSerializerOptions s_options = CreateOptions();

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new DateOnlyConverter());
        return options;
    }

    [Fact]
    public void Read_UnderNonInvariantCulture_RoundTrips()
    {
        var date = new DateOnly(2026, 6, 23);
        var json = JsonSerializer.Serialize(date, s_options);

        // Deserialize on a dedicated thread whose culture uses dd/MM/yyyy date ordering and a comma
        // decimal separator, without mutating the shared ambient culture.
        DateOnly parsed = default;
        var thread = new Thread(() =>
        {
            CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
            parsed = JsonSerializer.Deserialize<DateOnly>(json, s_options);
        });
        thread.Start();
        thread.Join();

        json.Should().Be("\"2026-06-23\"");
        parsed.Should().Be(date);
    }

    [Fact]
    public void Read_InvalidString_ThrowsJsonException()
    {
        var act = () => JsonSerializer.Deserialize<DateOnly>("\"not-a-date\"", s_options);

        act.Should().Throw<JsonException>();
    }
}
