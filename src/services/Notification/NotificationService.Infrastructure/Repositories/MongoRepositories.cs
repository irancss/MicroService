using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Infrastructure.Repositories;

public class MongoNotificationTemplateRepository : INotificationTemplateRepository
{
    private readonly IMongoCollection<NotificationTemplate> _collection;

    public MongoNotificationTemplateRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<NotificationTemplate>("notification_templates");
    }

    public async Task<NotificationTemplate?> GetByIdAsync(Guid id)
    {
        return await _collection
            .Find(t => t.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<NotificationTemplate?> GetByNameAsync(string name)
    {
        return await _collection
            .Find(t => t.Name == name && t.IsActive)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<NotificationTemplate>> GetAllAsync()
    {
        return await _collection
            .Find(_ => true)
            .ToListAsync();
    }

    public async Task<NotificationTemplate> CreateAsync(NotificationTemplate template)
    {
        await _collection.InsertOneAsync(template);
        return template;
    }

    public async Task<NotificationTemplate> UpdateAsync(NotificationTemplate template)
    {
        await _collection.ReplaceOneAsync(t => t.Id == template.Id, template);
        return template;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _collection.DeleteOneAsync(t => t.Id == id);
    }
}

public class MongoNotificationLogRepository : INotificationLogRepository
{
    private readonly IMongoCollection<NotificationLog> _collection;

    public MongoNotificationLogRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<NotificationLog>("notification_logs");
    }

    public async Task<NotificationLog> CreateAsync(NotificationLog log)
    {
        await _collection.InsertOneAsync(log);
        return log;
    }

    public async Task<NotificationLog> UpdateAsync(NotificationLog log)
    {
        await _collection.ReplaceOneAsync(l => l.Id == log.Id, log);
        return log;
    }

    public async Task<NotificationLog?> GetByIdAsync(Guid id)
    {
        return await _collection
            .Find(l => l.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<NotificationLog>> GetByUserIdAsync(string userId, int page = 1, int pageSize = 10)
    {
        return await _collection
            .Find(l => l.UserId == userId)
            .SortByDescending(l => l.SentAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<NotificationLog>> GetFailedNotificationsAsync()
    {
        return await _collection
            .Find(l => l.Status == Domain.Enums.NotificationStatus.Failed)
            .SortByDescending(l => l.SentAt)
            .ToListAsync();
    }
}
