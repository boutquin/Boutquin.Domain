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

using Boutquin.Domain.Abstractions;

namespace Boutquin.UnitTests.Domain;

/// <summary>
/// A test domain event used to verify domain event handling in Entity.
/// </summary>
public record TestDomainEvent(string Data) : IDomainEvent;

/// <summary>
/// A concrete entity implementation for testing purposes.
/// </summary>
public sealed class TestEntity : Entity<int>
{
    public TestEntity(int id) : base(id)
    {
    }

    public TestEntity()
    {
    }

    /// <summary>
    /// Exposes the protected RaiseDomainEvent method for testing.
    /// </summary>
    public void RaiseEvent(IDomainEvent domainEvent)
        => RaiseDomainEvent(domainEvent);
}

/// <summary>
/// A different entity type used to test cross-type equality comparisons.
/// </summary>
public sealed class OtherEntity : Entity<int>
{
    public OtherEntity(int id) : base(id)
    {
    }
}

/// <summary>
/// Contains unit tests for the <see cref="Entity{TEntityId}"/> abstract class.
/// </summary>
public sealed class EntityTests
{
    [Fact]
    public void Equals_SameId_ReturnsTrue()
    {
        // Arrange
        var entity1 = new TestEntity(1);
        var entity2 = new TestEntity(1);

        // Act & Assert
        entity1.Equals(entity2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentIds_ReturnsFalse()
    {
        // Arrange
        var entity1 = new TestEntity(1);
        var entity2 = new TestEntity(2);

        // Act & Assert
        entity1.Equals(entity2).Should().BeFalse();
    }

    [Fact]
    public void Equals_ComparedToNull_ReturnsFalse()
    {
        // Arrange
        var entity = new TestEntity(1);

        // Act & Assert
        entity.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void Equals_NonEntityObject_ReturnsFalse()
    {
        // Arrange
        var entity = new TestEntity(1);

        // Act & Assert
        entity.Equals("not an entity").Should().BeFalse();
    }

    [Fact]
    public void Equals_SameReference_ReturnsTrue()
    {
        // Arrange
        var entity = new TestEntity(1);

        // Act & Assert
        entity.Equals(entity).Should().BeTrue();
    }

    [Fact]
    public void Equals_DefaultIdVsSetId_ReturnsFalse()
    {
        // Arrange
        var transientEntity = new TestEntity(); // default Id (0)
        var persistedEntity = new TestEntity(1);

        // Act & Assert
        transientEntity.Equals(persistedEntity).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SameId_ReturnsSameValue()
    {
        // Arrange
        var entity1 = new TestEntity(42);
        var entity2 = new TestEntity(42);

        // Act & Assert
        entity1.GetHashCode().Should().Be(entity2.GetHashCode());
    }

    [Fact]
    public void EqualityOperator_SameId_ReturnsTrue()
    {
        // Arrange
        var entity1 = new TestEntity(1);
        var entity2 = new TestEntity(1);

        // Act & Assert
        (entity1 == entity2).Should().BeTrue();
    }

    [Fact]
    public void InequalityOperator_DifferentIds_ReturnsTrue()
    {
        // Arrange
        var entity1 = new TestEntity(1);
        var entity2 = new TestEntity(2);

        // Act & Assert
        (entity1 != entity2).Should().BeTrue();
    }

    [Fact]
    public void InequalityOperator_SameId_ReturnsFalse()
    {
        // Arrange
        var entity1 = new TestEntity(1);
        var entity2 = new TestEntity(1);

        // Act & Assert
        (entity1 != entity2).Should().BeFalse();
    }

    [Fact]
    public void RaiseDomainEvent_AddsEventToGetDomainEvents()
    {
        // Arrange
        var entity = new TestEntity(1);
        var domainEvent = new TestDomainEvent("test");

        // Act
        entity.RaiseEvent(domainEvent);

        // Assert
        entity.GetDomainEvents().Should().ContainSingle()
            .Which.Should().Be(domainEvent);
    }

    [Fact]
    public void ClearDomainEvents_RemovesAllEvents()
    {
        // Arrange
        var entity = new TestEntity(1);
        entity.RaiseEvent(new TestDomainEvent("event1"));
        entity.RaiseEvent(new TestDomainEvent("event2"));

        // Act
        entity.ClearDomainEvents();

        // Assert
        entity.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void ParameterlessConstructor_CreatesEntityWithDefaultId()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.Id.Should().Be(default(int));
    }
}
