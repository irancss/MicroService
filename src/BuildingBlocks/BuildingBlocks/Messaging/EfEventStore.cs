using System.Text.Json;
using BuildingBlocks.Application.Data;
using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Events.Base;
using BuildingBlocks.Messaging.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Messaging
{
    /// <summary>
    /// An Entity Framework Core based implementation of IEventStore.
    /// Persists events to a relational database for auditing or event sourcing.
    /// </summary>
    public class EfEventStore : IEventStore
    {
        private readonly IApplicationDbContext _dbContext;

        public EfEventStore(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<IntegrationEvent>> GetEventsForAggregateAsync(Guid aggregateId, CancellationToken cancellationToken = default)
        {
            var storedEvents = await _dbContext.StoredEvents
                .Where(e => e.AggregateId == aggregateId)
                .OrderBy(e => e.OccurredOnUtc)
                .ToListAsync(cancellationToken);

            if (!storedEvents.Any())
            {
                return Enumerable.Empty<IntegrationEvent>();
            }

            var events = new List<IntegrationEvent>();
            foreach (var storedEvent in storedEvents)
            {
                var eventType = Type.GetType(storedEvent.Type);
                if (eventType == null)
                {
                    // Log a warning here in a real application
                    continue;
                }

                var @event = JsonSerializer.Deserialize(storedEvent.Data, eventType) as IntegrationEvent;
                if (@event != null)
                {
                    events.Add(@event);
                }
            }

            return events;
        }

        public Task SaveAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IntegrationEvent
        {
            var storedEvent = new StoredEvent
            {
                Id = @event.Id,
                AggregateId = @event.CorrelationId,
                Type = @event.GetType().AssemblyQualifiedName!,
                Data = JsonSerializer.Serialize(@event, @event.GetType()),
                OccurredOnUtc = @event.OccurredOn
            };

            _dbContext.StoredEvents.Add(storedEvent);

            // IMPORTANT: This method only adds the event to the DbContext.
            // The caller is responsible for calling SaveChangesAsync() to commit the transaction.
            return Task.CompletedTask;
        }
    }
}