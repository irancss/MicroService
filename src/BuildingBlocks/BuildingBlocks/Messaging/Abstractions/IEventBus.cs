namespace BuildingBlocks.Messaging.Abstractions
{
     public interface IEventBus
    {
        Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : BaseEvent;
        Task PublishAsync<T>(IEnumerable<T> events, CancellationToken cancellationToken = default) where T : BaseEvent;
    }

    public interface IIntegrationEventHandler<in TIntegrationEvent> 
        where TIntegrationEvent : IIntegrationEvent
    {
        Task Handle(TIntegrationEvent @event, CancellationToken cancellationToken = default);
    }
}