using BuildingBlocks.Messaging.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Messaging.MassTransit
{
    public class MassTransitEventBus : IEventBus
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<MassTransitEventBus> _logger;

        public MassTransitEventBus(IPublishEndpoint publishEndpoint, ILogger<MassTransitEventBus> logger)
        {
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IIntegrationEvent
        {
            try
            {
                _logger.LogInformation("Publishing integration event: {EventType} with ID: {EventId}", @event.EventType, @event.Id);
                
                await _publishEndpoint.Publish(@event, cancellationToken);
                
                _logger.LogInformation("Successfully published integration event: {EventType} with ID: {EventId}", @event.EventType, @event.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing integration event: {EventType} with ID: {EventId}", @event.EventType, @event.Id);
                throw;
            }
        }
    }
}