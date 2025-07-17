using Microsoft.Extensions.Logging;
using Shopping.SharedKernel.Infrastructure.EventBus;
using Shopping.SharedKernel.Infrastructure.Outbox;
using System.Text.Json;

namespace Shopping.SharedKernel.Infrastructure.Persistence;

public class OutboxEventProcessor : IOutboxEventProcessor
{
    private readonly IOutboxEventRepository _outboxEventRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<OutboxEventProcessor> _logger;

    public OutboxEventProcessor(
        IOutboxEventRepository outboxEventRepository,
        IEventBus eventBus,
        ILogger<OutboxEventProcessor> logger)
    {
        _outboxEventRepository = outboxEventRepository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        const int batchSize = 100;
        const int maxRetries = 3;

        try
        {
            var unprocessedEvents = await _outboxEventRepository.GetUnprocessedEventsAsync(batchSize, cancellationToken);

            foreach (var outboxEvent in unprocessedEvents)
            {
                try
                {
                    // Skip events that have exceeded max retry attempts
                    if (outboxEvent.RetryCount >= maxRetries)
                    {
                        _logger.LogWarning("Skipping outbox event {EventId} due to max retries exceeded", outboxEvent.Id);
                        continue;
                    }

                    // Deserialize the event data
                    var eventType = Type.GetType(outboxEvent.EventType);
                    if (eventType == null)
                    {
                        _logger.LogError("Could not resolve event type {EventType} for outbox event {EventId}", 
                            outboxEvent.EventType, outboxEvent.Id);
                        await _outboxEventRepository.MarkAsFailedAsync(Guid.Parse(outboxEvent.Id), 
                            $"Could not resolve event type {outboxEvent.EventType}", cancellationToken);
                        continue;
                    }

                    var eventData = JsonSerializer.Deserialize(outboxEvent.EventData, eventType);
                    if (eventData == null)
                    {
                        _logger.LogError("Could not deserialize event data for outbox event {EventId}", outboxEvent.Id);
                        await _outboxEventRepository.MarkAsFailedAsync(Guid.Parse(outboxEvent.Id), 
                            "Could not deserialize event data", cancellationToken);
                        continue;
                    }

                    // Publish the event
                    await _eventBus.PublishAsync((dynamic)eventData, cancellationToken);

                    // Mark as processed
                    await _outboxEventRepository.MarkAsProcessedAsync(Guid.Parse(outboxEvent.Id), cancellationToken);

                    _logger.LogInformation("Successfully processed outbox event {EventId} of type {EventType}", 
                        outboxEvent.Id, outboxEvent.EventType);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing outbox event {EventId}", outboxEvent.Id);
                    await _outboxEventRepository.MarkAsFailedAsync(Guid.Parse(outboxEvent.Id), 
                        ex.Message, cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing outbox events");
            throw;
        }
    }
}
