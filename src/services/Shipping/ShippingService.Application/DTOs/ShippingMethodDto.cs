namespace ShippingService.Application.DTOs;

// ...existing DTOs...

/// <summary>
/// DTO اشتراک ویژه
/// </summary>
public class PremiumSubscriptionDto
{
    /// <summary>
    /// شناسه اشتراک
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// شناسه کاربر
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// نام اشتراک
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// توضیحات
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// تاریخ شروع
    /// </summary>
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// تاریخ پایان
    /// </summary>
    public DateTime EndDate { get; set; }
    
    /// <summary>
    /// وضعیت اشتراک
    /// </summary>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// درخواست‌های رایگان باقی‌مانده
    /// </summary>
    public int RemainingFreeRequests { get; set; }
    
    /// <summary>
    /// حداکثر درخواست رایگان در ماه
    /// </summary>
    public int MaxFreeRequestsPerMonth { get; set; }
    
    /// <summary>
    /// آیا فعال است؟
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// قیمت اشتراک
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// مدت اشتراک به روز
    /// </summary>
    public int DurationInDays { get; set; }
    
    /// <summary>
    /// تاریخ ایجاد
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO لاگ استفاده از اشتراک
/// </summary>
public class SubscriptionUsageLogDto
{
    /// <summary>
    /// شناسه لاگ
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// شناسه اشتراک
    /// </summary>
    public Guid SubscriptionId { get; set; }
    
    /// <summary>
    /// شناسه مرسوله
    /// </summary>
    public Guid ShipmentId { get; set; }
    
    /// <summary>
    /// مبلغ صرفه‌جویی
    /// </summary>
    public decimal SavedAmount { get; set; }
    
    /// <summary>
    /// تاریخ استفاده
    /// </summary>
    public DateTime UsageDate { get; set; }
    
    /// <summary>
    /// توضیحات
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// DTO قانون ارسال رایگان
/// </summary>
public class FreeShippingRuleDto
{
    /// <summary>
    /// شناسه قانون
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// نام قانون
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// توضیحات
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// آیا فعال است؟
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// اولویت
    /// </summary>
    public int Priority { get; set; }
    
    /// <summary>
    /// نوع تخفیف
    /// </summary>
    public string DiscountType { get; set; } = string.Empty;
    
    /// <summary>
    /// مقدار تخفیف
    /// </summary>
    public decimal DiscountValue { get; set; }
    
    /// <summary>
    /// حداکثر مبلغ تخفیف
    /// </summary>
    public decimal? MaxDiscountAmount { get; set; }
    
    /// <summary>
    /// تاریخ شروع
    /// </summary>
    public DateTime? StartDate { get; set; }
    
    /// <summary>
    /// تاریخ پایان
    /// </summary>
    public DateTime? EndDate { get; set; }
    
    /// <summary>
    /// حداکثر استفاده
    /// </summary>
    public int? MaxUsageCount { get; set; }
    
    /// <summary>
    /// تعداد استفاده فعلی
    /// </summary>
    public int CurrentUsageCount { get; set; }
    
    /// <summary>
    /// فقط کاربران ویژه
    /// </summary>
    public bool IsPremiumOnly { get; set; }
    
    /// <summary>
    /// شرایط قانون
    /// </summary>
    public List<FreeShippingConditionDto> Conditions { get; set; } = new();
    
    /// <summary>
    /// تاریخ ایجاد
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO شرط قانون ارسال رایگان
/// </summary>
public class FreeShippingConditionDto
{
    /// <summary>
    /// شناسه شرط
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// نوع شرط
    /// </summary>
    public string ConditionType { get; set; } = string.Empty;
    
    /// <summary>
    /// نام فیلد
    /// </summary>
    public string FieldName { get; set; } = string.Empty;
    
    /// <summary>
    /// عملگر
    /// </summary>
    public string Operator { get; set; } = string.Empty;
    
    /// <summary>
    /// مقدار
    /// </summary>
    public string Value { get; set; } = string.Empty;
    
    /// <summary>
    /// نوع داده
    /// </summary>
    public string ValueType { get; set; } = string.Empty;
    
    /// <summary>
    /// اجباری
    /// </summary>
    public bool IsRequired { get; set; }
    
    /// <summary>
    /// توضیحات
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// DTO نتیجه محاسبه ارسال رایگان
/// </summary>
public class FreeShippingCalculationDto
{
    /// <summary>
    /// آیا ارسال رایگان اعمال می‌شود؟
    /// </summary>
    public bool IsFreeShippingApplicable { get; set; }
    
    /// <summary>
    /// قانون اعمال شده
    /// </summary>
    public FreeShippingRuleDto? AppliedRule { get; set; }
    
    /// <summary>
    /// مبلغ تخفیف
    /// </summary>
    public decimal DiscountAmount { get; set; }
    
    /// <summary>
    /// مبلغ اصلی ارسال
    /// </summary>
    public decimal OriginalShippingCost { get; set; }
    
    /// <summary>
    /// مبلغ نهایی ارسال
    /// </summary>
    public decimal FinalShippingCost { get; set; }
    
    /// <summary>
    /// پیام توضیحی
    /// </summary>
    public string? Message { get; set; }
}