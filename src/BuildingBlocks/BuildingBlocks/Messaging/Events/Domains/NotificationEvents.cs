using BuildingBlocks.Messaging.Events.Base;

namespace BuildingBlocks.Messaging.Events.Domains
{
    public record NotificationSentDomainEvent : IntegrationEvent
    {
        public Guid NotificationId { get; init; }
        public Guid OriginalRequestCorrelationId { get; init; }
        public string NotificationType { get; init; } = string.Empty;
        public string Recipient { get; init; } = string.Empty;
        public bool IsSuccessful { get; init; }
        public string? ErrorMessage { get; init; }
        public DateTime SentAtUtc { get; init; }
    }

    public record NotificationInteractedEvent : IntegrationEvent
    {
        public Guid NotificationId { get; init; }
        public string InteractionType { get; init; } = string.Empty; // e.g., "Opened", "Clicked"
        public string Recipient { get; init; } = string.Empty;
        public DateTime InteractionAtUtc { get; init; }
        public Dictionary<string, string> Metadata { get; init; } = new();
    }
}