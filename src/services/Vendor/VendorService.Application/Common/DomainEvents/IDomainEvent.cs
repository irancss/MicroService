using MediatR;

namespace VendorService.Application.Common.DomainEvents;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}