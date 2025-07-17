using NotificationService.Domain.Enums;

namespace NotificationService.Domain.Entities;

public class NotificationTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
    public Dictionary<string, string> Parameters { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
}
