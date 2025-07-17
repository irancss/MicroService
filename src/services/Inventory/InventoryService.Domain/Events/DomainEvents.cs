namespace InventoryService.Domain.Events;

public abstract class DomainEvent
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime OccurredAt { get; protected set; } = DateTime.UtcNow;
}

public class LowStockDetectedEvent : DomainEvent
{
    public string ProductId { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int LowStockThreshold { get; set; }
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
}

public class ExcessStockDetectedEvent : DomainEvent
{
    public string ProductId { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int ExcessStockThreshold { get; set; }
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
}
