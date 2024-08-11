// Copyright (c) 2024 Pierre G. Boutquin. All rights reserved.
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

namespace Boutquin.Domain.Abstractions;

/// <summary>
/// Represents a base class for entities in the domain model, with a unique identifier and domain event handling capabilities.
/// </summary>
/// <typeparam name="TEntityId">The type of the identifier of the entity.</typeparam>
/// <remarks>
/// <para>
/// This abstract class provides the base functionality for any entity in the domain.
/// It includes an identifier and the ability to handle domain events, which are important aspects of domain-driven design.
/// </para>
/// <para>
/// Entities inheriting from this class can raise domain events that are collected and can be dispatched by the domain event handlers.
/// This is useful for decoupling the code that initiates the event from the code that handles the event.
/// </para>
/// </remarks>
/// <example>
/// Example of an entity inheriting from Entity:
/// <code>
/// public sealed class Order : Entity&lt;int&gt;
/// {
///     public Order(int id) : base(id) { }
///
///     public void AddOrderItem(OrderItem item)
///     {
///         // Order logic...
///         RaiseDomainEvent(new OrderItemAddedEvent(this.Id, item));
///     }
/// }
/// </code>
/// </example>
public abstract class Entity<TEntityId> : IEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Initializes a new instance of the Entity class with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    protected Entity(TEntityId id)
        => Id = id;

    /// <summary>
    /// Initializes a new instance of the Entity class without an identifier.
    /// </summary>
    protected Entity()
    {
    }

    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    public TEntityId Id { get; init; }

    /// <summary>
    /// Retrieves the list of domain events raised by the entity.
    /// </summary>
    /// <returns>A read-only list of domain events.</returns>
    public IReadOnlyList<IDomainEvent> GetDomainEvents()
        => _domainEvents.AsReadOnly();

    /// <summary>
    /// Clears the list of domain events.
    /// </summary>
    /// <remarks>
    /// This method should typically be called after the domain events have been processed to prevent the events from being processed again.
    /// </remarks>
    public void ClearDomainEvents()
        => _domainEvents.Clear();

    /// <summary>
    /// Raises a domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event to be raised.</param>
    /// <remarks>
    /// This method adds the provided domain event to the list of domain events to be processed.
    /// </remarks>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);
}
