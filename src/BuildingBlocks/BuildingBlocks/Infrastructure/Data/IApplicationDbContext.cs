using BuildingBlocks.Messaging.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BuildingBlocks.Infrastructure.Data;

/// <summary>
/// Represents the application's database context.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    DatabaseFacade Database { get; }

    /// <summary>
    /// [نکته] این DbSet برای پیاده‌سازی الگوی Transactional Outbox ضروری است.
    /// تمام رویدادهای یکپارچه‌سازی قبل از ارسال، ابتدا در این جدول ذخیره می‌شوند.
    /// </summary>
    DbSet<OutboxMessage> OutboxMessages { get; }

    /// <summary>
    /// [نکته] این DbSet برای پیاده‌سازی الگوی Event Sourcing یا Auditing استفاده می‌شود.
    /// یک کپی از تمام رویدادهای دامنه در این جدول ذخیره می‌شود.
    /// </summary>
    DbSet<StoredEvent> StoredEvents { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// [اصلاح شد] این کلاس به فایل IApplicationDbContext منتقل شد تا در یک مکان باشند.
/// موجودیت پیام Outbox برای تضمین ارسال قابل اعتماد پیام‌ها.
/// </summary>
public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty; // JSON content
    public DateTime OccurredOnUtc { get; set; }
    public DateTime? ProcessedOnUtc { get; set; }
    public string? Error { get; set; }
}