using BuildingBlocks.Messaging.EventDriven;
using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace BuildingBlocks.Messaging.EventDriven.Sagas
{
    /// <summary>
    /// Order processing saga that coordinates the entire order workflow
    /// </summary>
    public class OrderProcessingSaga : SagaOrchestrator<OrderProcessingData>,
        IConsumer<OrderCreatedEvent>,
        IConsumer<PaymentProcessedEvent>,
        IConsumer<InventoryReservedEvent>,
        IConsumer<ShipmentCreatedEvent>,
        IConsumer<OrderCancelledEvent>
    {
        public OrderProcessingSaga(IEventBus eventBus) : base(eventBus)
        {
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var orderEvent = context.Message;
            
            Data.CorrelationId = Guid.NewGuid();
            Data.OrderId = orderEvent.OrderId;
            Data.CustomerEmail = orderEvent.CustomerEmail;
            Data.Amount = orderEvent.Amount;
            Data.CurrentState = "OrderCreated";

            // Start the order processing workflow
            await PublishEventAsync(new InventoryReservationRequestedEvent
            {
                OrderId = orderEvent.OrderId,
                Amount = orderEvent.Amount,
                Items = new List<InventoryItem>() // Populate from order
            });

            Data.CurrentState = "InventoryReservationRequested";
        }

        public async Task Consume(ConsumeContext<InventoryReservedEvent> context)
        {
            var inventoryEvent = context.Message;
            
            if (inventoryEvent.OrderId == Data.OrderId)
            {
                Data.CurrentState = "InventoryReserved";

                // Request payment processing
                await PublishEventAsync(new PaymentRequestedEvent
                {
                    OrderId = Data.OrderId,
                    Amount = Data.Amount,
                    CustomerEmail = Data.CustomerEmail
                });

                Data.CurrentState = "PaymentRequested";
            }
        }

        public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
        {
            var paymentEvent = context.Message;
            
            if (paymentEvent.OrderId == Data.OrderId)
            {
                if (paymentEvent.IsSuccessful)
                {
                    Data.CurrentState = "PaymentCompleted";
                    Data.PaymentId = paymentEvent.PaymentId;

                    // Update order status
                    await PublishEventAsync(new OrderStatusChangedEvent
                    {
                        OrderId = Data.OrderId,
                        PreviousStatus = "PaymentPending",
                        CurrentStatus = "Confirmed",
                        CustomerEmail = Data.CustomerEmail
                    });

                    // Request shipment creation
                    await PublishEventAsync(new ShipmentRequestedEvent
                    {
                        OrderId = Data.OrderId,
                        CustomerEmail = Data.CustomerEmail
                    });

                    Data.CurrentState = "ShipmentRequested";
                }
                else
                {
                    // Payment failed - cancel order
                    await HandlePaymentFailure();
                }
            }
        }

        public async Task Consume(ConsumeContext<ShipmentCreatedEvent> context)
        {
            var shipmentEvent = context.Message;
            
            if (shipmentEvent.OrderId == Data.OrderId)
            {
                Data.CurrentState = "ShipmentCreated";
                Data.TrackingNumber = shipmentEvent.TrackingNumber;

                // Send notification to customer
                await PublishEventAsync(new NotificationRequestedEvent
                {
                    NotificationType = "Email",
                    Recipient = Data.CustomerEmail,
                    Subject = "Your Order has been Shipped",
                    Message = $"Your order {Data.OrderId} has been shipped. Tracking number: {Data.TrackingNumber}",
                    Priority = "Normal"
                });

                Data.CurrentState = "Completed";
            }
        }

        public async Task Consume(ConsumeContext<OrderCancelledEvent> context)
        {
            var cancelEvent = context.Message;
            
            if (cancelEvent.OrderId == Data.OrderId)
            {
                Data.CurrentState = "Cancelled";
                Data.CancellationReason = cancelEvent.CancellationReason;

                // Send cancellation notification
                await PublishEventAsync(new NotificationRequestedEvent
                {
                    NotificationType = "Email",
                    Recipient = Data.CustomerEmail,
                    Subject = "Order Cancellation Confirmation",
                    Message = $"Your order {Data.OrderId} has been cancelled. Refund of ${cancelEvent.RefundAmount} will be processed.",
                    Priority = "High"
                });
            }
        }

        private async Task HandlePaymentFailure()
        {
            Data.CurrentState = "PaymentFailed";

            // Release inventory
            await PublishEventAsync(new InventoryReleasedEvent
            {
                OrderId = Data.OrderId,
                Reason = "Payment failed"
            });

            // Cancel order
            await PublishEventAsync(new OrderCancelledEvent
            {
                OrderId = Data.OrderId,
                CustomerEmail = Data.CustomerEmail,
                RefundAmount = 0,
                CancellationReason = "Payment processing failed"
            });
        }

        public override Task HandleAsync<T>(ConsumeContext<T> context)
        {
            // Generic handler for other events
            return Task.CompletedTask;
        }
    }

    public class OrderProcessingData : ISagaData
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? PaymentId { get; set; }
        public string? TrackingNumber { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
    }

    // Additional events for saga coordination
    public class InventoryReservationRequestedEvent : BaseEvent
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public List<InventoryItem> Items { get; set; } = new();
    }

    public class InventoryItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class PaymentRequestedEvent : BaseEvent
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
    }

    public class ShipmentRequestedEvent : BaseEvent
    {
        public int OrderId { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
    }

    public class InventoryReleasedEvent : BaseEvent
    {
        public int OrderId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}

namespace BuildingBlocks.Messaging.EventDriven.Sagas
{
    /// <summary>
    /// Customer onboarding saga
    /// </summary>
    public class CustomerOnboardingSaga : SagaOrchestrator<CustomerOnboardingData>,
        IConsumer<CustomerRegisteredEvent>,
        IConsumer<NotificationSentEvent>,
        IConsumer<UserLoginEvent>
    {
        public CustomerOnboardingSaga(IEventBus eventBus) : base(eventBus)
        {
        }

        public async Task Consume(ConsumeContext<CustomerRegisteredEvent> context)
        {
            var customerEvent = context.Message;
            
            Data.CorrelationId = Guid.NewGuid();
            Data.CustomerId = customerEvent.CustomerId;
            Data.Email = customerEvent.Email;
            Data.FullName = customerEvent.FullName;
            Data.CurrentState = "Registered";

            // Send welcome email
            await PublishEventAsync(new NotificationRequestedEvent
            {
                NotificationType = "Email",
                Recipient = customerEvent.Email,
                Subject = "Welcome to our platform!",
                TemplateId = "welcome-email",
                TemplateData = new Dictionary<string, object>
                {
                    ["customerName"] = customerEvent.FullName,
                    ["registrationDate"] = customerEvent.RegistrationDate
                },
                Priority = "Normal"
            });

            Data.CurrentState = "WelcomeEmailSent";

            // Schedule follow-up email for 3 days later
            await ScheduleEventAsync(new FollowUpEmailEvent
            {
                CustomerId = customerEvent.CustomerId,
                Email = customerEvent.Email
            }, TimeSpan.FromDays(3));
        }

        public async Task Consume(ConsumeContext<NotificationSentEvent> context)
        {
            var notificationEvent = context.Message;
            
            if (notificationEvent.Recipient == Data.Email)
            {
                Data.WelcomeEmailSent = notificationEvent.IsSuccessful;
                if (notificationEvent.IsSuccessful)
                {
                    Data.CurrentState = "WelcomeEmailDelivered";
                }
            }
        }

        public async Task Consume(ConsumeContext<UserLoginEvent> context)
        {
            var loginEvent = context.Message;
            
            if (loginEvent.Email == Data.Email && loginEvent.IsSuccessful)
            {
                Data.FirstLoginAt = loginEvent.LoginTime;
                Data.CurrentState = "FirstLoginCompleted";

                // Send onboarding tips
                await PublishEventAsync(new NotificationRequestedEvent
                {
                    NotificationType = "Email",
                    Recipient = Data.Email,
                    Subject = "Getting Started Guide",
                    TemplateId = "onboarding-tips",
                    Priority = "Normal"
                });

                Data.CurrentState = "OnboardingCompleted";
            }
        }

        public override Task HandleAsync<T>(ConsumeContext<T> context)
        {
            return Task.CompletedTask;
        }
    }

    public class CustomerOnboardingData : ISagaData
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool WelcomeEmailSent { get; set; }
        public DateTime? FirstLoginAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class FollowUpEmailEvent : BaseEvent
    {
        public int CustomerId { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
