using MassTransit;
using Microsoft.Extensions.Logging;
using DiscountService.Application.Features.Discounts.Commands;
using DiscountService.Domain.Events;
using MediatR;

namespace DiscountService.Infrastructure.Messaging;

/// <summary>
/// Consumer for OrderCreatedEvent from RabbitMQ
/// </summary>
public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderCreatedEventConsumer> _logger;

    public OrderCreatedEventConsumer(IMediator mediator, ILogger<OrderCreatedEventConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        try
        {
            _logger.LogInformation("Received OrderCreatedEvent for Order: {OrderId}, User: {UserId}", 
                context.Message.OrderId, context.Message.UserId);

            var command = new ProcessOrderCreatedCommand
            {
                OrderId = context.Message.OrderId,
                UserId = context.Message.UserId,
                UserEmail = context.Message.UserEmail,
                CartTotal = context.Message.CartTotal,
                FinalTotal = context.Message.FinalTotal,
                DiscountId = context.Message.DiscountId,
                CouponCode = context.Message.CouponCode,
                DiscountAmount = context.Message.DiscountAmount,
                OrderCreatedAt = context.Message.OrderCreatedAt
            };

            var result = await _mediator.Send(command);

            if (!result)
            {
                _logger.LogError("Failed to process OrderCreatedEvent for Order: {OrderId}", context.Message.OrderId);
                throw new Exception($"Failed to process order created event for order {context.Message.OrderId}");
            }

            _logger.LogInformation("Successfully processed OrderCreatedEvent for Order: {OrderId}", context.Message.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing OrderCreatedEvent for Order: {OrderId}", context.Message.OrderId);
            throw; // This will cause the message to be retried or moved to dead letter queue
        }
    }
}
