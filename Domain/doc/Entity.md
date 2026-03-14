# Class Name: Entity&lt;TEntityId&gt;

**Namespace:** `Boutquin.Domain.Abstractions`
**Inherits:** `IEntity`, `IEquatable<Entity<TEntityId>>`

Abstract base class for entities in the domain model, providing identity-based equality and domain event buffering.

## Design Rationale

In Domain-Driven Design, an **entity** is defined by its identity — not by the values of its attributes. Two `Customer` objects with the same `Id` represent the same real-world customer even if one has an updated address. All equality operations in this class compare only the `Id` property.

Domain events are buffered in a private list rather than dispatched immediately. This ensures event handlers only run after the transaction commits, preventing inconsistent state if the transaction rolls back.

## Constructors

### `protected Entity(TEntityId id)`

Creates an entity with the specified identity.

### `protected Entity()`

Parameterless constructor for ORM frameworks (Entity Framework Core, Dapper, etc.) that need to instantiate entities via reflection during materialization. Protected to prevent application code from creating entities without an identity.

## Properties

### `TEntityId Id { get; init; }`

The unique identifier of the entity. Uses `init` (not `set`) to enforce immutable identity — once established, an entity cannot change its Id. The `default!` initializer suppresses nullable warnings for the ORM materialization window.

## Methods

### `IReadOnlyList<IDomainEvent> GetDomainEvents()`

Returns a read-only view of the domain events buffered by this entity. External code can observe events but cannot add or remove them.

### `void ClearDomainEvents()`

Clears the buffered domain events. Must be called after dispatching to prevent re-processing. Best practice: clear *before* dispatching to prevent re-entrancy issues in nested `SaveChanges` scenarios.

### `protected void RaiseDomainEvent(IDomainEvent domainEvent)`

Adds a domain event to the buffer. Protected because only the entity itself should determine what events its operations produce. The term "raise" (vs "publish") reflects that the event is buffered, not dispatched.

### `bool Equals(Entity<TEntityId>? other)`

Identity-based equality. Returns `false` if either entity has a default Id (transient entities must not match). `ReferenceEquals` is checked first so that a transient entity still equals itself.

### `override bool Equals(object? obj)`

Delegates to the strongly-typed `Equals` after a type check. Prevents cross-type identity collisions (e.g., `Order(id=42)` never equals `Customer(id=42)`).

### `override int GetHashCode()`

Hash code derived from `Id` only — required by the `Equals`/`GetHashCode` contract.

### `operator ==` / `operator !=`

Delegate to `object.Equals(object?, object?)` for consistent identity-based semantics.

## Related Interfaces

### `IEntity`

Defines the domain event contract:
- `IReadOnlyList<IDomainEvent> GetDomainEvents()`
- `void ClearDomainEvents()`

### `IDomainEvent`

Marker interface extending MediatR's `INotification`. Domain event implementations carry properties describing what happened.

### `IUnitOfWork`

Defines `Task<int> SaveChangesAsync(CancellationToken)` — the persistence boundary where domain events are typically dispatched.

## Example

```csharp
public sealed class Order : Entity<int>
{
    public Order(int id) : base(id) { }

    public void AddOrderItem(OrderItem item)
    {
        // Business logic...
        RaiseDomainEvent(new OrderItemAddedEvent(Id, item));
    }
}

// Typical dispatch pattern (in SaveChanges interceptor):
var events = entity.GetDomainEvents();
entity.ClearDomainEvents();
foreach (var e in events) { await mediator.Publish(e); }
```
