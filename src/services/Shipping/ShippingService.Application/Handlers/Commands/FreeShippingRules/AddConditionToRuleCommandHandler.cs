using MediatR;
using ShippingService.Application.Features.FreeShippingRules.Commands;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Services;

namespace ShippingService.Application.Handlers.Commands.FreeShippingRules
{
    /// <summary>
    /// Handler for adding condition to free shipping rule
    /// </summary>
    public class AddConditionToRuleCommandHandler : IRequestHandler<AddConditionToRuleCommand, AddConditionToRuleResponse>
    {
        private readonly IFreeShippingRuleService _freeShippingRuleService;

        public AddConditionToRuleCommandHandler(IFreeShippingRuleService freeShippingRuleService)
        {
            _freeShippingRuleService = freeShippingRuleService;
        }

        public async Task<AddConditionToRuleResponse> Handle(AddConditionToRuleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var rule = await _freeShippingRuleService.GetRuleByIdAsync(request.RuleId);
                if (rule == null)
                {
                    return new AddConditionToRuleResponse
                    {
                        Success = false,
                        Message = "قانون یافت نشد"
                    };
                }

                var condition = new FreeShippingCondition(
                    request.RuleId,
                    request.ConditionType,
                    request.FieldName,
                    request.Operator,
                    request.Value,
                    request.ValueType,
                    request.IsRequired,
                    request.Description
                );

                rule.AddCondition(condition);
                await _freeShippingRuleService.UpdateRuleAsync(rule);

                return new AddConditionToRuleResponse
                {
                    ConditionId = condition.Id,
                    Success = true,
                    Message = "شرط با موفقیت اضافه شد"
                };
            }
            catch (Exception ex)
            {
                return new AddConditionToRuleResponse
                {
                    Success = false,
                    Message = $"خطا در اضافه کردن شرط: {ex.Message}"
                };
            }
        }
    }
}
