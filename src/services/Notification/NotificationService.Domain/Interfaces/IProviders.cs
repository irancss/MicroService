using NotificationService.Domain.Enums;

namespace NotificationService.Domain.Interfaces;

public interface INotificationProvider
{
    NotificationType SupportedType { get; }
    Task<bool> SendAsync(string recipient, string subject, string body, Dictionary<string, object>? metadata = null);
    bool IsHealthy();
}

public interface IEmailProvider : INotificationProvider
{
    Task<bool> SendEmailAsync(string to, string subject, string body, string? fromEmail = null, string? fromName = null);
}

public interface ISmsProvider : INotificationProvider
{
    Task<bool> SendSmsAsync(string phoneNumber, string message);
}

public interface ITemplateEngine
{
    string ProcessTemplate(string template, Dictionary<string, object> parameters);
}
