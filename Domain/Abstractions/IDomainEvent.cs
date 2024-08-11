// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Boutquin.Domain.Abstractions;

using MediatR;

/// <summary>
/// Represents a domain event within the system.
/// </summary>
/// <remarks>
/// <para>
/// A domain event is an event that is significant within the domain of the application.
/// Implementing this interface allows an object to be dispatched by MediatR as a notification,
/// enabling a decoupled, event-driven architecture.
/// </para>
/// <para>
/// In a domain-driven design context, domain events are used to model important occurrences within the domain.
/// These events are typically things that have happened in the past (e.g., OrderPlaced, ItemShipped, etc.).
/// By implementing the <see cref="INotification"/> interface from MediatR, domain events can be dispatched
/// to relevant handlers which can then perform specific actions in response to the event.
/// </para>
/// <para>
/// Domain events are a key component of event-driven architectures and can be used to facilitate
/// various aspects of system design such as eventual consistency, integration with external systems,
/// and complex business workflows.
/// </para>
/// <para>
/// The interface itself does not define any members; it serves as a marker interface to signify that
/// a class is a domain event. Implementors of this interface should contain properties that provide
/// details about the event itself.
/// </para>
/// </remarks>
/// <example>
/// Example of a domain event implementation:
/// <code>
/// public sealed class OrderPlacedEvent : IDomainEvent
/// {
///     public Guid OrderId { get; }
///     public DateTime OrderDate { get; }
/// 
///     public OrderPlacedEvent(Guid orderId, DateTime orderDate)
///     {
///         OrderId = orderId;
///         OrderDate = orderDate;
///     }
/// }
/// </code>
/// </example>
public interface IDomainEvent : INotification
{
}
