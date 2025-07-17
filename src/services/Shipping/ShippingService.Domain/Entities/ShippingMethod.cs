using Shared.Kernel.Domain;
using ShippingService.Domain.ValueObjects;

namespace ShippingService.Domain.Entities;

public class ShippingMethod : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal BaseCost { get; private set; }
    public bool IsActive { get; private set; }
    public bool RequiresTimeSlot { get; private set; }

    /// <summary>
    /// حداکثر وزن قابل ارسال (کیلوگرم)
    /// </summary>
    public decimal MaxWeight { get; private set; } = 1000;

    // Collections
    private readonly List<CostRule> _costRules = new();
    private readonly List<RestrictionRule> _restrictionRules = new();
    private readonly List<TimeSlotTemplate> _timeSlotTemplates = new();

    public IReadOnlyCollection<CostRule> CostRules => _costRules.AsReadOnly();
    public IReadOnlyCollection<RestrictionRule> RestrictionRules => _restrictionRules.AsReadOnly();
    public IReadOnlyCollection<TimeSlotTemplate> TimeSlotTemplates => _timeSlotTemplates.AsReadOnly();

    private ShippingMethod() { } // For EF Core

    public ShippingMethod(string name, string? description, decimal baseCost, bool requiresTimeSlot = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
        
        if (baseCost < 0)
            throw new ArgumentException("Base cost cannot be negative", nameof(baseCost));

        Name = name;
        Description = description;
        BaseCost = baseCost;
        RequiresTimeSlot = requiresTimeSlot;
        IsActive = true;
    }

    public void UpdateBasicInfo(string name, string? description, decimal baseCost)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
        
        if (baseCost < 0)
            throw new ArgumentException("Base cost cannot be negative", nameof(baseCost));

        Name = name;
        Description = description;
        BaseCost = baseCost;
        SetUpdated();
    }

    public void AddCostRule(CostRule costRule)
    {
        if (costRule == null)
            throw new ArgumentNullException(nameof(costRule));

        _costRules.Add(costRule);
        SetUpdated();
    }

    public void RemoveCostRule(CostRule costRule)
    {
        if (costRule != null)
        {
            costRule.Deactivate();
            SetUpdated();
        }
    }

    public void AddRestrictionRule(RestrictionRule restrictionRule)
    {
        if (restrictionRule == null)
            throw new ArgumentNullException(nameof(restrictionRule));

        _restrictionRules.Add(restrictionRule);
        SetUpdated();
    }

    public void RemoveRestrictionRule(RestrictionRule restrictionRule)
    {
        if (restrictionRule != null)
        {
            restrictionRule.Deactivate();
            SetUpdated();
        }
    }

    public void AddTimeSlotTemplate(TimeSlotTemplate timeSlotTemplate)
    {
        if (timeSlotTemplate == null)
            throw new ArgumentNullException(nameof(timeSlotTemplate));

        _timeSlotTemplates.Add(timeSlotTemplate);
        SetUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdated();
    }

    public decimal CalculateFinalCost(List<CartItem> cartItems, DateTime deliveryDate)
    {
        var finalCost = BaseCost;

        foreach (var rule in _costRules.Where(r => r.IsActive))
        {
            var additionalCost = ApplyCostRule(rule, cartItems, deliveryDate);
            finalCost += additionalCost;
        }

        return Math.Max(0, finalCost);
    }

    public bool IsAllowedForCart(List<CartItem> cartItems)
    {
        return _restrictionRules.Where(r => r.IsActive)
            .All(rule => !ViolatesRestriction(rule, cartItems));
    }

    private decimal ApplyCostRule(CostRule rule, List<CartItem> cartItems, DateTime deliveryDate)
    {
        return rule.RuleType switch
        {
            Enums.RuleType.DayOfWeek => ApplyDayOfWeekRule(rule, deliveryDate),
            Enums.RuleType.TimeRange => ApplyTimeRangeRule(rule, deliveryDate),
            Enums.RuleType.DateRange => ApplyDateRangeRule(rule, deliveryDate),
            Enums.RuleType.Weight => ApplyWeightRule(rule, cartItems),
            _ => 0
        };
    }

    private decimal ApplyDayOfWeekRule(CostRule rule, DateTime deliveryDate)
    {
        if (Enum.TryParse<DayOfWeek>(rule.Value, out var targetDay) && deliveryDate.DayOfWeek == targetDay)
        {
            return rule.IsPercentage ? BaseCost * (rule.Amount / 100) : rule.Amount;
        }
        return 0;
    }

    private decimal ApplyTimeRangeRule(CostRule rule, DateTime deliveryDate)
    {
        // Implementation for time range rules
        return 0;
    }

    private decimal ApplyDateRangeRule(CostRule rule, DateTime deliveryDate)
    {
        // Implementation for date range rules
        return 0;
    }

    private decimal ApplyWeightRule(CostRule rule, List<CartItem> cartItems)
    {
        var totalWeight = cartItems.Sum(item => item.Weight * item.Quantity);
        if (decimal.TryParse(rule.Value, out var weightThreshold) && totalWeight > weightThreshold)
        {
            return rule.IsPercentage ? BaseCost * (rule.Amount / 100) : rule.Amount;
        }
        return 0;
    }

    private bool ViolatesRestriction(RestrictionRule rule, List<CartItem> cartItems)
    {
        return rule.RuleType switch
        {
            Enums.RuleType.Product => cartItems.Any(item => item.ProductId == rule.Value),
            Enums.RuleType.Category => cartItems.Any(item => item.Category == rule.Value),
            Enums.RuleType.Weight => CheckWeightRestriction(rule, cartItems),
            _ => false
        };
    }

    private bool CheckWeightRestriction(RestrictionRule rule, List<CartItem> cartItems)
    {
        var totalWeight = cartItems.Sum(item => item.Weight * item.Quantity);
        return decimal.TryParse(rule.Value, out var maxWeight) && totalWeight > maxWeight;
    }

    /// <summary>
    /// Factory methods for creating standard shipping methods
    /// </summary>
    public static ShippingMethod CreateStandardShipping()
    {
        var method = new ShippingMethod(
            "Standard Shipping",
            "ارسال عادی 2-3 روز کاری",
            50000
        );
        method.MaxWeight = 30;
        return method;
    }

    public static ShippingMethod CreateExpressShipping()
    {
        var method = new ShippingMethod(
            "Express Shipping", 
            "ارسال سریع 1-2 روز کاری",
            100000
        );
        method.MaxWeight = 20;
        return method;
    }

    public static ShippingMethod CreateOvernightShipping()
    {
        var method = new ShippingMethod(
            "Overnight Shipping",
            "ارسال شبانه یک روز کاری",
            200000
        );
        method.MaxWeight = 10;
        return method;
    }

    public static ShippingMethod CreateEconomyShipping()
    {
        var method = new ShippingMethod(
            "Economy Shipping",
            "ارسال اقتصادی 5-7 روز کاری", 
            25000
        );
        method.MaxWeight = 50;
        return method;
    }
}

// Cart item DTO for domain calculations
public class CartItem
{
    public string ProductId { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Weight { get; set; }
    public decimal UnitPrice { get; set; }
}
