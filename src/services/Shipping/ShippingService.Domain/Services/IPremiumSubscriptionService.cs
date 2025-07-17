using ShippingService.Domain.Entities;

namespace ShippingService.Domain.Services;

/// <summary>
/// سرویس مدیریت اشتراک‌های ویژه و قوانین ارسال رایگان
/// </summary>
public interface IPremiumSubscriptionService
{
    /// <summary>
    /// بررسی وجود اشتراک فعال برای کاربر
    /// </summary>
    /// <param name="userId">شناسه کاربر</param>
    /// <returns>اشتراک فعال یا null</returns>
    Task<PremiumSubscription?> GetActiveSubscriptionAsync(string userId);
    
    /// <summary>
    /// ایجاد اشتراک جدید
    /// </summary>
    /// <param name="userId">شناسه کاربر</param>
    /// <param name="subscriptionType">نوع اشتراک</param>
    /// <returns>اشتراک جدید</returns>
    Task<PremiumSubscription> CreateSubscriptionAsync(string userId, string subscriptionType);
    
    /// <summary>
    /// تمدید اشتراک
    /// </summary>
    /// <param name="subscriptionId">شناسه اشتراک</param>
    /// <param name="additionalDays">روزهای اضافی</param>
    /// <returns>وضعیت موفقیت</returns>
    Task<bool> ExtendSubscriptionAsync(Guid subscriptionId, int additionalDays);
    
    /// <summary>
    /// استفاده از درخواست رایگان
    /// </summary>
    /// <param name="userId">شناسه کاربر</param>
    /// <param name="shipmentId">شناسه مرسوله</param>
    /// <param name="savedAmount">مبلغ صرفه‌جویی</param>
    /// <returns>وضعیت موفقیت</returns>
    Task<bool> UseFreeRequestAsync(string userId, Guid shipmentId, decimal savedAmount);
    
    /// <summary>
    /// بررسی امکان استفاده از درخواست رایگان
    /// </summary>
    /// <param name="userId">شناسه کاربر</param>
    /// <returns>آیا می‌توان استفاده کرد؟</returns>
    Task<bool> CanUseFreeRequestAsync(string userId);
}
