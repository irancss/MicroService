using MediatR;

namespace ShippingService.Application.Features.FreeShippingRules.Commands
{
    /// <summary>
    /// Command for deleting a free shipping rule
    /// </summary>
    public record DeleteFreeShippingRuleCommand(Guid Id) : IRequest<DeleteFreeShippingRuleResponse>;

    /// <summary>
    /// Response for deleting a free shipping rule
    /// </summary>
    public class DeleteFreeShippingRuleResponse
    {
        public bool IsDeleted { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
