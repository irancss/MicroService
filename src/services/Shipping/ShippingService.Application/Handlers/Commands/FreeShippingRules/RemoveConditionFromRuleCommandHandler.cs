using MediatR;
using ShippingService.Application.Features.FreeShippingRules.Commands;
using ShippingService.Domain.Services;

namespace ShippingService.Application.Handlers.Commands.FreeShippingRules
{
    /// <summary>
    /// Handler for removing condition from free shipping rule
    /// </summary>
    public class RemoveConditionFromRuleCommandHandler : IRequestHandler<RemoveConditionFromRuleCommand, RemoveConditionFromRuleResponse>
    {
        private readonly IFreeShippingRuleService _freeShippingRuleService;

        public RemoveConditionFromRuleCommandHandler(IFreeShippingRuleService freeShippingRuleService)
        {
            _freeShippingRuleService = freeShippingRuleService;
        }

        public async Task<RemoveConditionFromRuleResponse> Handle(RemoveConditionFromRuleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var rule = await _freeShippingRuleService.GetRuleByIdAsync(request.RuleId);
                if (rule == null)
                {
                    return new RemoveConditionFromRuleResponse
                    {
                        Success = false,
                        Message = "قانون یافت نشد"
                    };
                }

                // Remove condition from rule
                rule.RemoveCondition(request.ConditionId);
                await _freeShippingRuleService.UpdateRuleAsync(rule);

                return new RemoveConditionFromRuleResponse
                {
                    Success = true,
                    Message = "شرط با موفقیت حذف شد"
                };
            }
            catch (Exception ex)
            {
                return new RemoveConditionFromRuleResponse
                {
                    Success = false,
                    Message = $"خطا در حذف شرط: {ex.Message}"
                };
            }
        }
    }
}
