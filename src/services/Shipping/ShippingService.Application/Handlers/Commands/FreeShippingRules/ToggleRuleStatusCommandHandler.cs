using MediatR;
using ShippingService.Application.Features.FreeShippingRules.Commands;
using ShippingService.Domain.Services;

namespace ShippingService.Application.Handlers.Commands.FreeShippingRules
{
    /// <summary>
    /// Handler for toggling free shipping rule status
    /// </summary>
    public class ToggleRuleStatusCommandHandler : IRequestHandler<ToggleRuleStatusCommand, ToggleRuleStatusResponse>
    {
        private readonly IFreeShippingRuleService _freeShippingRuleService;

        public ToggleRuleStatusCommandHandler(IFreeShippingRuleService freeShippingRuleService)
        {
            _freeShippingRuleService = freeShippingRuleService;
        }

        public async Task<ToggleRuleStatusResponse> Handle(ToggleRuleStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var rule = await _freeShippingRuleService.GetRuleByIdAsync(request.RuleId);
                if (rule == null)
                {
                    return new ToggleRuleStatusResponse
                    {
                        Success = false,
                        Message = "قانون یافت نشد"
                    };
                }

                if (request.IsActive)
                {
                    rule.Activate();
                }
                else
                {
                    rule.Deactivate();
                }

                await _freeShippingRuleService.UpdateRuleAsync(rule);

                return new ToggleRuleStatusResponse
                {
                    Success = true,
                    Message = request.IsActive ? "قانون فعال شد" : "قانون غیرفعال شد",
                    NewStatus = request.IsActive
                };
            }
            catch (Exception ex)
            {
                return new ToggleRuleStatusResponse
                {
                    Success = false,
                    Message = $"خطا در تغییر وضعیت قانون: {ex.Message}"
                };
            }
        }
    }
}
