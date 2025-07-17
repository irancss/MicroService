using Microsoft.EntityFrameworkCore;
using Shopping.SharedKernel.Infrastructure.Outbox;

namespace Shopping.SharedKernel.Infrastructure.Persistence;

public class OutboxEventRepository : IOutboxEventRepository
{
    private readonly DbContext _context;

    public OutboxEventRepository(DbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(OutboxEvent outboxEvent, CancellationToken cancellationToken = default)
    {
        await _context.Set<OutboxEvent>().AddAsync(outboxEvent, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<OutboxEvent>> GetUnprocessedEventsAsync(int batchSize = 100, CancellationToken cancellationToken = default)
    {
        return await _context.Set<OutboxEvent>()
            .Where(e => !e.IsProcessed)
            .OrderBy(e => e.OccurredOn)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsProcessedAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var outboxEvent = await _context.Set<OutboxEvent>()
            .FirstOrDefaultAsync(e => e.Id == eventId.ToString(), cancellationToken);

        if (outboxEvent != null)
        {
            outboxEvent.IsProcessed = true;
            outboxEvent.ProcessedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkAsFailedAsync(Guid eventId, string error, CancellationToken cancellationToken = default)
    {
        var outboxEvent = await _context.Set<OutboxEvent>()
            .FirstOrDefaultAsync(e => e.Id == eventId.ToString(), cancellationToken);

        if (outboxEvent != null)
        {
            outboxEvent.Error = error;
            outboxEvent.RetryCount++;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
