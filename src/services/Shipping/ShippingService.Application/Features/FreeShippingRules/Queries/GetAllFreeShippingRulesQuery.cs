using MediatR;
using ShippingService.Domain.Enums;

namespace ShippingService.Application.Features.FreeShippingRules.Queries
{
    /// <summary>
    /// Query for getting all free shipping rules
    /// </summary>
    public class GetAllFreeShippingRulesQuery : IRequest<GetAllFreeShippingRulesResponse>
    {
        public bool? IsActive { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// Response for getting all free shipping rules
    /// </summary>
    public class GetAllFreeShippingRulesResponse
    {
        public List<FreeShippingRuleDto> Rules { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    /// <summary>
    /// DTO for free shipping rule
    /// </summary>
    public class FreeShippingRuleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int Priority { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public int? MaxUsageCount { get; set; }
        public int CurrentUsageCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<FreeShippingConditionDto> Conditions { get; set; } = new();
    }

    /// <summary>
    /// DTO for free shipping condition
    /// </summary>
    public class FreeShippingConditionDto
    {
        public Guid Id { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public ConditionType ConditionType { get; set; }
        public ComparisonOperator Operator { get; set; }
        public Domain.Enums.ValueType ValueType { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}
