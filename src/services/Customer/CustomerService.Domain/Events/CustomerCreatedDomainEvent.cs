using BuildingBlocks.Domain.Events;

namespace CustomerService.Domain.Events
{
    public record CustomerCreatedDomainEvent(Guid CustomerId, string FullName, string Email) : DomainEvent;
}
