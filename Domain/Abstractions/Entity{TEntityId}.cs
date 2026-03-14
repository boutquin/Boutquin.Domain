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

namespace Boutquin.Domain.Abstractions;

/// <summary>
/// Represents a base class for entities in the domain model, with a unique identifier and
/// domain event handling capabilities.
/// </summary>
/// <typeparam name="TEntityId">The type of the identifier of the entity.</typeparam>
/// <remarks>
/// <para>
/// <b>Why identity-based equality (not state-based):</b> In Domain-Driven Design, an <em>entity</em>
/// is defined by its identity, not by the values of its attributes. Two <c>Customer</c> objects with
/// the same <c>Id</c> represent the same real-world customer even if one has an updated address. This
/// is the fundamental distinction between entities and value objects: entities have continuity of
/// identity across state changes, while value objects are interchangeable when their attributes match.
/// All equality operations in this class (<c>Equals</c>, <c>GetHashCode</c>, <c>==</c>, <c>!=</c>)
/// compare only the <c>Id</c> property — attribute values are intentionally ignored.
/// </para>
/// <para>
/// <b>Why domain events are buffered in a private list (not dispatched immediately):</b> Domain events
/// represent side effects of an operation (e.g., "an order was placed"). If events were dispatched
/// at the moment they are raised, the handlers would execute <em>within</em> the current transaction —
/// before the aggregate's state change is committed. This creates two problems:
/// </para>
/// <list type="number">
///   <item>
///     <description>
///       <b>Consistency:</b> If the transaction rolls back after an event handler has already sent
///       an email or updated another aggregate, the system enters an inconsistent state. Buffering
///       events until after the transaction commits (via <see cref="GetDomainEvents"/> and
///       <see cref="ClearDomainEvents"/>) ensures handlers only run when the state change is durable.
///     </description>
///   </item>
///   <item>
///     <description>
///       <b>Ordering:</b> Multiple events may be raised during a single operation. Buffering them
///       in a list preserves their chronological order, so handlers process them in the same sequence
///       they were raised — which matters when one handler depends on the effects of a prior one.
///     </description>
///   </item>
/// </list>
/// <para>
/// The typical consumption pattern is: the persistence layer (e.g., a <c>SaveChanges</c> interceptor)
/// calls <see cref="GetDomainEvents"/> to retrieve buffered events, dispatches them to their handlers
/// (via MediatR, an in-process bus, or similar), and then calls <see cref="ClearDomainEvents"/> to
/// prevent re-processing on the next save.
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
public abstract class Entity<TEntityId> : IEntity, IEquatable<Entity<TEntityId>>
{
    // Events are stored in a private, mutable list — not exposed directly. Consumers access them
    // through GetDomainEvents() which returns a read-only view. This ensures only the entity itself
    // (via RaiseDomainEvent) can add events, maintaining the invariant that events correspond to
    // actual domain operations performed on this entity.
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
    /// <remarks>
    /// <para>
    /// <b>Why a parameterless constructor exists:</b> ORM frameworks (Entity Framework Core, Dapper, etc.)
    /// require the ability to instantiate entity objects during materialization (hydrating objects from
    /// database query results). These frameworks use reflection to create instances and then set
    /// properties — they cannot call parameterized constructors reliably across all mapping scenarios.
    /// </para>
    /// <para>
    /// The constructor is <c>protected</c> (not <c>public</c>) to prevent application code from
    /// accidentally creating entities without an identity. Only the ORM (which uses reflection and
    /// bypasses access modifiers) and derived entity classes can call it.
    /// </para>
    /// </remarks>
    protected Entity()
    {
    }

    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Why <c>init</c> instead of <c>private set</c>:</b> The <c>init</c> accessor allows the
    /// property to be set during object initialization (including by ORMs using reflection) but
    /// prevents it from being changed afterward. This enforces the DDD principle that an entity's
    /// identity is immutable once established — an entity cannot change its identity during its lifetime.
    /// </para>
    /// <para>
    /// <b>Why <c>default!</c>:</b> The null-forgiving operator suppresses the nullable warning for
    /// the case where the parameterless constructor is used (ORM materialization). The ORM will set
    /// the <c>Id</c> immediately after construction, so the brief window where <c>Id</c> holds
    /// <c>default</c> is an implementation detail of the materialization process, not observable
    /// by application code.
    /// </para>
    /// </remarks>
    public TEntityId Id { get; init; } = default!;

    /// <summary>
    /// Retrieves the list of domain events raised by the entity.
    /// </summary>
    /// <remarks>
    /// Returns a read-only view to prevent external code from adding, removing, or reordering events.
    /// The entity is the sole authority on what events occurred — external code can only observe and
    /// clear them.
    /// </remarks>
    /// <returns>A read-only list of domain events.</returns>
    public IReadOnlyList<IDomainEvent> GetDomainEvents()
        => _domainEvents.AsReadOnly();

    /// <summary>
    /// Clears the list of domain events.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Why this must be called after dispatching:</b> Without clearing, the same events would be
    /// re-dispatched on the next <c>SaveChanges</c> call, causing duplicate side effects (double emails,
    /// double audit log entries, etc.). The typical pattern is:
    /// </para>
    /// <code>
    /// var events = entity.GetDomainEvents();
    /// entity.ClearDomainEvents();     // clear BEFORE dispatching to prevent re-entrancy issues
    /// foreach (var e in events) { await mediator.Publish(e); }
    /// </code>
    /// <para>
    /// Clearing before dispatching (rather than after) prevents a subtle bug: if a handler triggers
    /// another <c>SaveChanges</c> that re-reads the entity's events, clearing afterward would cause
    /// the original events to be dispatched again during the nested save.
    /// </para>
    /// </remarks>
    public void ClearDomainEvents()
        => _domainEvents.Clear();

    /// <summary>
    /// Raises a domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event to be raised.</param>
    /// <remarks>
    /// <para>
    /// <b>Why <c>protected</c> (not <c>public</c>):</b> Only the entity itself should decide what
    /// events its operations produce. Making this <c>public</c> would allow external code to inject
    /// arbitrary events into an entity's event stream, breaking the invariant that events reflect
    /// actual domain operations. Derived classes call this method from within their domain methods
    /// (e.g., <c>Order.AddOrderItem</c> raises <c>OrderItemAddedEvent</c>).
    /// </para>
    /// <para>
    /// <b>Why "raise" not "publish":</b> The event is not published (dispatched to handlers) here —
    /// it is only added to the buffer. The terminology "raise" reflects this: the entity declares that
    /// something happened, and the infrastructure layer decides when and how to publish it.
    /// </para>
    /// </remarks>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);

    /// <summary>
    /// Determines whether this entity is equal to another entity of the same type based on identity (Id).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Why the default-Id guard (transient entity check):</b> An entity whose <c>Id</c> equals
    /// <c>default</c> (e.g., <c>0</c> for <c>int</c>, <c>null</c> for a reference type, or
    /// <c>Guid.Empty</c> for <c>Guid</c>) has not yet been persisted — it is a <em>transient</em>
    /// entity. Two transient entities should never be considered equal just because they both have
    /// a default Id, since they are distinct objects that will receive different identities when saved.
    /// Without this guard, all newly created (unsaved) entities of the same type would compare as
    /// equal — which breaks collection operations like <c>HashSet</c> deduplication and EF Core
    /// change tracking.
    /// </para>
    /// <para>
    /// <b>Why <c>ReferenceEquals</c> is checked early:</b> This is both a performance optimization
    /// (skip the comparer when comparing an object to itself) and a correctness requirement (a
    /// transient entity must still equal itself, even though its Id is <c>default</c>).
    /// </para>
    /// </remarks>
    /// <param name="other">The entity to compare with.</param>
    /// <returns><c>true</c> if the entities have the same Id; otherwise, <c>false</c>.</returns>
    public bool Equals(Entity<TEntityId>? other)
    {
        if (other is null)
        {
            return false;
        }

        // Same reference → always equal, even if transient (Id == default).
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        // If either entity has a default Id, it is transient and must not match anything
        // except itself (handled by the ReferenceEquals check above).
        if (EqualityComparer<TEntityId>.Default.Equals(Id, default!) ||
            EqualityComparer<TEntityId>.Default.Equals(other.Id, default!))
        {
            return false;
        }

        return EqualityComparer<TEntityId>.Default.Equals(Id, other.Id);
    }

    /// <summary>
    /// Determines whether this entity is equal to another object based on identity (Id).
    /// </summary>
    /// <remarks>
    /// Delegates to the strongly-typed <see cref="Equals(Entity{TEntityId}?)"/> after a type check.
    /// Two entities of different <c>TEntityId</c> types are never equal, even if their Ids have the
    /// same underlying value — this prevents accidental cross-type identity collisions
    /// (e.g., <c>Order(id=42)</c> should never equal <c>Customer(id=42)</c>).
    /// </remarks>
    /// <param name="obj">The object to compare with.</param>
    /// <returns><c>true</c> if the objects are of the same type and have the same Id; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
        => obj is Entity<TEntityId> other && Equals(other);

    /// <summary>
    /// Returns a hash code based on the entity's Id.
    /// </summary>
    /// <remarks>
    /// <b>Why only Id is hashed:</b> Since <c>Equals</c> compares only <c>Id</c>, the hash code
    /// must also be derived only from <c>Id</c> to satisfy the contract: if <c>a.Equals(b)</c>,
    /// then <c>a.GetHashCode() == b.GetHashCode()</c>. Including attribute values in the hash
    /// would violate this invariant — two entities with the same Id but different attributes would
    /// have different hash codes while comparing as equal, breaking hash-based collections.
    /// </remarks>
    /// <returns>A hash code for the entity.</returns>
    public override int GetHashCode()
        => EqualityComparer<TEntityId>.Default.GetHashCode(Id!);

    /// <summary>
    /// Determines whether two entities are equal based on identity.
    /// </summary>
    /// <remarks>
    /// Delegates to <see cref="object.Equals(object?, object?)"/> which handles null checks and
    /// calls the instance <see cref="Equals(object?)"/> method — ensuring the same identity-based
    /// semantics are used regardless of whether the caller uses <c>==</c> or <c>.Equals()</c>.
    /// </remarks>
    public static bool operator ==(Entity<TEntityId>? left, Entity<TEntityId>? right)
        => Equals(left, right);

    /// <summary>
    /// Determines whether two entities are not equal based on identity.
    /// </summary>
    public static bool operator !=(Entity<TEntityId>? left, Entity<TEntityId>? right)
        => !Equals(left, right);
}
