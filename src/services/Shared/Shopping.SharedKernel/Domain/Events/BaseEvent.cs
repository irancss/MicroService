
using MediatR;
using Shopping.SharedKernel.Application.CQRS.DomainEvents;

namespace Shopping.SharedKernel.Domain.Events;

public abstract class BaseEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}