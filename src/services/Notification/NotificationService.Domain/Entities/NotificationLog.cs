using NotificationService.Domain.Enums;

namespace NotificationService.Domain.Entities;

public class NotificationLog
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public string Recipient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime SentAt { get; set; }
    public string? Provider { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    public int RetryCount { get; set; } = 0;
    public Guid? TemplateId { get; set; }
}
