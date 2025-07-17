using Shared.Kernel.Domain;

namespace ShippingService.Domain.Entities;

/// <summary>
/// لاگ استفاده از اشتراک ویژه برای رهگیری و مدیریت مصرف
/// </summary>
public class SubscriptionUsageLog : BaseEntity
{
    /// <summary>
    /// شناسه اشتراک
    /// </summary>
    public Guid SubscriptionId { get; private set; }
    
    /// <summary>
    /// شناسه مرسوله
    /// </summary>
    public Guid ShipmentId { get; private set; }
    
    /// <summary>
    /// مبلغ صرفه‌جویی شده
    /// </summary>
    public decimal SavedAmount { get; private set; }
    
    /// <summary>
    /// تاریخ استفاده
    /// </summary>
    public DateTime UsageDate { get; private set; }
    
    /// <summary>
    /// توضیحات اضافی
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// اشتراک مرتبط
    /// </summary>
    public PremiumSubscription Subscription { get; private set; } = null!;

    protected SubscriptionUsageLog() { }

    /// <summary>
    /// سازنده لاگ استفاده از اشتراک
    /// </summary>
    /// <param name="subscriptionId">شناسه اشتراک</param>
    /// <param name="shipmentId">شناسه مرسوله</param>
    /// <param name="savedAmount">مبلغ صرفه‌جویی</param>
    /// <param name="notes">توضیحات</param>
    public SubscriptionUsageLog(
        Guid subscriptionId,
        Guid shipmentId,
        decimal savedAmount,
        string? notes = null)
    {
        Id = Guid.NewGuid();
        SubscriptionId = subscriptionId;
        ShipmentId = shipmentId;
        SavedAmount = savedAmount;
        UsageDate = DateTime.UtcNow;
        Notes = notes;
        CreatedAt = DateTime.UtcNow;
    }
}
