using Microsoft.Extensions.Logging;
using Shopping.SharedKernel.Application.CQRS.DomainEvents;
using Shopping.SharedKernel.Infrastructure.EventBus;

namespace Shopping.SharedKernel.Infrastructure.EventBus;

/// <summary>
/// In-memory implementation of IEventBus for development/testing
/// For production, replace with RabbitMQ, Azure Service Bus, or Kafka implementation
/// </summary>
public class InMemoryEventBus : IEventBus
{
    private readonly ILogger<InMemoryEventBus> _logger;

    public InMemoryEventBus(ILogger<InMemoryEventBus> logger)
    {
        _logger = logger;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IDomainEvent
    {
        _logger.LogInformation("Publishing event {EventType} with ID {EventId}", 
            typeof(T).Name, @event.Id);

        // In a real implementation, this would publish to a message broker
        // For now, just log the event
        await Task.CompletedTask;
        
        _logger.LogInformation("Event {EventType} with ID {EventId} published successfully", 
            typeof(T).Name, @event.Id);
    }

    public async Task PublishAsync<T>(T @event, string routingKey, CancellationToken cancellationToken = default) where T : IDomainEvent
    {
        _logger.LogInformation("Publishing event {EventType} with ID {EventId} to routing key {RoutingKey}", 
            typeof(T).Name, @event.Id, routingKey);

        // In a real implementation, this would publish to a specific route/topic
        await Task.CompletedTask;
        
        _logger.LogInformation("Event {EventType} with ID {EventId} published successfully to routing key {RoutingKey}", 
            typeof(T).Name, @event.Id, routingKey);
    }
}

/// <summary>
/// In-memory implementation of IIntegrationEventBus for development/testing
/// </summary>
public class InMemoryIntegrationEventBus : IIntegrationEventBus
{
    private readonly ILogger<InMemoryIntegrationEventBus> _logger;

    public InMemoryIntegrationEventBus(ILogger<InMemoryIntegrationEventBus> logger)
    {
        _logger = logger;
    }

    public async Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) where T : IIntegrationEvent
    {
        _logger.LogInformation("Publishing integration event {EventType} with ID {EventId}", 
            integrationEvent.EventType, integrationEvent.Id);

        // In a real implementation, this would publish to a message broker
        await Task.CompletedTask;
        
        _logger.LogInformation("Integration event {EventType} with ID {EventId} published successfully", 
            integrationEvent.EventType, integrationEvent.Id);
    }
}
