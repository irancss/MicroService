using MassTransit;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Messaging.Events;

namespace BuildingBlocks.Messaging.Handlers
{
    /// <summary>
    /// Sample event handler for OrderCreatedEvent
    /// </summary>
    public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedEventHandler> _logger;

        public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var orderEvent = context.Message;
            
            _logger.LogInformation("Processing OrderCreatedEvent for Order {OrderId}, Amount: {Amount}, Customer: {CustomerEmail}",
                orderEvent.OrderId, orderEvent.Amount, orderEvent.CustomerEmail);

            // Process the order created event
            // For example: Send email notification, update inventory, etc.
            
            await Task.Delay(1000); // Simulate processing time
            
            _logger.LogInformation("OrderCreatedEvent processed successfully for Order {OrderId}", orderEvent.OrderId);
        }
    }

    /// <summary>
    /// Sample event handler for PaymentProcessedEvent
    /// </summary>
    public class PaymentProcessedEventHandler : IEventHandler<PaymentProcessedEvent>
    {
        private readonly ILogger<PaymentProcessedEventHandler> _logger;

        public PaymentProcessedEventHandler(ILogger<PaymentProcessedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
        {
            var paymentEvent = context.Message;
            
            _logger.LogInformation("Processing PaymentProcessedEvent for Order {OrderId}, Amount: {Amount}, Success: {IsSuccessful}",
                paymentEvent.OrderId, paymentEvent.Amount, paymentEvent.IsSuccessful);

            if (paymentEvent.IsSuccessful)
            {
                // Handle successful payment
                _logger.LogInformation("Payment successful for Order {OrderId}, PaymentId: {PaymentId}", 
                    paymentEvent.OrderId, paymentEvent.PaymentId);
            }
            else
            {
                // Handle failed payment
                _logger.LogWarning("Payment failed for Order {OrderId}", paymentEvent.OrderId);
            }
            
            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// Sample event handler for InventoryReservedEvent
    /// </summary>
    public class InventoryReservedEventHandler : IEventHandler<InventoryReservedEvent>
    {
        private readonly ILogger<InventoryReservedEventHandler> _logger;

        public InventoryReservedEventHandler(ILogger<InventoryReservedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<InventoryReservedEvent> context)
        {
            var inventoryEvent = context.Message;
            
            _logger.LogInformation("Processing InventoryReservedEvent for Product {ProductId}, Quantity: {Quantity}, Order: {OrderId}",
                inventoryEvent.ProductId, inventoryEvent.Quantity, inventoryEvent.OrderId);

            // Process inventory reservation
            // For example: Update order status, notify warehouse, etc.
            
            _logger.LogInformation("Inventory reserved successfully. ReservationId: {ReservationId}", 
                inventoryEvent.ReservationId);
            
            await Task.CompletedTask;
        }
    }
}
