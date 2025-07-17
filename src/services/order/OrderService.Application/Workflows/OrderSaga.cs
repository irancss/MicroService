using MassTransit;
using Microsoft.Extensions.Logging;
using OrderService.Application.Events;
using OrderService.Application.Sagas;
using OrderService.Application.Commands;
using OrderService.Application.Activities;
using OrderService.Core.Enums;

namespace OrderService.Application.Workflows
{
    public class OrderSaga : MassTransitStateMachine<OrderSagaState>
    {
        private readonly ILogger<OrderSaga> _logger;

        public OrderSaga(ILogger<OrderSaga> logger)
        {
            _logger = logger;
            
            InstanceState(x => x.CurrentState);
            
            // Define states
            State(() => InventoryReservation);
            State(() => PaymentProcessing);
            State(() => OrderCompleted);
            State(() => OrderFailed);
            State(() => Compensating);
            
            // Define events
            Event(() => OrderCreationStarted, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => InventoryReserved, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => InventoryReservationFailed, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => PaymentProcessed, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => PaymentFailed, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderCancellationRequested, x => x.CorrelateById(m => m.Message.OrderId));
            
            // Define the saga workflow
            Initially(
                When(OrderCreationStarted)
                    .Then(context =>
                    {
                        context.Saga.OrderId = context.Message.OrderId;
                        context.Saga.CustomerId = context.Message.CustomerId;
                        context.Saga.TotalAmount = context.Message.TotalAmount;
                        context.Saga.ShippingAddress = context.Message.ShippingAddress;
                        context.Saga.BillingAddress = context.Message.BillingAddress;
                        context.Saga.Items = context.Message.Items;
                        context.Saga.CreatedAt = DateTime.UtcNow;
                        context.Saga.RetryCount = 0;
                        
                        _logger.LogInformation("Order saga started for order {OrderId}", context.Message.OrderId);
                    })
                    .Publish(context => new InventoryReservationRequested
                    {
                        OrderId = context.Saga.OrderId,
                        CorrelationId = context.Saga.CorrelationId,
                        Items = context.Saga.Items
                    })
                    .TransitionTo(InventoryReservation)
            );
            
            During(InventoryReservation,
                When(InventoryReserved)
                    .Then(context =>
                    {
                        context.Saga.InventoryReserved = true;
                        _logger.LogInformation("Inventory reserved for order {OrderId}", context.Message.OrderId);
                    })
                    .Publish(context => new PaymentProcessingRequested
                    {
                        OrderId = context.Saga.OrderId,
                        CorrelationId = context.Saga.CorrelationId,
                        CustomerId = context.Saga.CustomerId,
                        Amount = context.Saga.TotalAmount
                    })
                    .TransitionTo(PaymentProcessing),
                    
                When(InventoryReservationFailed)
                    .Then(context =>
                    {
                        context.Saga.FailureReason = context.Message.Reason;
                        context.Saga.FailedAt = DateTime.UtcNow;
                        _logger.LogWarning("Inventory reservation failed for order {OrderId}: {Reason}", 
                            context.Message.OrderId, context.Message.Reason);
                    })
                    .Publish(context => new OrderFailed
                    {
                        OrderId = context.Saga.OrderId,
                        CorrelationId = context.Saga.CorrelationId,
                        Reason = context.Saga.FailureReason ?? "Inventory reservation failed"
                    })
                    .TransitionTo(OrderFailed)
            );
            
            During(PaymentProcessing,
                When(PaymentProcessed)
                    .If(context => context.Message.Success, x => x
                        .Then(context =>
                        {
                            context.Saga.PaymentProcessed = true;
                            context.Saga.PaymentTransactionId = context.Message.TransactionId;
                            context.Saga.CompletedAt = DateTime.UtcNow;
                            _logger.LogInformation("Payment processed successfully for order {OrderId}", context.Message.OrderId);
                        })
                        .Send(context => new Uri("queue:order-status-update"), context => new UpdateOrderStatusCommand
                        {
                            OrderId = context.Saga.OrderId,
                            Status = OrderStatus.Confirmed,
                            UpdatedBy = "OrderSaga"
                        })
                        .Publish(context => new OrderCompleted
                        {
                            OrderId = context.Saga.OrderId,
                            CorrelationId = context.Saga.CorrelationId
                        })
                        .TransitionTo(OrderCompleted)),
                        
                When(PaymentProcessed)
                    .If(context => !context.Message.Success, x => x
                        .Then(context =>
                        {
                            context.Saga.FailureReason = context.Message.FailureReason ?? "Payment processing failed";
                            context.Saga.FailedAt = DateTime.UtcNow;
                            _logger.LogWarning("Payment failed for order {OrderId}: {Reason}", 
                                context.Message.OrderId, context.Saga.FailureReason);
                        })
                        .Publish(context => new InventoryReleaseRequested
                        {
                            OrderId = context.Saga.OrderId,
                            CorrelationId = context.Saga.CorrelationId,
                            Items = context.Saga.Items
                        })
                        .TransitionTo(Compensating)),
                        
                When(PaymentFailed)
                    .Then(context =>
                    {
                        context.Saga.FailureReason = context.Message.Reason;
                        context.Saga.FailedAt = DateTime.UtcNow;
                        _logger.LogWarning("Payment failed for order {OrderId}: {Reason}", 
                            context.Message.OrderId, context.Message.Reason);
                    })
                    .Publish(context => new InventoryReleaseRequested
                    {
                        OrderId = context.Saga.OrderId,
                        CorrelationId = context.Saga.CorrelationId,
                        Items = context.Saga.Items
                    })
                    .TransitionTo(Compensating)
            );
            
            During(Compensating,
                When(CompensationCompleted)
                    .Then(context =>
                    {
                        _logger.LogInformation("Compensation completed for order {OrderId}", context.Saga.OrderId);
                    })
                    .Send(context => new Uri("queue:order-status-update"), context => new UpdateOrderStatusCommand
                    {
                        OrderId = context.Saga.OrderId,
                        Status = OrderStatus.Cancelled,
                        UpdatedBy = "OrderSaga"
                    })
                    .Publish(context => new OrderFailed
                    {
                        OrderId = context.Saga.OrderId,
                        CorrelationId = context.Saga.CorrelationId,
                        Reason = context.Saga.FailureReason ?? "Order processing failed"
                    })
                    .TransitionTo(OrderFailed)
            );
            
            // Handle cancellation requests
            DuringAny(
                When(OrderCancellationRequested)
                    .If(context => context.Saga.CurrentState == "InventoryReservation" || 
                                   context.Saga.CurrentState == "PaymentProcessing", x => x
                        .Then(context =>
                        {
                            context.Saga.FailureReason = "Order cancelled by user";
                            context.Saga.FailedAt = DateTime.UtcNow;
                            _logger.LogInformation("Order cancellation requested for order {OrderId}", context.Message.OrderId);
                        })
                        .Publish(context => new InventoryReleaseRequested
                        {
                            OrderId = context.Saga.OrderId,
                            CorrelationId = context.Saga.CorrelationId,
                            Items = context.Saga.Items
                        })
                        .TransitionTo(Compensating)),
                        
                When(OrderCancellationRequested)
                    .If(context => !(context.Saga.CurrentState == "InventoryReservation" || 
                                     context.Saga.CurrentState == "PaymentProcessing"), x => x
                        .Then(context =>
                        {
                            _logger.LogWarning("Cannot cancel order {OrderId} in current state {State}", 
                                context.Message.OrderId, context.Saga.CurrentState);
                        }))
            );
            
            SetCompletedWhenFinalized();
        }
        
        public State? InventoryReservation { get; private set; }
        public State? PaymentProcessing { get; private set; }
        public State? OrderCompleted { get; private set; }
        public State? OrderFailed { get; private set; }
        public State? Compensating { get; private set; }
        
        public Event<OrderCreationStarted>? OrderCreationStarted { get; private set; }
        public Event<InventoryReserved>? InventoryReserved { get; private set; }
        public Event<InventoryReservationFailed>? InventoryReservationFailed { get; private set; }
        public Event<PaymentProcessed>? PaymentProcessed { get; private set; }
        public Event<PaymentFailed>? PaymentFailed { get; private set; }
        public Event<OrderCancellationRequested>? OrderCancellationRequested { get; private set; }
        public Event<CompensationCompleted>? CompensationCompleted { get; private set; }
    }
    
    public class OrderCancellationRequested
    {
        public Guid OrderId { get; set; }
        public Guid CorrelationId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }
    
    public class CompensationCompleted
    {
        public Guid OrderId { get; set; }
        public Guid CorrelationId { get; set; }
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    }
    
    // Additional event classes needed by the saga
    public class InventoryReservationRequested
    {
        public Guid OrderId { get; set; }
        public Guid CorrelationId { get; set; }
        public List<Core.Models.OrderItem> Items { get; set; } = new();
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }

    public class PaymentProcessingRequested
    {
        public Guid OrderId { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }

    public class InventoryReleaseRequested
    {
        public Guid OrderId { get; set; }
        public Guid CorrelationId { get; set; }
        public List<Core.Models.OrderItem> Items { get; set; } = new();
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }
}
