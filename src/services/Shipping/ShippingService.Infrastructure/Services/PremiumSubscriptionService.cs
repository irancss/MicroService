using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Services;
using ShippingService.Infrastructure.Data;

namespace ShippingService.Infrastructure.Services;

/// <summary>
/// پیاده‌سازی سرویس مدیریت اشتراک‌های ویژه
/// </summary>
public class PremiumSubscriptionService : IPremiumSubscriptionService
{
    private readonly ShippingDbContext _context;
    private readonly ILogger<PremiumSubscriptionService> _logger;

    /// <summary>
    /// سازنده سرویس اشتراک‌های ویژه
    /// </summary>
    /// <param name="context">کانتکست دیتابیس</param>
    /// <param name="logger">لاگر</param>
    public PremiumSubscriptionService(
        ShippingDbContext context,
        ILogger<PremiumSubscriptionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// بررسی وجود اشتراک فعال برای کاربر
    /// </summary>
    /// <param name="userId">شناسه کاربر</param>
    /// <returns>اشتراک فعال یا null</returns>
    public async Task<PremiumSubscription?> GetActiveSubscriptionAsync(string userId)
    {
        try
        {
            var subscription = await _context.PremiumSubscriptions
                .Include(s => s.UsageLogs)
                .FirstOrDefaultAsync(s => 
                    s.UserId == userId && 
                    s.IsActive && 
                    s.EndDate > DateTime.UtcNow);

            if (subscription != null)
            {
                // بررسی انقضای اشتراک
                subscription.CheckExpiration();
                await _context.SaveChangesAsync();
            }

            return subscription;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت اشتراک فعال کاربر {UserId}", userId);
            return null;
        }
    }

    /// <summary>
    /// ایجاد اشتراک جدید
    /// </summary>
    /// <param name="userId">شناسه کاربر</param>
    /// <param name="subscriptionType">نوع اشتراک</param>
    /// <returns>اشتراک جدید</returns>
    public async Task<PremiumSubscription> CreateSubscriptionAsync(string userId, string subscriptionType)
    {
        try
        {
            // تنظیمات پیش‌فرض بر اساس نوع اشتراک
            var (name, maxFreeRequests, price, duration) = GetSubscriptionSettings(subscriptionType);

            // لغو اشتراک‌های قبلی فعال
            var existingSubscriptions = await _context.PremiumSubscriptions
                .Where(s => s.UserId == userId && s.IsActive)
                .ToListAsync();

            foreach (var existing in existingSubscriptions)
            {
                existing.Cancel();
            }

            // ایجاد اشتراک جدید
            var subscription = new PremiumSubscription(
                userId: userId,
                name: name,
                maxFreeRequestsPerMonth: maxFreeRequests,
                price: price,
                durationInDays: duration,
                description: $"اشتراک {subscriptionType} با {maxFreeRequests} درخواست رایگان در ماه"
            );

            _context.PremiumSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            _logger.LogInformation("اشتراک جدید {SubscriptionType} برای کاربر {UserId} ایجاد شد", subscriptionType, userId);

            return subscription;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ایجاد اشتراک برای کاربر {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// تمدید اشتراک
    /// </summary>
    /// <param name="subscriptionId">شناسه اشتراک</param>
    /// <param name="additionalDays">روزهای اضافی</param>
    /// <returns>وضعیت موفقیت</returns>
    public async Task<bool> ExtendSubscriptionAsync(Guid subscriptionId, int additionalDays)
    {
        try
        {
            var subscription = await _context.PremiumSubscriptions
                .FindAsync(subscriptionId);

            if (subscription == null)
            {
                _logger.LogWarning("اشتراک با شناسه {SubscriptionId} یافت نشد", subscriptionId);
                return false;
            }

            subscription.Extend(additionalDays);
            await _context.SaveChangesAsync();

            _logger.LogInformation("اشتراک {SubscriptionId} به مدت {AdditionalDays} روز تمدید شد", 
                subscriptionId, additionalDays);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در تمدید اشتراک {SubscriptionId}", subscriptionId);
            return false;
        }
    }

    /// <summary>
    /// استفاده از درخواست رایگان
    /// </summary>
    /// <param name="userId">شناسه کاربر</param>
    /// <param name="shipmentId">شناسه مرسوله</param>
    /// <param name="savedAmount">مبلغ صرفه‌جویی</param>
    /// <returns>وضعیت موفقیت</returns>
    public async Task<bool> UseFreeRequestAsync(string userId, Guid shipmentId, decimal savedAmount)
    {
        try
        {
            var subscription = await GetActiveSubscriptionAsync(userId);
            
            if (subscription == null || !subscription.CanUseFreeRequest())
            {
                return false;
            }

            var success = subscription.UseFreeRequest(shipmentId, savedAmount);
            
            if (success)
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("درخواست رایگان برای کاربر {UserId} و مرسوله {ShipmentId} استفاده شد", 
                    userId, shipmentId);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در استفاده از درخواست رایگان برای کاربر {UserId}", userId);
            return false;
        }
    }

    /// <summary>
    /// بررسی امکان استفاده از درخواست رایگان
    /// </summary>
    /// <param name="userId">شناسه کاربر</param>
    /// <returns>آیا می‌توان استفاده کرد؟</returns>
    public async Task<bool> CanUseFreeRequestAsync(string userId)
    {
        try
        {
            var subscription = await GetActiveSubscriptionAsync(userId);
            return subscription?.CanUseFreeRequest() ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در بررسی امکان استفاده از درخواست رایگان برای کاربر {UserId}", userId);
            return false;
        }
    }

    /// <summary>
    /// دریافت تنظیمات اشتراک بر اساس نوع
    /// </summary>
    /// <param name="subscriptionType">نوع اشتراک</param>
    /// <returns>تنظیمات اشتراک</returns>
    private static (string name, int maxFreeRequests, decimal price, int duration) GetSubscriptionSettings(string subscriptionType)
    {
        return subscriptionType.ToLower() switch
        {
            "basic" => ("پایه", 5, 99000, 30),
            "premium" => ("ویژه", 15, 199000, 30),
            "vip" => ("VIP", 50, 399000, 30),
            "enterprise" => ("سازمانی", 200, 999000, 30),
            _ => ("استاندارد", 10, 149000, 30)
        };
    }
}
