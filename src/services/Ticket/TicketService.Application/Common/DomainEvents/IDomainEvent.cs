using MediatR;

namespace TicketService.Application.Common.DomainEvents;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}