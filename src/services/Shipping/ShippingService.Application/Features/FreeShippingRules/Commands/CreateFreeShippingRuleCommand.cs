using MediatR;
using ShippingService.Domain.Enums;

namespace ShippingService.Application.Features.FreeShippingRules.Commands
{
    /// <summary>
    /// Command for creating a new free shipping rule
    /// </summary>
    public class CreateFreeShippingRuleCommand : IRequest<CreateFreeShippingRuleResponse>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int Priority { get; set; } = 1;
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public int? MaxUsageCount { get; set; }
        public List<CreateFreeShippingConditionDto> Conditions { get; set; } = new();
    }

    /// <summary>
    /// DTO for creating free shipping conditions
    /// </summary>
    public class CreateFreeShippingConditionDto
    {
        public string FieldName { get; set; } = string.Empty;
        public ConditionType ConditionType { get; set; }
        public ComparisonOperator Operator { get; set; }
        public Domain.Enums.ValueType ValueType { get; set; }
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response for creating a free shipping rule
    /// </summary>
    public class CreateFreeShippingRuleResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
