using MediatR;
using ShippingService.Application.Features.FreeShippingRules.Queries;
using ShippingService.Application.Services;

namespace ShippingService.Application.Handlers.Queries.FreeShippingRules
{
    /// <summary>
    /// Handler for getting all free shipping rules
    /// </summary>
    public class GetAllFreeShippingRulesQueryHandler : IRequestHandler<GetAllFreeShippingRulesQuery, GetAllFreeShippingRulesResponse>
    {
        private readonly IFreeShippingRuleService _freeShippingRuleService;

        public GetAllFreeShippingRulesQueryHandler(IFreeShippingRuleService freeShippingRuleService)
        {
            _freeShippingRuleService = freeShippingRuleService;
        }

        public async Task<GetAllFreeShippingRulesResponse> Handle(GetAllFreeShippingRulesQuery request, CancellationToken cancellationToken)
        {
            var rules = await _freeShippingRuleService.GetAllRulesAsync(request.IsActive);

            var ruleDtos = rules.Select(rule => new FreeShippingRuleDto
            {
                Id = rule.Id,
                Name = rule.Name,
                Description = rule.Description,
                IsActive = rule.IsActive,
                Priority = rule.Priority,
                DiscountType = rule.DiscountType,
                DiscountValue = rule.DiscountValue,
                MaxUsageCount = rule.MaxUsageCount,
                CurrentUsageCount = rule.CurrentUsageCount,
                CreatedAt = rule.CreatedAt,
                UpdatedAt = rule.UpdatedAt,
                Conditions = rule.Conditions.Select(c => new FreeShippingConditionDto
                {
                    Id = c.Id,
                    FieldName = c.FieldName,
                    ConditionType = c.ConditionType,
                    Operator = c.Operator,
                    ValueType = c.ValueType,
                    Value = c.Value
                }).ToList()
            }).ToList();

            // Apply pagination
            var paginatedRules = ruleDtos
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new GetAllFreeShippingRulesResponse
            {
                Rules = paginatedRules,
                TotalCount = ruleDtos.Count,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
