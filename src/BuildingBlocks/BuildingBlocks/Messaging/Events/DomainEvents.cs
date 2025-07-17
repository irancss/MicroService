using MassTransit;

namespace BuildingBlocks.Messaging.Events
{
    // E-commerce Domain Events
    public class ProductCreatedEvent : BaseEvent
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }

    public class ProductUpdatedEvent : BaseEvent
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public int StockQuantity { get; set; }
        public Dictionary<string, object> Changes { get; set; } = new();
    }

    public class StockUpdatedEvent : BaseEvent
    {
        public int ProductId { get; set; }
        public int PreviousStock { get; set; }
        public int CurrentStock { get; set; }
        public string Reason { get; set; } = string.Empty; // "Purchase", "Sale", "Adjustment"
    }

    public class OrderStatusChangedEvent : BaseEvent
    {
        public int OrderId { get; set; }
        public string PreviousStatus { get; set; } = string.Empty;
        public string CurrentStatus { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }

    public class OrderCancelledEvent : BaseEvent
    {
        public int OrderId { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal RefundAmount { get; set; }
        public string CancellationReason { get; set; } = string.Empty;
        public List<OrderCancelledItem> Items { get; set; } = new();
    }

    public class OrderCancelledItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class CustomerRegisteredEvent : BaseEvent
    {
        public int CustomerId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
    }

    public class ShipmentCreatedEvent : BaseEvent
    {
        public int ShipmentId { get; set; }
        public int OrderId { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public string CarrierName { get; set; } = string.Empty;
        public DateTime EstimatedDeliveryDate { get; set; }
        public ShipmentAddress DeliveryAddress { get; set; } = new();
    }

    public class ShipmentStatusChangedEvent : BaseEvent
    {
        public int ShipmentId { get; set; }
        public int OrderId { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public string PreviousStatus { get; set; } = string.Empty;
        public string CurrentStatus { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime StatusDate { get; set; }
    }

    public class ShipmentAddress
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    // Notification Events
    public class NotificationRequestedEvent : BaseEvent
    {
        public string NotificationType { get; set; } = string.Empty; // "Email", "SMS", "Push"
        public string Recipient { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string TemplateId { get; set; } = string.Empty;
        public Dictionary<string, object> TemplateData { get; set; } = new();
        public string Priority { get; set; } = "Normal"; // "Low", "Normal", "High", "Urgent"
    }

    public class NotificationSentEvent : BaseEvent
    {
        public Guid NotificationId { get; set; }
        public string NotificationType { get; set; } = string.Empty;
        public string Recipient { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }

    // User Activity Events
    public class UserLoginEvent : BaseEvent
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public DateTime LoginTime { get; set; }
        public bool IsSuccessful { get; set; }
    }

    public class UserLogoutEvent : BaseEvent
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime LogoutTime { get; set; }
        public TimeSpan SessionDuration { get; set; }
    }

    // Analytics Events
    public class ProductViewedEvent : BaseEvent
    {
        public int ProductId { get; set; }
        public int? UserId { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public string ReferrerUrl { get; set; } = string.Empty;
        public DateTime ViewedAt { get; set; }
    }

    public class CartUpdatedEvent : BaseEvent
    {
        public int UserId { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public List<CartItem> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public string Action { get; set; } = string.Empty; // "Add", "Remove", "Update", "Clear"
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    // System Events
    public class SystemHealthCheckEvent : BaseEvent
    {
        public string ServiceName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // "Healthy", "Degraded", "Unhealthy"
        public Dictionary<string, object> HealthData { get; set; } = new();
        public TimeSpan ResponseTime { get; set; }
    }

    public class ServiceStartedEvent : BaseEvent
    {
        public string ServiceName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public string MachineName { get; set; } = string.Empty;
    }

    public class ServiceStoppedEvent : BaseEvent
    {
        public string ServiceName { get; set; } = string.Empty;
        public DateTime StopTime { get; set; }
        public TimeSpan Uptime { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
