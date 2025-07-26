using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Events.Base;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Messaging; // [اصلاح شد] Namespace

/// <summary>
/// In-memory implementation of IEventBus for development/testing.
/// </summary>
public class InMemoryEventBus : IEventBus
{
    private readonly ILogger<InMemoryEventBus> _logger;

    public InMemoryEventBus(ILogger<InMemoryEventBus> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IntegrationEvent
    {
        _logger.LogInformation("Publishing event {EventType} with ID {EventId} (In-Memory)",
            typeof(T).Name, @event.Id);

        // In a real implementation, this would do nothing.
        // For testing, one might use a shared collection to verify events were "published".

        return Task.CompletedTask;
    }

    public Task PublishAsync<T>(IEnumerable<T> events, CancellationToken cancellationToken = default) where T : IntegrationEvent
    {
        foreach (var @event in events)
        {
            _logger.LogInformation("Publishing event {EventType} with ID {EventId} (In-Memory)",
                typeof(T).Name, @event.Id);
        }
        return Task.CompletedTask;
    }
}