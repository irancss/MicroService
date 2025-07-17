using MediatR;

namespace ShippingService.Application.Features.FreeShippingRules.Commands
{
    /// <summary>
    /// Command to remove a condition from a free shipping rule
    /// </summary>
    public record RemoveConditionFromRuleCommand(
        Guid RuleId,
        Guid ConditionId
    ) : IRequest<RemoveConditionFromRuleResponse>;

    /// <summary>
    /// Response for removing condition from rule
    /// </summary>
    public class RemoveConditionFromRuleResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
