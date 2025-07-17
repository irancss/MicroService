namespace NotificationService.Domain.Enums;

public enum NotificationType
{
    Email = 1,
    Sms = 2,
    Push = 3
}

public enum NotificationStatus
{
    Pending = 1,
    Sent = 2,
    Failed = 3,
    Delivered = 4
}

public enum NotificationPriority
{
    Low = 1,
    Normal = 2,
    High = 3,
    Critical = 4
}
