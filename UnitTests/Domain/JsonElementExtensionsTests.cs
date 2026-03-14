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

namespace Boutquin.UnitTests.Domain;

using System.Text.Json;

/// <summary>
/// Contains unit tests for the <see cref="JsonElementExtensions"/> class.
/// </summary>
public sealed class JsonElementExtensionsTests
{
    /// <summary>
    /// Simple record used for deserialization tests.
    /// </summary>
    private sealed record TestPerson(string Name, int Age);

    /// <summary>
    /// Tests that ToObject deserializes a JsonElement to a simple object.
    /// </summary>
    [Fact]
    public void ToObject_WithSimpleObject_DeserializesCorrectly()
    {
        // Arrange
        var json = """{"Name":"Alice","Age":30}""";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = element.ToObject<TestPerson>();

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Alice");
        result.Age.Should().Be(30);
    }

    /// <summary>
    /// Tests that ToObject respects custom JsonSerializerOptions.
    /// </summary>
    [Fact]
    public void ToObject_WithCustomOptions_DeserializesCorrectly()
    {
        // Arrange
        var json = """{"name":"Bob","age":25}""";
        var element = JsonDocument.Parse(json).RootElement;
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // Act
        var result = element.ToObject<TestPerson>(options);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Bob");
        result.Age.Should().Be(25);
    }

    /// <summary>
    /// Tests that ToObject returns null for a JSON null value when T is nullable.
    /// </summary>
    [Fact]
    public void ToObject_WithNullJsonValue_ReturnsNull()
    {
        // Arrange
        var json = "null";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = element.ToObject<TestPerson>();

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Tests round-trip: serialize object, parse to JsonElement, then ToObject.
    /// </summary>
    [Fact]
    public void ToObject_RoundTrip_PreservesValues()
    {
        // Arrange
        var original = new TestPerson("Charlie", 42);
        var json = JsonSerializer.Serialize(original);
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = element.ToObject<TestPerson>();

        // Assert
        result.Should().BeEquivalentTo(original);
    }
}
