using MediatR;
using ShippingService.Application.Features.FreeShippingRules.Commands;
using ShippingService.Application.Services;
using ShippingService.Domain.Entities;

namespace ShippingService.Application.Handlers.Commands.FreeShippingRules
{
    /// <summary>
    /// Handler for creating free shipping rules
    /// </summary>
    public class CreateFreeShippingRuleCommandHandler : IRequestHandler<CreateFreeShippingRuleCommand, CreateFreeShippingRuleResponse>
    {
        private readonly IFreeShippingRuleService _freeShippingRuleService;

        public CreateFreeShippingRuleCommandHandler(IFreeShippingRuleService freeShippingRuleService)
        {
            _freeShippingRuleService = freeShippingRuleService;
        }

        public async Task<CreateFreeShippingRuleResponse> Handle(CreateFreeShippingRuleCommand request, CancellationToken cancellationToken)
        {
            var rule = new FreeShippingRule(
                request.Name,
                request.DiscountType,
                request.DiscountValue,
                request.Priority,
                request.Description
            );

            // Add conditions
            foreach (var conditionDto in request.Conditions)
            {
                var condition = new FreeShippingCondition(
                    rule.Id,
                    conditionDto.ConditionType,
                    conditionDto.FieldName,
                    conditionDto.Operator,
                    conditionDto.Value,
                    conditionDto.ValueType,
                    true
                );
                rule.AddCondition(condition);
            }

            var createdRule = await _freeShippingRuleService.CreateRuleAsync(rule);

            return new CreateFreeShippingRuleResponse
            {
                Id = createdRule.Id,
                Name = createdRule.Name,
                IsActive = createdRule.IsActive,
                CreatedAt = createdRule.CreatedAt
            };
        }
    }
}
