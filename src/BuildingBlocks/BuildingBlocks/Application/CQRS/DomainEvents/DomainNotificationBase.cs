using System.Text.Json.Serialization;
using BuildingBlocks.Domain.Events; // [اصلاح شد] ارجاع به IDomainEvent صحیح

namespace BuildingBlocks.Application.CQRS.DomainEvents;

/// <summary>
/// A base implementation for a MediatR notification that wraps a domain event.
/// </summary>
/// <typeparam name="T">The type of the domain event.</typeparam>
public class DomainNotificationBase<T> : IDomainEventNotification<T>
    where T : IDomainEvent
{
    [JsonIgnore]
    public T DomainEvent { get; }

    public Guid Id { get; }

    public DomainNotificationBase(T domainEvent)
    {
        Id = Guid.NewGuid();
        DomainEvent = domainEvent;
    }
}