using Shared.Kernel.Domain;
using ShippingService.Domain.Enums;

namespace ShippingService.Domain.Entities;

/// <summary>
/// قوانین ارسال رایگان که توسط ادمین تنظیم می‌شود
/// </summary>
public class FreeShippingRule : AggregateRoot
{
    /// <summary>
    /// نام قانون
    /// </summary>
    public string Name { get; private set; }
    
    /// <summary>
    /// توضیحات قانون
    /// </summary>
    public string? Description { get; private set; }
    
    /// <summary>
    /// آیا قانون فعال است؟
    /// </summary>
    public bool IsActive { get; private set; }
    
    /// <summary>
    /// اولویت قانون (عدد کمتر = اولویت بیشتر)
    /// </summary>
    public int Priority { get; private set; }
    
    /// <summary>
    /// نوع تخفیف
    /// </summary>
    public DiscountType DiscountType { get; private set; }
    
    /// <summary>
    /// مقدار تخفیف
    /// </summary>
    public decimal DiscountValue { get; private set; }
    
    /// <summary>
    /// حداکثر مبلغ تخفیف
    /// </summary>
    public decimal? MaxDiscountAmount { get; private set; }
    
    /// <summary>
    /// تاریخ شروع اعتبار قانون
    /// </summary>
    public DateTime? StartDate { get; private set; }
    
    /// <summary>
    /// تاریخ پایان اعتبار قانون
    /// </summary>
    public DateTime? EndDate { get; private set; }
    
    /// <summary>
    /// حداکثر تعداد استفاده از قانون
    /// </summary>
    public int? MaxUsageCount { get; private set; }
    
    /// <summary>
    /// تعداد استفاده فعلی
    /// </summary>
    public int CurrentUsageCount { get; private set; }
    
    /// <summary>
    /// آیا قانون فقط برای اشتراک‌های ویژه است؟
    /// </summary>
    public bool IsPremiumOnly { get; private set; }

    private readonly List<FreeShippingCondition> _conditions = new();
    
    /// <summary>
    /// شرایط اعمال قانون
    /// </summary>
    public IReadOnlyCollection<FreeShippingCondition> Conditions => _conditions.AsReadOnly();

    protected FreeShippingRule() 
    {
        Name = string.Empty;
    }

    /// <summary>
    /// سازنده قانون ارسال رایگان
    /// </summary>
    /// <param name="name">نام قانون</param>
    /// <param name="discountType">نوع تخفیف</param>
    /// <param name="discountValue">مقدار تخفیف</param>
    /// <param name="priority">اولویت</param>
    /// <param name="description">توضیحات</param>
    public FreeShippingRule(
        string name,
        DiscountType discountType,
        decimal discountValue,
        int priority = 1,
        string? description = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        DiscountType = discountType;
        DiscountValue = discountValue;
        Priority = priority;
        IsActive = true;
        CurrentUsageCount = 0;
        IsPremiumOnly = false;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// افزودن شرط به قانون
    /// </summary>
    /// <param name="condition">شرط جدید</param>
    public void AddCondition(FreeShippingCondition condition)
    {
        _conditions.Add(condition);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// حذف شرط از قانون
    /// </summary>
    /// <param name="conditionId">شناسه شرط</param>
    public void RemoveCondition(Guid conditionId)
    {
        var condition = _conditions.FirstOrDefault(c => c.Id == conditionId);
        if (condition != null)
        {
            _conditions.Remove(condition);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// تنظیم محدودیت زمانی
    /// </summary>
    /// <param name="startDate">تاریخ شروع</param>
    /// <param name="endDate">تاریخ پایان</param>
    public void SetDateRange(DateTime? startDate, DateTime? endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// تنظیم محدودیت استفاده
    /// </summary>
    /// <param name="maxUsageCount">حداکثر استفاده</param>
    public void SetUsageLimit(int? maxUsageCount)
    {
        MaxUsageCount = maxUsageCount;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// تنظیم حداکثر مبلغ تخفیف
    /// </summary>
    /// <param name="maxAmount">حداکثر مبلغ</param>
    public void SetMaxDiscountAmount(decimal? maxAmount)
    {
        MaxDiscountAmount = maxAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// تنظیم قانون برای کاربران ویژه
    /// </summary>
    /// <param name="isPremiumOnly">فقط کاربران ویژه</param>
    public void SetPremiumOnly(bool isPremiumOnly)
    {
        IsPremiumOnly = isPremiumOnly;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// فعال/غیرفعال کردن قانون
    /// </summary>
    /// <param name="isActive">وضعیت فعال</param>
    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// تغییر اولویت قانون
    /// </summary>
    /// <param name="priority">اولویت جدید</param>
    public void SetPriority(int priority)
    {
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// بررسی اعتبار قانون
    /// </summary>
    /// <returns>آیا قانون معتبر است؟</returns>
    public bool IsValid()
    {
        if (!IsActive) return false;
        
        var now = DateTime.UtcNow;
        
        // بررسی محدودیت زمانی
        if (StartDate.HasValue && now < StartDate.Value) return false;
        if (EndDate.HasValue && now > EndDate.Value) return false;
        
        // بررسی محدودیت استفاده
        if (MaxUsageCount.HasValue && CurrentUsageCount >= MaxUsageCount.Value) return false;
        
        return true;
    }

    /// <summary>
    /// استفاده از قانون
    /// </summary>
    public void Use()
    {
        CurrentUsageCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// محاسبه مبلغ تخفیف
    /// </summary>
    /// <param name="originalAmount">مبلغ اصلی</param>
    /// <returns>مبلغ تخفیف</returns>
    public decimal CalculateDiscount(decimal originalAmount)
    {
        if (!IsValid()) return 0;

        decimal discount = DiscountType switch
        {
            DiscountType.Percentage => originalAmount * (DiscountValue / 100),
            DiscountType.FixedAmount => DiscountValue,
            DiscountType.FreeShipping => originalAmount,
            _ => 0
        };

        // اعمال حداکثر تخفیف
        if (MaxDiscountAmount.HasValue && discount > MaxDiscountAmount.Value)
        {
            discount = MaxDiscountAmount.Value;
        }

        return Math.Min(discount, originalAmount);
    }

    /// <summary>
    /// فعال کردن قانون
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// غیرفعال کردن قانون
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
