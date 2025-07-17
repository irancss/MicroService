

using MediatR;

namespace Shopping.SharedKernel.Application.CQRS.DomainEvents;

public interface IDomainEvent : INotification
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
}