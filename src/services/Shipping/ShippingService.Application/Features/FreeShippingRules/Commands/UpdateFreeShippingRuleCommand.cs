using MediatR;
using ShippingService.Domain.Enums;

namespace ShippingService.Application.Features.FreeShippingRules.Commands
{
    /// <summary>
    /// Command for updating an existing free shipping rule
    /// </summary>
    public class UpdateFreeShippingRuleCommand : IRequest<UpdateFreeShippingRuleResponse>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int Priority { get; set; } = 1;
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public int? MaxUsageCount { get; set; }
        public List<UpdateFreeShippingConditionDto> Conditions { get; set; } = new();
    }

    /// <summary>
    /// DTO for updating free shipping conditions
    /// </summary>
    public class UpdateFreeShippingConditionDto
    {
        public Guid? Id { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public ConditionType ConditionType { get; set; }
        public ComparisonOperator Operator { get; set; }
        public Domain.Enums.ValueType ValueType { get; set; }
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response for updating a free shipping rule
    /// </summary>
    public class UpdateFreeShippingRuleResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
