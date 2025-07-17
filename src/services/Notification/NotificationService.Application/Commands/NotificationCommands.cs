using MediatR;
using NotificationService.Domain.Enums;

namespace NotificationService.Application.Commands;

public record SendEmailCommand : IRequest<bool>
{
    public string To { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public string? FromEmail { get; init; }
    public string? FromName { get; init; }
    public string UserId { get; init; } = string.Empty;
    public Dictionary<string, object> Metadata { get; init; } = new();
    public NotificationPriority Priority { get; init; } = NotificationPriority.Normal;
    public Guid? TemplateId { get; init; }
}

public record SendSmsCommand : IRequest<bool>
{
    public string PhoneNumber { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public Dictionary<string, object> Metadata { get; init; } = new();
    public NotificationPriority Priority { get; init; } = NotificationPriority.Normal;
    public Guid? TemplateId { get; init; }
}

public record QueueNotificationFromEventCommand : IRequest<bool>
{
    public string EventType { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public Dictionary<string, object> Payload { get; init; } = new();
    public NotificationPriority Priority { get; init; } = NotificationPriority.Normal;
}

public record CreateTemplateCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public NotificationType Type { get; init; }
    public string Subject { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public string Language { get; init; } = "en";
    public Dictionary<string, string> Parameters { get; init; } = new();
    public string CreatedBy { get; init; } = string.Empty;
}

public record UpdateTemplateCommand : IRequest<bool>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public NotificationType Type { get; init; }
    public string Subject { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public string Language { get; init; } = "en";
    public Dictionary<string, string> Parameters { get; init; } = new();
    public bool IsActive { get; init; } = true;
    public string UpdatedBy { get; init; } = string.Empty;
}
