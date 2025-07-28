using BuildingBlocks.Messaging.Events.Base;

namespace CustomerService.Application.Contracts
{
    public record CustomerRegisteredIntegrationEvent : IntegrationEvent
    {
        public Guid CustomerId { get; init; }
        public string FullName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
    }
}
