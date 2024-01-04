// Copyright (c) 2023-2024 Pierre G. Boutquin. All rights reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License").
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//
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
/// public class OrderPlacedEvent : IDomainEvent
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
