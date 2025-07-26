using BuildingBlocks.Messaging.Events.Base;

namespace BuildingBlocks.Messaging.Abstractions;

/// <summary>
/// Defines a contract for persisting and retrieving integration events.
/// This is useful for auditing, replaying events, or implementing event sourcing.
/// </summary>
public interface IEventStore
{
    /// <summary>
    /// Saves an event to the store.
    /// </summary>
    Task SaveAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IntegrationEvent;

    /// <summary>
    /// Retrieves all events for a specific aggregate.
    /// </summary>
    Task<IEnumerable<IntegrationEvent>> GetEventsForAggregateAsync(Guid aggregateId, CancellationToken cancellationToken = default);
}