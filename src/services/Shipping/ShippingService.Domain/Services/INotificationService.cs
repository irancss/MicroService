using ShippingService.Domain.Enums;

namespace ShippingService.Domain.Services;

public interface INotificationService
{
    Task SendShipmentStatusUpdateAsync(string customerId, string trackingNumber, ShipmentStatus status, string? message = null);
    Task SendReturnStatusUpdateAsync(string customerId, string returnTrackingNumber, ReturnStatus status, string? message = null);
    Task SendDeliveryNotificationAsync(string customerId, string trackingNumber, DateTime estimatedDeliveryTime);
    Task SendSmsAsync(string phoneNumber, string message);
    Task SendEmailAsync(string email, string subject, string message);
    Task SendPushNotificationAsync(string userId, string title, string message, object? data = null);
}

public class NotificationRequest
{
    public string UserId { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public object? Data { get; set; }
}

public enum NotificationType
{
    SMS = 1,
    Email = 2,
    Push = 3,
    All = 99
}
