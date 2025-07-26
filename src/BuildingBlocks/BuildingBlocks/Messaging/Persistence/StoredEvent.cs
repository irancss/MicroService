namespace BuildingBlocks.Messaging.Persistence
{
    /// <summary>
    /// Represents an event stored in the database for auditing or replaying purposes.
    /// </summary>
    public class StoredEvent
    {
        public Guid Id { get; set; }
        public Guid AggregateId { get; set; }
        public string Type { get; set; } = string.Empty; // Assembly-qualified name of the event type
        public string Data { get; set; } = string.Empty; // Event data serialized as JSON
        public DateTime OccurredOnUtc { get; set; }
        public string? User { get; set; } // Optional: who triggered the event
    }
}