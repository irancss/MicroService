using ShippingService.Domain.Entities;

namespace ShippingService.Domain.Services
{

/// <summary>
/// سرویس مدیریت قوانین ارسال رایگان
/// </summary>
public interface IFreeShippingRuleService
{
    /// <summary>
    /// دریافت تمام قوانین فعال
    /// </summary>
    /// <returns>لیست قوانین فعال</returns>
    Task<IEnumerable<FreeShippingRule>> GetActiveRulesAsync();
    
    /// <summary>
    /// بررسی امکان ارسال رایگان برای مرسوله
    /// </summary>
    /// <param name="shipmentData">اطلاعات مرسوله</param>
    /// <param name="isPremiumUser">آیا کاربر ویژه است؟</param>
    /// <returns>قانون قابل اعمال و مبلغ تخفیف</returns>
    Task<(FreeShippingRule? rule, decimal discount)> CalculateFreeShippingAsync(
        ShipmentData shipmentData, bool isPremiumUser = false);
    
    /// <summary>
    /// اعمال قانون ارسال رایگان
    /// </summary>
    /// <param name="ruleId">شناسه قانون</param>
    /// <param name="shipmentId">شناسه مرسوله</param>
    /// <returns>وضعیت موفقیت</returns>
    Task<bool> ApplyRuleAsync(Guid ruleId, Guid shipmentId);
    
    /// <summary>
    /// ایجاد قانون جدید
    /// </summary>
    /// <param name="rule">قانون جدید</param>
    /// <returns>قانون ایجاد شده</returns>
    Task<FreeShippingRule> CreateRuleAsync(FreeShippingRule rule);
    
    /// <summary>
    /// به‌روزرسانی قانون
    /// </summary>
    /// <param name="rule">قانون به‌روز شده</param>
    /// <returns>وضعیت موفقیت</returns>
    Task<bool> UpdateRuleAsync(FreeShippingRule rule);
    
    /// <summary>
    /// حذف قانون
    /// </summary>
    /// <param name="ruleId">شناسه قانون</param>
    /// <returns>وضعیت موفقیت</returns>
    Task<bool> DeleteRuleAsync(Guid ruleId);

    /// <summary>
    /// دریافت قانون بر اساس شناسه
    /// </summary>
    /// <param name="ruleId">شناسه قانون</param>
    /// <returns>قانون ارسال رایگان</returns>
    Task<FreeShippingRule?> GetRuleByIdAsync(Guid ruleId);

    /// <summary>
    /// محاسبه ارسال رایگان بر اساس context
    /// </summary>
    /// <param name="context">کانتکست داده‌ها</param>
    /// <param name="originalCost">هزینه اصلی</param>
    /// <returns>نتیجه محاسبه</returns>
    Task<FreeShippingCalculationResult> CalculateFreeShippingAsync(Dictionary<string, object> context, decimal originalCost);
}

/// <summary>
/// Result of free shipping calculation
/// نتیجه محاسبه ارسال رایگان
/// </summary>
public class FreeShippingCalculationResult
{
    public bool IsEligible { get; set; }
    public decimal OriginalCost { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalCost { get; set; }
    public string? AppliedRuleName { get; set; }
    public string? RuleDescription { get; set; }
}

/// <summary>
/// اطلاعات مرسوله برای بررسی قوانین
/// </summary>
public class ShipmentData
{
    /// <summary>
    /// شناسه کاربر
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// مبلغ سفارش
    /// </summary>
    public decimal OrderAmount { get; set; }
    
    /// <summary>
    /// تعداد آیتم‌ها
    /// </summary>
    public int ItemCount { get; set; }
    
    /// <summary>
    /// وزن کل
    /// </summary>
    public decimal TotalWeight { get; set; }
    
    /// <summary>
    /// دسته‌بندی‌های محصولات
    /// </summary>
    public List<string> ProductCategories { get; set; } = new();
    
    /// <summary>
    /// شناسه روش ارسال
    /// </summary>
    public Guid ShippingMethodId { get; set; }
    
    /// <summary>
    /// کد پستی مقصد
    /// </summary>
    public string DestinationPostalCode { get; set; } = string.Empty;
    
    /// <summary>
    /// شهر مقصد
    /// </summary>
    public string DestinationCity { get; set; } = string.Empty;
    
    /// <summary>
    /// تاریخ ارسال
    /// </summary>
    public DateTime ShippingDate { get; set; }
    
    /// <summary>
    /// سطح کاربر
    /// </summary>
    public string UserLevel { get; set; } = string.Empty;
    
    /// <summary>
    /// اطلاعات اضافی
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}
}
