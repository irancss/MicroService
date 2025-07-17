using MassTransit;
using OrderService.Core.Enums;
using OrderService.Core.Models;

namespace OrderService.Application.Sagas
{
    public class OrderSagaState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = string.Empty;
        
        // Order Information
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public List<OrderItem> Items { get; set; } = new();
        
        // Process tracking
        public bool InventoryReserved { get; set; }
        public bool PaymentProcessed { get; set; }
        public string? PaymentTransactionId { get; set; }
        public string? FailureReason { get; set; }
        
        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? FailedAt { get; set; }
        
        // Retry mechanism
        public int RetryCount { get; set; }
        public DateTime? NextRetryAt { get; set; }
    }
}
