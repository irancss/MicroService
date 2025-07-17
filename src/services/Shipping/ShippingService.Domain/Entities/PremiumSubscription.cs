using Shared.Kernel.Domain;
using ShippingService.Domain.Enums;

namespace ShippingService.Domain.Entities;

/// <summary>
/// موجودیت اشتراک ویژه که به کاربران امکان استفاده از تخفیف‌ها و ارسال رایگان را می‌دهد
/// </summary>
public class PremiumSubscription : AggregateRoot
{
    /// <summary>
    /// شناسه کاربر صاحب اشتراک
    /// </summary>
    public string UserId { get; private set; }
    
    /// <summary>
    /// نام اشتراک
    /// </summary>
    public string Name { get; private set; }
    
    /// <summary>
    /// توضیحات اشتراک
    /// </summary>
    public string? Description { get; private set; }
    
    /// <summary>
    /// تاریخ شروع اشتراک
    /// </summary>
    public DateTime StartDate { get; private set; }
    
    /// <summary>
    /// تاریخ پایان اشتراک
    /// </summary>
    public DateTime EndDate { get; private set; }
    
    /// <summary>
    /// وضعیت اشتراک
    /// </summary>
    public SubscriptionStatus Status { get; private set; }
    
    /// <summary>
    /// تعداد درخواست‌های رایگان باقی‌مانده
    /// </summary>
    public int RemainingFreeRequests { get; private set; }
    
    /// <summary>
    /// حداکثر تعداد درخواست‌های رایگان در ماه
    /// </summary>
    public int MaxFreeRequestsPerMonth { get; private set; }
    
    /// <summary>
    /// تاریخ آخرین بازنشانی درخواست‌های رایگان
    /// </summary>
    public DateTime LastResetDate { get; private set; }
    
    /// <summary>
    /// آیا اشتراک فعال است؟
    /// </summary>
    public bool IsActive { get; private set; }
    
    /// <summary>
    /// مبلغ اشتراک
    /// </summary>
    public decimal Price { get; private set; }
    
    /// <summary>
    /// مدت اشتراک به روز
    /// </summary>
    public int DurationInDays { get; private set; }

    private readonly List<SubscriptionUsageLog> _usageLogs = new();
    
    /// <summary>
    /// لاگ‌های استفاده از اشتراک
    /// </summary>
    public IReadOnlyCollection<SubscriptionUsageLog> UsageLogs => _usageLogs.AsReadOnly();

    protected PremiumSubscription() { }

    /// <summary>
    /// سازنده اشتراک ویژه
    /// </summary>
    /// <param name="userId">شناسه کاربر</param>
    /// <param name="name">نام اشتراک</param>
    /// <param name="maxFreeRequestsPerMonth">حداکثر درخواست رایگان در ماه</param>
    /// <param name="price">قیمت اشتراک</param>
    /// <param name="durationInDays">مدت اشتراک به روز</param>
    /// <param name="description">توضیحات</param>
    public PremiumSubscription(
        string userId,
        string name,
        int maxFreeRequestsPerMonth,
        decimal price,
        int durationInDays,
        string? description = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Name = name;
        Description = description;
        MaxFreeRequestsPerMonth = maxFreeRequestsPerMonth;
        RemainingFreeRequests = maxFreeRequestsPerMonth;
        Price = price;
        DurationInDays = durationInDays;
        StartDate = DateTime.UtcNow;
        EndDate = StartDate.AddDays(durationInDays);
        LastResetDate = DateTime.UtcNow;
        Status = SubscriptionStatus.Active;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// استفاده از یک درخواست رایگان
    /// </summary>
    /// <param name="shipmentId">شناسه مرسوله</param>
    /// <param name="savedAmount">مبلغ صرفه‌جویی شده</param>
    /// <returns>آیا استفاده موفق بود؟</returns>
    public bool UseFreeRequest(Guid shipmentId, decimal savedAmount)
    {
        if (!CanUseFreeRequest())
            return false;

        RemainingFreeRequests--;
        UpdatedAt = DateTime.UtcNow;
        
        var usageLog = new SubscriptionUsageLog(Id, shipmentId, savedAmount);
        _usageLogs.Add(usageLog);
        
        return true;
    }

    /// <summary>
    /// بررسی امکان استفاده از درخواست رایگان
    /// </summary>
    /// <returns>آیا می‌توان استفاده کرد؟</returns>
    public bool CanUseFreeRequest()
    {
        CheckAndResetMonthlyRequests();
        return IsActive && Status == SubscriptionStatus.Active && 
               RemainingFreeRequests > 0 && DateTime.UtcNow <= EndDate;
    }

    /// <summary>
    /// تمدید اشتراک
    /// </summary>
    /// <param name="additionalDays">روزهای اضافی</param>
    public void Extend(int additionalDays)
    {
        EndDate = EndDate.AddDays(additionalDays);
        if (DateTime.UtcNow <= EndDate)
        {
            Status = SubscriptionStatus.Active;
            IsActive = true;
        }
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// لغو اشتراک
    /// </summary>
    public void Cancel()
    {
        Status = SubscriptionStatus.Cancelled;
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// تعلیق اشتراک
    /// </summary>
    public void Suspend()
    {
        Status = SubscriptionStatus.Suspended;
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// فعال‌سازی مجدد اشتراک
    /// </summary>
    public void Reactivate()
    {
        if (DateTime.UtcNow <= EndDate)
        {
            Status = SubscriptionStatus.Active;
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// بررسی و بازنشانی درخواست‌های ماهانه
    /// </summary>
    private void CheckAndResetMonthlyRequests()
    {
        var now = DateTime.UtcNow;
        if (now.Month != LastResetDate.Month || now.Year != LastResetDate.Year)
        {
            RemainingFreeRequests = MaxFreeRequestsPerMonth;
            LastResetDate = now;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// بررسی انقضای اشتراک
    /// </summary>
    public void CheckExpiration()
    {
        if (DateTime.UtcNow > EndDate && Status == SubscriptionStatus.Active)
        {
            Status = SubscriptionStatus.Expired;
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
