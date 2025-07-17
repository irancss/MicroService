using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Enums;
using ShippingService.Domain.Services;
using ShippingService.Infrastructure.Data;

namespace ShippingService.Infrastructure.Services;

/// <summary>
/// پیاده‌سازی سرویس مدیریت قوانین ارسال رایگان
/// </summary>
public class FreeShippingRuleService : IFreeShippingRuleService
{
    private readonly ShippingDbContext _context;
    private readonly ILogger<FreeShippingRuleService> _logger;

    /// <summary>
    /// سازنده سرویس قوانین ارسال رایگان
    /// </summary>
    /// <param name="context">کانتکست دیتابیس</param>
    /// <param name="logger">لاگر</param>
    public FreeShippingRuleService(
        ShippingDbContext context,
        ILogger<FreeShippingRuleService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// دریافت تمام قوانین فعال
    /// </summary>
    /// <returns>لیست قوانین فعال</returns>
    public async Task<IEnumerable<FreeShippingRule>> GetActiveRulesAsync()
    {
        try
        {
            return await _context.FreeShippingRules
                .Include(r => r.Conditions)
                .Where(r => r.IsActive)
                .OrderBy(r => r.Priority)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت قوانین فعال ارسال رایگان");
            return Enumerable.Empty<FreeShippingRule>();
        }
    }

    /// <summary>
    /// بررسی امکان ارسال رایگان برای مرسوله
    /// </summary>
    /// <param name="shipmentData">اطلاعات مرسوله</param>
    /// <param name="isPremiumUser">آیا کاربر ویژه است؟</param>
    /// <returns>قانون قابل اعمال و مبلغ تخفیف</returns>
    public async Task<(FreeShippingRule? rule, decimal discount)> CalculateFreeShippingAsync(
        ShipmentData shipmentData, bool isPremiumUser = false)
    {
        try
        {
            var activeRules = await GetActiveRulesAsync();
            
            foreach (var rule in activeRules.Where(r => r.IsValid()))
            {
                // بررسی قانون ویژه
                if (rule.IsPremiumOnly && !isPremiumUser)
                    continue;

                // بررسی شرایط قانون
                if (await EvaluateRuleConditionsAsync(rule, shipmentData))
                {
                    var originalShippingCost = await GetShippingCostAsync(shipmentData.ShippingMethodId);
                    var discount = rule.CalculateDiscount(originalShippingCost);
                    
                    _logger.LogInformation("قانون {RuleName} برای کاربر {UserId} اعمال شد با تخفیف {Discount}", 
                        rule.Name, shipmentData.UserId, discount);
                    
                    return (rule, discount);
                }
            }

            return (null, 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در محاسبه ارسال رایگان برای کاربر {UserId}", shipmentData.UserId);
            return (null, 0);
        }
    }

    /// <summary>
    /// اعمال قانون ارسال رایگان
    /// </summary>
    /// <param name="ruleId">شناسه قانون</param>
    /// <param name="shipmentId">شناسه مرسوله</param>
    /// <returns>وضعیت موفقیت</returns>
    public async Task<bool> ApplyRuleAsync(Guid ruleId, Guid shipmentId)
    {
        try
        {
            var rule = await _context.FreeShippingRules.FindAsync(ruleId);
            
            if (rule == null || !rule.IsValid())
            {
                return false;
            }

            rule.Use();
            await _context.SaveChangesAsync();

            _logger.LogInformation("قانون {RuleId} برای مرسوله {ShipmentId} اعمال شد", ruleId, shipmentId);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در اعمال قانون {RuleId} برای مرسوله {ShipmentId}", ruleId, shipmentId);
            return false;
        }
    }

    /// <summary>
    /// ایجاد قانون جدید
    /// </summary>
    /// <param name="rule">قانون جدید</param>
    /// <returns>قانون ایجاد شده</returns>
    public async Task<FreeShippingRule> CreateRuleAsync(FreeShippingRule rule)
    {
        try
        {
            _context.FreeShippingRules.Add(rule);
            await _context.SaveChangesAsync();

            _logger.LogInformation("قانون جدید {RuleName} ایجاد شد", rule.Name);
            
            return rule;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ایجاد قانون {RuleName}", rule.Name);
            throw;
        }
    }

    /// <summary>
    /// به‌روزرسانی قانون
    /// </summary>
    /// <param name="rule">قانون به‌روز شده</param>
    /// <returns>وضعیت موفقیت</returns>
    public async Task<bool> UpdateRuleAsync(FreeShippingRule rule)
    {
        try
        {
            _context.FreeShippingRules.Update(rule);
            await _context.SaveChangesAsync();

            _logger.LogInformation("قانون {RuleId} به‌روزرسانی شد", rule.Id);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در به‌روزرسانی قانون {RuleId}", rule.Id);
            return false;
        }
    }

    /// <summary>
    /// حذف قانون
    /// </summary>
    /// <param name="ruleId">شناسه قانون</param>
    /// <returns>وضعیت موفقیت</returns>
    public async Task<bool> DeleteRuleAsync(Guid ruleId)
    {
        try
        {
            var rule = await _context.FreeShippingRules.FindAsync(ruleId);
            
            if (rule == null)
            {
                return false;
            }

            _context.FreeShippingRules.Remove(rule);
            await _context.SaveChangesAsync();

            _logger.LogInformation("قانون {RuleId} حذف شد", ruleId);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در حذف قانون {RuleId}", ruleId);
            return false;
        }
    }

    /// <summary>
    /// دریافت قانون بر اساس شناسه
    /// </summary>
    /// <param name="ruleId">شناسه قانون</param>
    /// <returns>قانون ارسال رایگان</returns>
    public async Task<FreeShippingRule?> GetRuleByIdAsync(Guid ruleId)
    {
        try
        {
            return await _context.FreeShippingRules
                .Include(r => r.Conditions)
                .FirstOrDefaultAsync(r => r.Id == ruleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت قانون ارسال رایگان با شناسه {RuleId}", ruleId);
            return null;
        }
    }

    /// <summary>
    /// محاسبه ارسال رایگان بر اساس context
    /// </summary>
    /// <param name="context">کانتکست داده‌ها</param>
    /// <param name="originalCost">هزینه اصلی</param>
    /// <returns>نتیجه محاسبه</returns>
    public async Task<FreeShippingCalculationResult> CalculateFreeShippingAsync(Dictionary<string, object> context, decimal originalCost)
    {
        try
        {
            var activeRules = await GetActiveRulesAsync();

            foreach (var rule in activeRules)
            {
                if (await EvaluateRuleAsync(rule, context))
                {
                    var discount = CalculateDiscount(rule, originalCost);
                    return new FreeShippingCalculationResult
                    {
                        IsEligible = true,
                        OriginalCost = originalCost,
                        DiscountAmount = discount,
                        FinalCost = originalCost - discount,
                        AppliedRuleName = rule.Name,
                        RuleDescription = rule.Description
                    };
                }
            }

            return new FreeShippingCalculationResult
            {
                IsEligible = false,
                OriginalCost = originalCost,
                DiscountAmount = 0,
                FinalCost = originalCost
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در محاسبه ارسال رایگان");
            return new FreeShippingCalculationResult
            {
                IsEligible = false,
                OriginalCost = originalCost,
                DiscountAmount = 0,
                FinalCost = originalCost
            };
        }
    }

    /// <summary>
    /// بررسی شرایط یک قانون
    /// </summary>
    /// <param name="rule">قانون</param>
    /// <param name="shipmentData">اطلاعات مرسوله</param>
    /// <returns>آیا شرایط برقرار است؟</returns>
    private async Task<bool> EvaluateRuleConditionsAsync(FreeShippingRule rule, ShipmentData shipmentData)
    {
        var requiredConditions = rule.Conditions.Where(c => c.IsRequired).ToList();
        var optionalConditions = rule.Conditions.Where(c => !c.IsRequired).ToList();

        // تمام شرایط اجباری باید برقرار باشند
        foreach (var condition in requiredConditions)
        {
            if (!await EvaluateConditionAsync(condition, shipmentData))
            {
                return false;
            }
        }

        // حداقل یکی از شرایط اختیاری باید برقرار باشد (اگر شرط اختیاری وجود داشته باشد)
        if (optionalConditions.Any())
        {
            var optionalResults = new List<bool>();
            foreach (var condition in optionalConditions)
            {
                optionalResults.Add(await EvaluateConditionAsync(condition, shipmentData));
            }
            
            if (!optionalResults.Any(r => r))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// بررسی یک شرط
    /// </summary>
    /// <param name="condition">شرط</param>
    /// <param name="shipmentData">اطلاعات مرسوله</param>
    /// <returns>نتیجه بررسی</returns>
    private async Task<bool> EvaluateConditionAsync(FreeShippingCondition condition, ShipmentData shipmentData)
    {
        try
        {
            var value = GetFieldValue(condition.FieldName, shipmentData);
            return condition.Evaluate(value);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "خطا در بررسی شرط {ConditionId}", condition.Id);
            return false;
        }
    }

    /// <summary>
    /// دریافت مقدار فیلد از اطلاعات مرسوله
    /// </summary>
    /// <param name="fieldName">نام فیلد</param>
    /// <param name="shipmentData">اطلاعات مرسوله</param>
    /// <returns>مقدار فیلد</returns>
    private static object? GetFieldValue(string fieldName, ShipmentData shipmentData)
    {
        return fieldName.ToLower() switch
        {
            "orderamount" => shipmentData.OrderAmount,
            "itemcount" => shipmentData.ItemCount,
            "totalweight" => shipmentData.TotalWeight,
            "productcategories" => string.Join(",", shipmentData.ProductCategories),
            "shippingmethodid" => shipmentData.ShippingMethodId.ToString(),
            "destinationpostalcode" => shipmentData.DestinationPostalCode,
            "destinationcity" => shipmentData.DestinationCity,
            "dayofweek" => shipmentData.ShippingDate.DayOfWeek.ToString(),
            "userid" => shipmentData.UserId,
            "userlevel" => shipmentData.UserLevel,
            _ => shipmentData.AdditionalData.GetValueOrDefault(fieldName)
        };
    }

    /// <summary>
    /// دریافت هزینه ارسال
    /// </summary>
    /// <param name="shippingMethodId">شناسه روش ارسال</param>
    /// <returns>هزینه ارسال</returns>
    private async Task<decimal> GetShippingCostAsync(Guid shippingMethodId)
    {
        try
        {
            var shippingMethod = await _context.ShippingMethods
                .FindAsync(shippingMethodId);
            
            return shippingMethod?.BaseCost ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت هزینه ارسال برای روش {ShippingMethodId}", shippingMethodId);
            return 0;
        }
    }

    /// <summary>
    /// بررسی قانون بر اساس کانتکست
    /// </summary>
    private Task<bool> EvaluateRuleAsync(FreeShippingRule rule, Dictionary<string, object> context)
    {
        foreach (var condition in rule.Conditions)
        {
            if (!EvaluateConditionWithContext(condition, context))
            {
                return Task.FromResult(false); // اگر یکی از شرایط برقرار نباشد
            }
        }
        return Task.FromResult(true);
    }

    /// <summary>
    /// بررسی شرط بر اساس کانتکست
    /// </summary>
    private bool EvaluateConditionWithContext(FreeShippingCondition condition, Dictionary<string, object> context)
    {
        if (!context.TryGetValue(condition.FieldName, out var value))
        {
            return false;
        }

        return condition.Evaluate(value ?? string.Empty);
    }

    /// <summary>
    /// محاسبه مقدار تخفیف
    /// </summary>
    private decimal CalculateDiscount(FreeShippingRule rule, decimal originalCost)
    {
        return rule.DiscountType switch
        {
            DiscountType.Percentage => originalCost * (rule.DiscountValue / 100),
            DiscountType.FixedAmount => Math.Min(rule.DiscountValue, originalCost),
            DiscountType.FreeShipping => originalCost,
            _ => 0
        };
    }
}
