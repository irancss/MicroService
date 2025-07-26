using MassTransit;
using BuildingBlocks.Messaging.Events.Contracts;
using BuildingBlocks.Messaging.Events.Domains;

namespace BuildingBlocks.Messaging.EventDriven.Sagas
{
    public class OrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? PaymentId { get; set; }
        public string? TrackingNumber { get; set; }
        public string? FailureReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public State AwaitingPayment { get; private set; } = null!;
        public State AwaitingShipment { get; private set; } = null!;
        public State Cancelled { get; private set; } = null!;
        public State Completed { get; private set; } = null!;

        public Event<OrderCreatedEvent> OrderCreated { get; private set; } = null!;
        public Event<PaymentSucceededEvent> PaymentSucceeded { get; private set; } = null!;
        public Event<PaymentFailedEvent> PaymentFailed { get; private set; } = null!;
        public Event<ShipmentCreatedEvent> ShipmentCreated { get; private set; } = null!;

        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => OrderCreated, x => x.CorrelateBy<int>(s => s.OrderId, ctx => ctx.Message.OrderId).SelectId(ctx => NewId.NextGuid()));
            Event(() => PaymentSucceeded, x => x.CorrelateBy<int>(s => s.OrderId, ctx => ctx.Message.OrderId));
            Event(() => PaymentFailed, x => x.CorrelateBy<int>(s => s.OrderId, ctx => ctx.Message.OrderId));
            Event(() => ShipmentCreated, x => x.CorrelateBy<int>(s => s.OrderId, ctx => ctx.Message.OrderId));

            Initially(
                When(OrderCreated)
                    .Then(ctx => {
                        ctx.Saga.OrderId = ctx.Message.OrderId;
                        ctx.Saga.CustomerEmail = ctx.Message.CustomerEmail;
                        ctx.Saga.Amount = ctx.Message.Amount;
                        ctx.Saga.CreatedAt = DateTime.UtcNow;
                    })
                    .Publish(ctx => new PaymentRequestEvent(ctx.Saga.OrderId, ctx.Saga.Amount))
                    .TransitionTo(AwaitingPayment)
            );

            During(AwaitingPayment,
                When(PaymentSucceeded)
                    .Then(ctx => ctx.Saga.PaymentId = ctx.Message.PaymentId)
                    .Publish(ctx => new ShipmentRequestEvent(ctx.Saga.OrderId, "Default Customer Address"))
                    .TransitionTo(AwaitingShipment),
                When(PaymentFailed)
                    .Then(ctx => ctx.Saga.FailureReason = ctx.Message.Reason)
                    .Publish(ctx => new OrderCancellationEvent(ctx.Saga.OrderId, $"Payment failed: {ctx.Message.Reason}"))
                    .TransitionTo(Cancelled)
                    .Finalize()
            );

            During(AwaitingShipment,
                When(ShipmentCreated)
                    .Then(ctx => ctx.Saga.TrackingNumber = ctx.Message.TrackingNumber)
                    .Publish(ctx => new NotificationRequestEvent(ctx.Saga.CustomerEmail, $"Order {ctx.Saga.OrderId} shipped!", $"Tracking: {ctx.Message.TrackingNumber}"))
                    .TransitionTo(Completed)
                    .Finalize()
            );

            SetCompletedWhenFinalized();
        }
    }
}