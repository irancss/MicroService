using Shopping.SharedKernel.Application.CQRS.DomainEvents;

namespace Shopping.SharedKernel.Infrastructure.EventBus;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IDomainEvent;
    Task PublishAsync<T>(T @event, string routingKey, CancellationToken cancellationToken = default) where T : IDomainEvent;
}

public interface IIntegrationEventBus
{
    Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) where T : IIntegrationEvent;
}

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
    string EventType { get; }
}

public abstract class IntegrationEventBase : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType => GetType().Name;
}
