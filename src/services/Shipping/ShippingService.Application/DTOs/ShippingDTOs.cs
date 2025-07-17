namespace ShippingService.Application.DTOs;

public class ShippingMethodDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BaseCost { get; set; }
    public bool IsActive { get; set; }
    public bool RequiresTimeSlot { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateShippingMethodDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BaseCost { get; set; }
    public bool RequiresTimeSlot { get; set; }
}

public class UpdateShippingMethodDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BaseCost { get; set; }
}

public class CostRuleDto
{
    public int RuleType { get; set; }
    public string Value { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsPercentage { get; set; }
    public bool IsActive { get; set; }
}

public class RestrictionRuleDto
{
    public int RuleType { get; set; }
    public string Value { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class TimeSlotTemplateDto
{
    public Guid Id { get; set; }
    public Guid ShippingMethodId { get; set; }
    public int DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int Capacity { get; set; }
    public bool IsActive { get; set; }
}

public class TimeSlotBookingDto
{
    public Guid Id { get; set; }
    public Guid TimeSlotTemplateId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string? OrderId { get; set; }
    public bool IsActive { get; set; }
}

public class AvailableShippingOptionDto
{
    public Guid ShippingMethodId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal FinalCost { get; set; }
    public bool RequiresTimeSlot { get; set; }
    public List<AvailableTimeSlotDto> AvailableTimeSlots { get; set; } = new();
}

public class AvailableTimeSlotDto
{
    public Guid TemplateId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int AvailableCapacity { get; set; }
}

public class CartItemDto
{
    public string ProductId { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Weight { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CustomerAddressDto
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
