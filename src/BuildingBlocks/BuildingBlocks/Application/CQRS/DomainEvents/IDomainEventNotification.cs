using BuildingBlocks.Domain.Events;
using MediatR;

namespace BuildingBlocks.Application.CQRS.DomainEvents;

/// <summary>
/// A MediatR notification that wraps a generic domain event.
/// </summary>
public interface IDomainEventNotification<out TEventType> : IDomainEventNotification where TEventType : IDomainEvent
{
    TEventType DomainEvent { get; }
}

/// <summary>
/// A marker interface for a domain event notification.
/// </summary>
public interface IDomainEventNotification : INotification
{
    Guid Id { get; }
}