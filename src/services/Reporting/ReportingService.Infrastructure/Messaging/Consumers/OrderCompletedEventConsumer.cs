using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using ReportingService.Application.Commands.ProcessOrderData;
using ReportingService.Domain.Events;

namespace ReportingService.Infrastructure.Messaging.Consumers;

/// <summary>
/// MassTransit consumer that receives OrderCompletedEvent and triggers MediatR command
/// This implements event-driven architecture where external events trigger data processing
/// </summary>
public class OrderCompletedEventConsumer : IConsumer<OrderCompletedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderCompletedEventConsumer> _logger;

    public OrderCompletedEventConsumer(IMediator mediator, ILogger<OrderCompletedEventConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
    {
        try
        {
            _logger.LogInformation("Received OrderCompletedEvent for OrderId: {OrderId}", context.Message.OrderId);

            // Create MediatR command from the received event
            var command = new ProcessOrderCompletedEventCommand
            {
                OrderEvent = context.Message
            };

            // Process the command using MediatR
            var result = await _mediator.Send(command);

            if (result.Success)
            {
                _logger.LogInformation("Successfully processed OrderCompletedEvent for OrderId: {OrderId}", context.Message.OrderId);
            }
            else
            {
                _logger.LogError("Failed to process OrderCompletedEvent for OrderId: {OrderId}. Error: {Error}", 
                    context.Message.OrderId, result.Message);
                
                // Could implement retry logic or dead letter queue here
                throw new InvalidOperationException($"Failed to process order data: {result.Message}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming OrderCompletedEvent for OrderId: {OrderId}", context.Message.OrderId);
            throw; // Let MassTransit handle retry logic
        }
    }
}
