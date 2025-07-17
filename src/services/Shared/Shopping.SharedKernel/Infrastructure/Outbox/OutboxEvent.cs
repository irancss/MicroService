using Shopping.SharedKernel.Domain.Entities;

namespace Shopping.SharedKernel.Infrastructure.Outbox;

public class OutboxEvent : BaseEntity
{
    public string EventType { get; set; } = string.Empty;
    public string EventData { get; set; } = string.Empty;
    public DateTime OccurredOn { get; set; }
    public bool IsProcessed { get; set; }
    public DateTime? ProcessedOn { get; set; }
    public string? Error { get; set; }
    public int RetryCount { get; set; }
}

public interface IOutboxEventRepository
{
    Task AddAsync(OutboxEvent outboxEvent, CancellationToken cancellationToken = default);
    Task<List<OutboxEvent>> GetUnprocessedEventsAsync(int batchSize = 100, CancellationToken cancellationToken = default);
    Task MarkAsProcessedAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task MarkAsFailedAsync(Guid eventId, string error, CancellationToken cancellationToken = default);
}

public interface IOutboxEventProcessor
{
    Task ProcessAsync(CancellationToken cancellationToken = default);
}
