using BuildingBlocks.Messaging.Events.Base;

namespace BuildingBlocks.Messaging.Events.Contracts
{
    public record NotificationRequestEvent(string Recipient, string Subject, string Message) : IntegrationEvent;

    public record NotificationSentEvent : IntegrationEvent
    {
        public Guid OriginalNotificationId { get; init; }
        public bool IsSuccessful { get; init; }
        public string? ErrorMessage { get; init; }
    }
}