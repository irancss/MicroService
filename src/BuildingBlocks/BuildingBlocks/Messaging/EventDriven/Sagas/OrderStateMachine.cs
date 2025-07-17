// File: BuildingBlocks/Messaging/EventDriven/Sagas/OrderStateMachine.cs

using MassTransit;
using BuildingBlocks.Messaging.Events; // Namespace رویدادهای شما

namespace BuildingBlocks.Messaging.EventDriven.Sagas
{
    // این کلاس، داده‌های Saga را که در دیتابیس ذخیره می‌شود، تعریف می‌کند
    public class OrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; } // کلید اصلی در جدول Saga
        public string CurrentState { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? PaymentId { get; set; }
        public string? TrackingNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // این کلاس، خود State Machine (منطق و گردش کار) را تعریف می‌کند
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        // 1. تعریف حالت‌ها (States)
        public State AwaitingInventoryReservation { get; private set; } = null!;
        public State AwaitingPayment { get; private set; } = null!;
        public State AwaitingShipment { get; private set; } = null!;
        public State Completed { get; private set; } = null!;
        public State Cancelled { get; private set; } = null!;

        // 2. تعریف رویدادها (Events)
        public Event<OrderCreatedEvent> OrderCreated { get; private set; } = null!;
        public Event<InventoryReservedEvent> InventoryReserved { get; private set; } = null!;
        public Event<PaymentProcessedEvent> PaymentSucceeded { get; private set; } = null!;
        public Event<PaymentProcessedEvent> PaymentFailed { get; private set; } = null!;
        public Event<ShipmentCreatedEvent> ShipmentCreated { get; private set; } = null!;

        public OrderStateMachine()
        {
            // مشخص کردن فیلدی که حالت فعلی در آن ذخیره می‌شود
            InstanceState(x => x.CurrentState);

            // 3. تعریف نحوه ارتباط رویدادها با نمونه‌های Saga
            // وقتی OrderCreatedEvent می‌آید، یک نمونه جدید Saga با OrderId مربوطه ایجاد کن
            Event(() => OrderCreated, x => x.CorrelateBy<int>(state => state.OrderId, context => context.Message.OrderId).SelectId(context => NewId.NextGuid()));
            
            // برای رویدادهای بعدی، نمونه Saga موجود را بر اساس OrderId پیدا کن
            Event(() => InventoryReserved, x => x.CorrelateById(context => context.Message.CorrelationId)); // فرض می‌کنیم رویدادها CorrelationId دارند
            Event(() => ShipmentCreated, x => x.CorrelateById(context => context.Message.CorrelationId));
            
            // یک رویداد می‌تواند به دو نتیجه مختلف (موفقیت/شکست) منجر شود
            Event(() => PaymentSucceeded, e => e.CorrelateById(context => context.Message.CorrelationId));
            Event(() => PaymentFailed, e => e.CorrelateById(context => context.Message.CorrelationId));
            
            // 4. تعریف گردش کار (Workflow)
            Initially(
                When(OrderCreated)
                    .Then(context =>
                    {
                        context.Saga.OrderId = context.Message.OrderId;
                        context.Saga.CustomerEmail = context.Message.CustomerEmail;
                        context.Saga.Amount = context.Message.Amount;
                        context.Saga.CreatedAt = DateTime.UtcNow;
                    })
                    .Publish(context => new InventoryReservationRequestedEvent { OrderId = context.Saga.OrderId, CorrelationId = context.Saga.CorrelationId })
                    .TransitionTo(AwaitingInventoryReservation)
            );

            During(AwaitingInventoryReservation,
                When(InventoryReserved)
                    .Publish(context => new PaymentRequestedEvent { OrderId = context.Saga.OrderId, Amount = context.Saga.Amount, CorrelationId = context.Saga.CorrelationId })
                    .TransitionTo(AwaitingPayment)
            );
            
            During(AwaitingPayment,
                // اگر پرداخت موفق بود
                When(PaymentSucceeded, context => context.Message.IsSuccessful)
                    .Then(context => context.Saga.PaymentId = context.Message.PaymentId)
                    .Publish(context => new ShipmentRequestedEvent { OrderId = context.Saga.OrderId, CorrelationId = context.Saga.CorrelationId })
                    .TransitionTo(AwaitingShipment),
                
                // اگر پرداخت ناموفق بود (عملیات جبرانی)
                When(PaymentFailed, context => !context.Message.IsSuccessful)
                    .Publish(context => new InventoryReleasedEvent { OrderId = context.Saga.OrderId, Reason = "Payment failed", CorrelationId = context.Saga.CorrelationId })
                    .Publish(context => new OrderCancelledEvent { OrderId = context.Saga.OrderId, CancellationReason = "Payment failed", CorrelationId = context.Saga.CorrelationId })
                    .TransitionTo(Cancelled)
                    .Finalize() // Saga در اینجا تمام می‌شود
            );
            
            During(AwaitingShipment,
                When(ShipmentCreated)
                    .Then(context => context.Saga.TrackingNumber = context.Message.TrackingNumber)
                    .Publish(context => new NotificationRequestedEvent { Recipient = context.Saga.CustomerEmail, Message = $"Order {context.Saga.OrderId} shipped!" })
                    .TransitionTo(Completed)
                    .Finalize() // Saga در اینجا تمام می‌شود
            );

            // اطمینان از اینکه Saga پس از اتمام از دیتابیس حذف می‌شود
            SetCompletedWhenFinalized();
        }
    }
}