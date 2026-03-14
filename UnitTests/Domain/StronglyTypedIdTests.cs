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

/// <summary>
/// A strongly typed identifier backed by a Guid, for testing purposes.
/// </summary>
public record TestId(Guid Value) : StronglyTypedId<Guid>(Value);

/// <summary>
/// A strongly typed identifier backed by an int, for testing purposes.
/// </summary>
public record IntId(int Value) : StronglyTypedId<int>(Value);

/// <summary>
/// Contains unit tests for the <see cref="StronglyTypedId{TValue}"/> abstract record.
/// </summary>
public sealed class StronglyTypedIdTests
{
    [Fact]
    public void ToString_ReturnsValueString()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var id = new TestId(guid);

        // Act
        var result = id.ToString();

        // Assert
        result.Should().Be(guid.ToString());
    }

    [Fact]
    public void Equals_SameGuid_ReturnsTrue()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var id1 = new TestId(guid);
        var id2 = new TestId(guid);

        // Act & Assert
        id1.Should().Be(id2);
    }

    [Fact]
    public void Equals_DifferentGuids_ReturnsFalse()
    {
        // Arrange
        var id1 = new TestId(Guid.NewGuid());
        var id2 = new TestId(Guid.NewGuid());

        // Act & Assert
        id1.Should().NotBe(id2);
    }

    [Fact]
    public void IntId_ToString_ReturnsValueString()
    {
        // Arrange
        var id = new IntId(42);

        // Act
        var result = id.ToString();

        // Assert
        result.Should().Be("42");
    }

    [Fact]
    public void IntId_SameValue_AreEqual()
    {
        // Arrange
        var id1 = new IntId(7);
        var id2 = new IntId(7);

        // Act & Assert
        id1.Should().Be(id2);
    }

    [Fact]
    public void IntId_DifferentValues_AreNotEqual()
    {
        // Arrange
        var id1 = new IntId(1);
        var id2 = new IntId(2);

        // Act & Assert
        id1.Should().NotBe(id2);
    }
}
