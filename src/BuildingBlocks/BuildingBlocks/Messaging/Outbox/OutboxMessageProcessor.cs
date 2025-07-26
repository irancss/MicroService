using BuildingBlocks.Application.Data;
using BuildingBlocks.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Reflection;

namespace BuildingBlocks.Messaging.Outbox
{
    public class OutboxOptions
    {
        public int IntervalInSeconds { get; set; } = 5;
        public int BatchSize { get; set; } = 100;
    }

    /// <summary>
    /// [نکته] این پیاده‌سازی واحد و استاندارد برای الگوی Outbox است.
    /// A background service that periodically processes messages from the transactional outbox
    /// and publishes them to the event bus.
    /// </summary>
    public class OutboxMessageProcessor : BackgroundService
    {
        private readonly ILogger<OutboxMessageProcessor> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly OutboxOptions _options;
        private static readonly MethodInfo? PublishMethod;

        static OutboxMessageProcessor()
        {
            // [بهینه‌سازی] متد Publish<T> یک بار با Reflection پیدا و کش می‌شود.
            PublishMethod = typeof(IEventBus).GetMethod(nameof(IEventBus.PublishAsync));
        }

        public OutboxMessageProcessor(
            ILogger<OutboxMessageProcessor> logger,
            IServiceProvider serviceProvider,
            IOptions<OutboxOptions> options)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox Message Processor is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessOutboxMessagesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing outbox messages.");
                }

                await Task.Delay(TimeSpan.FromSeconds(_options.IntervalInSeconds), stoppingToken);
            }

            _logger.LogInformation("Outbox Message Processor is stopping.");
        }

        private async Task ProcessOutboxMessagesAsync(CancellationToken stoppingToken)
        {
            _logger.LogTrace("Checking for new messages in the outbox...");

            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
            var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

            var messages = await dbContext.OutboxMessages
                .Where(m => m.ProcessedOnUtc == null)
                .OrderBy(m => m.OccurredOnUtc)
                .Take(_options.BatchSize)
                .ToListAsync(stoppingToken);

            if (!messages.Any())
            {
                _logger.LogTrace("No new outbox messages found.");
                return;
            }

            _logger.LogInformation("Found {MessageCount} new messages in outbox to process.", messages.Count);

            foreach (var message in messages)
            {
                try
                {
                    var eventType = Type.GetType(message.Type);
                    if (eventType == null)
                    {
                        _logger.LogError("Could not find type '{EventType}' for outbox message {MessageId}.", message.Type, message.Id);
                        message.Error = $"Type '{message.Type}' not found.";
                        message.ProcessedOnUtc = DateTime.UtcNow;
                        continue;
                    }

                    var @event = JsonSerializer.Deserialize(message.Content, eventType);
                    if (@event == null)
                    {
                        _logger.LogError("Failed to deserialize content for outbox message {MessageId}.", message.Id);
                        message.Error = "Deserialization failed.";
                        message.ProcessedOnUtc = DateTime.UtcNow;
                        continue;
                    }

                    if (PublishMethod == null)
                    {
                        throw new InvalidOperationException("Could not find PublishAsync method on IEventBus.");
                    }

                    var genericPublishMethod = PublishMethod.MakeGenericMethod(eventType);
                    await (Task)genericPublishMethod.Invoke(eventBus, new object?[] { @event, stoppingToken })!;

                    message.ProcessedOnUtc = DateTime.UtcNow;
                    message.Error = null;

                    _logger.LogInformation("Successfully processed and published outbox message {MessageId} of type {EventType}.", message.Id, message.Type);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process outbox message {MessageId}.", message.Id);
                    message.Error = ex.Message;
                }
            }

            await dbContext.SaveChangesAsync(stoppingToken);
        }
    }
}