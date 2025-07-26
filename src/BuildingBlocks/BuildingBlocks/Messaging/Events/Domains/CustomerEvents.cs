using BuildingBlocks.Messaging.Events.Base;

namespace BuildingBlocks.Messaging.Events.Domains
{
    public record CustomerRegisteredEvent : IntegrationEvent
    {
        public int CustomerId { get; init; }
        public string Email { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
    }
}