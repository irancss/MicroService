namespace OrderService.Application.Events
{
    public class InventoryReserved
    {
        public Guid OrderId { get; set; }
        public Guid CorrelationId { get; set; }
        public bool Success { get; set; }
        public string? FailureReason { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class InventoryReservationFailed
    {
        public Guid OrderId { get; set; }
        public Guid CorrelationId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class PaymentProcessed
    {
        public Guid OrderId { get; set; }
        public Guid CorrelationId { get; set; }
        public bool Success { get; set; }
        public string? FailureReason { get; set; }
        public string? TransactionId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class PaymentFailed
    {
        public Guid OrderId { get; set; }
        public Guid CorrelationId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class OrderCompleted
    {
        public Guid OrderId { get; set; }
        public Guid CorrelationId { get; set; }
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    }

    public class OrderFailed
    {
        public Guid OrderId { get; set; }
        public Guid CorrelationId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime FailedAt { get; set; } = DateTime.UtcNow;
    }
}
