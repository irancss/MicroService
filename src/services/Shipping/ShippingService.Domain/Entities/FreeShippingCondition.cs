using Shared.Kernel.Domain;
using ShippingService.Domain.Enums;

namespace ShippingService.Domain.Entities;

/// <summary>
/// شرایط اعمال قوانین ارسال رایگان
/// </summary>
public class FreeShippingCondition : BaseEntity
{
    /// <summary>
    /// شناسه قانون مرتبط
    /// </summary>
    public Guid RuleId { get; private set; }
    
    /// <summary>
    /// نوع شرط
    /// </summary>
    public ConditionType ConditionType { get; private set; }
    
    /// <summary>
    /// فیلد مورد بررسی
    /// </summary>
    public string FieldName { get; private set; }
    
    /// <summary>
    /// عملگر مقایسه
    /// </summary>
    public ComparisonOperator Operator { get; private set; }
    
    /// <summary>
    /// مقدار مرجع برای مقایسه
    /// </summary>
    public string Value { get; private set; }
    
    /// <summary>
    /// نوع داده مقدار
    /// </summary>
    public Domain.Enums.ValueType ValueType { get; private set; }
    
    /// <summary>
    /// آیا شرط اجباری است؟ (AND)
    /// </summary>
    public bool IsRequired { get; private set; }
    
    /// <summary>
    /// توضیحات شرط
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// قانون مرتبط
    /// </summary>
    public FreeShippingRule Rule { get; private set; } = null!;

    protected FreeShippingCondition() 
    {
        FieldName = string.Empty;
        Value = string.Empty;
    }

    /// <summary>
    /// سازنده شرط ارسال رایگان
    /// </summary>
    /// <param name="ruleId">شناسه قانون</param>
    /// <param name="conditionType">نوع شرط</param>
    /// <param name="fieldName">نام فیلد</param>
    /// <param name="operator">عملگر</param>
    /// <param name="value">مقدار</param>
    /// <param name="valueType">نوع داده</param>
    /// <param name="isRequired">اجباری</param>
    /// <param name="description">توضیحات</param>
    public FreeShippingCondition(
        Guid ruleId,
        ConditionType conditionType,
        string fieldName,
        ComparisonOperator @operator,
        string value,
        Domain.Enums.ValueType valueType,
        bool isRequired = true,
        string? description = null)
    {
        Id = Guid.NewGuid();
        RuleId = ruleId;
        ConditionType = conditionType;
        FieldName = fieldName;
        Operator = @operator;
        Value = value;
        ValueType = valueType;
        IsRequired = isRequired;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// بررسی شرط بر اساس مقادیر ورودی
    /// </summary>
    /// <param name="inputValue">مقدار ورودی</param>
    /// <returns>نتیجه بررسی</returns>
    public bool Evaluate(object inputValue)
    {
        try
        {        return ValueType switch
        {
            Domain.Enums.ValueType.String => EvaluateString(inputValue?.ToString(), Value),
            Domain.Enums.ValueType.Number => EvaluateNumber(inputValue, Value),
            Domain.Enums.ValueType.Boolean => EvaluateBoolean(inputValue, Value),
            Domain.Enums.ValueType.DateTime => EvaluateDateTime(inputValue, Value),
            Domain.Enums.ValueType.Decimal => EvaluateDecimal(inputValue, Value),
            _ => false
        };
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// مقایسه مقادیر رشته‌ای
    /// </summary>
    private bool EvaluateString(string? input, string reference)
    {
        if (input == null) return false;
        
        return Operator switch
        {
            ComparisonOperator.Equals => string.Equals(input, reference, StringComparison.OrdinalIgnoreCase),
            ComparisonOperator.NotEquals => !string.Equals(input, reference, StringComparison.OrdinalIgnoreCase),
            ComparisonOperator.Contains => input.Contains(reference, StringComparison.OrdinalIgnoreCase),
            ComparisonOperator.StartsWith => input.StartsWith(reference, StringComparison.OrdinalIgnoreCase),
            ComparisonOperator.EndsWith => input.EndsWith(reference, StringComparison.OrdinalIgnoreCase),
            ComparisonOperator.In => reference.Split(',').Any(r => string.Equals(input.Trim(), r.Trim(), StringComparison.OrdinalIgnoreCase)),
            _ => false
        };
    }

    /// <summary>
    /// مقایسه مقادیر عددی
    /// </summary>
    private bool EvaluateNumber(object? input, string reference)
    {
        if (!double.TryParse(input?.ToString(), out double inputNumber) ||
            !double.TryParse(reference, out double referenceNumber))
            return false;

        return Operator switch
        {
            ComparisonOperator.Equals => Math.Abs(inputNumber - referenceNumber) < 0.001,
            ComparisonOperator.NotEquals => Math.Abs(inputNumber - referenceNumber) >= 0.001,
            ComparisonOperator.GreaterThan => inputNumber > referenceNumber,
            ComparisonOperator.GreaterThanOrEqual => inputNumber >= referenceNumber,
            ComparisonOperator.LessThan => inputNumber < referenceNumber,
            ComparisonOperator.LessThanOrEqual => inputNumber <= referenceNumber,
            _ => false
        };
    }

    /// <summary>
    /// مقایسه مقادیر دهدهی
    /// </summary>
    private bool EvaluateDecimal(object? input, string reference)
    {
        if (!decimal.TryParse(input?.ToString(), out decimal inputDecimal) ||
            !decimal.TryParse(reference, out decimal referenceDecimal))
            return false;

        return Operator switch
        {
            ComparisonOperator.Equals => inputDecimal == referenceDecimal,
            ComparisonOperator.NotEquals => inputDecimal != referenceDecimal,
            ComparisonOperator.GreaterThan => inputDecimal > referenceDecimal,
            ComparisonOperator.GreaterThanOrEqual => inputDecimal >= referenceDecimal,
            ComparisonOperator.LessThan => inputDecimal < referenceDecimal,
            ComparisonOperator.LessThanOrEqual => inputDecimal <= referenceDecimal,
            _ => false
        };
    }

    /// <summary>
    /// مقایسه مقادیر بولی
    /// </summary>
    private bool EvaluateBoolean(object? input, string reference)
    {
        if (!bool.TryParse(input?.ToString(), out bool inputBool) ||
            !bool.TryParse(reference, out bool referenceBool))
            return false;

        return Operator switch
        {
            ComparisonOperator.Equals => inputBool == referenceBool,
            ComparisonOperator.NotEquals => inputBool != referenceBool,
            _ => false
        };
    }

    /// <summary>
    /// مقایسه مقادیر تاریخ
    /// </summary>
    private bool EvaluateDateTime(object? input, string reference)
    {
        if (!DateTime.TryParse(input?.ToString(), out DateTime inputDate) ||
            !DateTime.TryParse(reference, out DateTime referenceDate))
            return false;

        return Operator switch
        {
            ComparisonOperator.Equals => inputDate.Date == referenceDate.Date,
            ComparisonOperator.NotEquals => inputDate.Date != referenceDate.Date,
            ComparisonOperator.GreaterThan => inputDate > referenceDate,
            ComparisonOperator.GreaterThanOrEqual => inputDate >= referenceDate,
            ComparisonOperator.LessThan => inputDate < referenceDate,
            ComparisonOperator.LessThanOrEqual => inputDate <= referenceDate,
            _ => false
        };
    }
}
