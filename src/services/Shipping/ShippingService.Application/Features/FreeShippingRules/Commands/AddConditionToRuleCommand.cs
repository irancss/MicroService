using MediatR;
using ShippingService.Domain.Enums;

namespace ShippingService.Application.Features.FreeShippingRules.Commands
{
    /// <summary>
    /// Command to add a condition to an existing free shipping rule
    /// </summary>
    public record AddConditionToRuleCommand(
        Guid RuleId,
        ConditionType ConditionType,
        string FieldName,
        ComparisonOperator Operator,
        string Value,
        Domain.Enums.ValueType ValueType,
        bool IsRequired = true,
        string? Description = null
    ) : IRequest<AddConditionToRuleResponse>;

    /// <summary>
    /// Response for adding condition to rule
    /// </summary>
    public class AddConditionToRuleResponse
    {
        public Guid ConditionId { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
