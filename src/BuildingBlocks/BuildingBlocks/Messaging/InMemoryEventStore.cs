using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Events.Base;
using System.Collections.Concurrent;

namespace BuildingBlocks.Messaging
{
    /// <summary>
    /// A simple in-memory implementation of IEventStore for development and testing purposes.
    /// This implementation is NOT suitable for production.
    /// </summary>
    public class InMemoryEventStore : IEventStore
    {
        private static readonly ConcurrentDictionary<Guid, List<IntegrationEvent>> _events = new();

        public Task SaveAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IntegrationEvent
        {
            var aggregateId = @event.CorrelationId;

            _events.AddOrUpdate(aggregateId,
                new List<IntegrationEvent> { @event },
                (key, list) => { list.Add(@event); return list; });

            return Task.CompletedTask;
        }

        public Task<IEnumerable<IntegrationEvent>> GetEventsForAggregateAsync(Guid aggregateId, CancellationToken cancellationToken = default)
        {
            _events.TryGetValue(aggregateId, out var events);
            return Task.FromResult(events?.AsEnumerable() ?? Enumerable.Empty<IntegrationEvent>());
        }
    }
}