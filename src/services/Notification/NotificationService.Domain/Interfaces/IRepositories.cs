using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Interfaces;

public interface INotificationTemplateRepository
{
    Task<NotificationTemplate?> GetByIdAsync(Guid id);
    Task<NotificationTemplate?> GetByNameAsync(string name);
    Task<IEnumerable<NotificationTemplate>> GetAllAsync();
    Task<NotificationTemplate> CreateAsync(NotificationTemplate template);
    Task<NotificationTemplate> UpdateAsync(NotificationTemplate template);
    Task DeleteAsync(Guid id);
}

public interface INotificationLogRepository
{
    Task<NotificationLog> CreateAsync(NotificationLog log);
    Task<NotificationLog> UpdateAsync(NotificationLog log);
    Task<NotificationLog?> GetByIdAsync(Guid id);
    Task<IEnumerable<NotificationLog>> GetByUserIdAsync(string userId, int page = 1, int pageSize = 10);
    Task<IEnumerable<NotificationLog>> GetFailedNotificationsAsync();
}
