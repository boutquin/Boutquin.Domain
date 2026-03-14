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

using System.Text;
using System.Text.Json;

using Boutquin.Domain.Converters;

namespace Boutquin.UnitTests.Domain;

/// <summary>
/// Contains unit tests for <see cref="DateOnlyConverter"/> and <see cref="DateOnlyDictionaryConverterFactory"/>.
/// </summary>
public sealed class DateOnlyConverterTests
{
    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new DateOnlyConverter());
        options.Converters.Add(new DateOnlyDictionaryConverterFactory());
        return options;
    }

    // --- DateOnlyConverter Tests ---

    [Fact]
    public void Read_ValidDateString_ReturnsCorrectDateOnly()
    {
        // Arrange
        var json = "\"2024-01-15\""u8;
        var reader = new Utf8JsonReader(json);
        reader.Read();
        var converter = new DateOnlyConverter();

        // Act
        var result = converter.Read(ref reader, typeof(DateOnly), new JsonSerializerOptions());

        // Assert
        result.Should().Be(new DateOnly(2024, 1, 15));
    }

    [Fact]
    public void Read_NullString_ThrowsJsonException()
    {
        // Arrange & Act
        var act = () =>
        {
            var json = "null"u8;
            var reader = new Utf8JsonReader(json);
            reader.Read();
            var converter = new DateOnlyConverter();
            return converter.Read(ref reader, typeof(DateOnly), new JsonSerializerOptions());
        };

        // Assert
        act.Should().Throw<JsonException>().WithMessage("*null*");
    }

    [Fact]
    public void Read_InvalidFormat_ThrowsJsonException()
    {
        // Arrange & Act
        var act = () =>
        {
            var json = "\"not-a-date\""u8;
            var reader = new Utf8JsonReader(json);
            reader.Read();
            var converter = new DateOnlyConverter();
            return converter.Read(ref reader, typeof(DateOnly), new JsonSerializerOptions());
        };

        // Assert
        act.Should().Throw<JsonException>().WithMessage("*Invalid date format*");
    }

    [Fact]
    public void Write_SerializesToExpectedFormat()
    {
        // Arrange
        var date = new DateOnly(2024, 1, 15);
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);
        var converter = new DateOnlyConverter();

        // Act
        converter.Write(writer, date, new JsonSerializerOptions());
        writer.Flush();
        var json = Encoding.UTF8.GetString(stream.ToArray());

        // Assert
        json.Should().Be("\"2024-01-15\"");
    }

    [Fact]
    public void RoundTrip_SerializeThenDeserialize_PreservesValue()
    {
        // Arrange
        var original = new DateOnly(2024, 6, 30);
        var options = CreateOptions();

        // Act
        var json = JsonSerializer.Serialize(original, options);
        var deserialized = JsonSerializer.Deserialize<DateOnly>(json, options);

        // Assert
        deserialized.Should().Be(original);
    }

    // --- DateOnlyDictionaryConverterFactory Tests ---

    [Fact]
    public void CanConvert_SortedDictionaryWithDateOnlyKey_ReturnsTrue()
    {
        // Arrange
        var factory = new DateOnlyDictionaryConverterFactory();

        // Act
        var result = factory.CanConvert(typeof(SortedDictionary<DateOnly, decimal>));

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanConvert_DictionaryWithDateOnlyKey_ReturnsTrue()
    {
        // Arrange
        var factory = new DateOnlyDictionaryConverterFactory();

        // Act
        var result = factory.CanConvert(typeof(Dictionary<DateOnly, decimal>));

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanConvert_DictionaryWithStringKey_ReturnsFalse()
    {
        // Arrange
        var factory = new DateOnlyDictionaryConverterFactory();

        // Act
        var result = factory.CanConvert(typeof(Dictionary<string, decimal>));

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void SortedDictionary_RoundTrip_PreservesValues()
    {
        // Arrange
        var original = new SortedDictionary<DateOnly, decimal>
        {
            { new DateOnly(2024, 1, 1), 100.50m },
            { new DateOnly(2024, 6, 15), 200.75m },
            { new DateOnly(2024, 12, 31), 300.25m }
        };
        var options = CreateOptions();

        // Act
        var json = JsonSerializer.Serialize(original, options);
        var deserialized = JsonSerializer.Deserialize<SortedDictionary<DateOnly, decimal>>(json, options);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized.Should().HaveCount(3);
        deserialized![new DateOnly(2024, 1, 1)].Should().Be(100.50m);
        deserialized[new DateOnly(2024, 6, 15)].Should().Be(200.75m);
        deserialized[new DateOnly(2024, 12, 31)].Should().Be(300.25m);
    }

    [Fact]
    public void Dictionary_RoundTrip_PreservesValues()
    {
        // Arrange
        var original = new Dictionary<DateOnly, decimal>
        {
            { new DateOnly(2024, 3, 10), 42.00m },
            { new DateOnly(2024, 7, 20), 99.99m }
        };
        var options = CreateOptions();

        // Act
        var json = JsonSerializer.Serialize(original, options);
        var deserialized = JsonSerializer.Deserialize<Dictionary<DateOnly, decimal>>(json, options);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized.Should().HaveCount(2);
        deserialized![new DateOnly(2024, 3, 10)].Should().Be(42.00m);
        deserialized[new DateOnly(2024, 7, 20)].Should().Be(99.99m);
    }
}
