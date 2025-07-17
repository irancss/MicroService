namespace BuildingBlocks.Domain;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty; // نام کامل event، مثلا: "UserRegisteredEvent"
    public string Content { get; set; } = string.Empty; // event به صورت جیسون (JSON)
    public DateTime OccurredOnUtc { get; set; }
    public DateTime? ProcessedOnUtc { get; set; } // اگر null باشد یعنی هنوز پردازش نشده
    public string? Error { get; set; }
}