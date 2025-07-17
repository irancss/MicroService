using Shared.Kernel.CQRS;
using ShippingService.Application.Commands;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Repositories;
using ShippingService.Domain.ValueObjects;
using ShippingService.Domain.Enums;

namespace ShippingService.Application.Handlers.Commands;

public class CreateShippingMethodCommandHandler : ICommandHandler<CreateShippingMethodCommand, Guid>
{
    private readonly IShippingMethodRepository _repository;

    public CreateShippingMethodCommandHandler(IShippingMethodRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateShippingMethodCommand request, CancellationToken cancellationToken)
    {
        var shippingMethod = new ShippingMethod(
            request.Name,
            request.Description,
            request.BaseCost,
            request.RequiresTimeSlot);

        await _repository.AddAsync(shippingMethod, cancellationToken);
        return shippingMethod.Id;
    }
}

public class UpdateShippingMethodCommandHandler : ICommandHandler<UpdateShippingMethodCommand>
{
    private readonly IShippingMethodRepository _repository;

    public UpdateShippingMethodCommandHandler(IShippingMethodRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdateShippingMethodCommand request, CancellationToken cancellationToken)
    {
        var shippingMethod = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Shipping method with ID {request.Id} not found");

        shippingMethod.UpdateBasicInfo(request.Name, request.Description, request.BaseCost);
        await _repository.UpdateAsync(shippingMethod, cancellationToken);
    }
}

public class DeleteShippingMethodCommandHandler : ICommandHandler<DeleteShippingMethodCommand>
{
    private readonly IShippingMethodRepository _repository;

    public DeleteShippingMethodCommandHandler(IShippingMethodRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteShippingMethodCommand request, CancellationToken cancellationToken)
    {
        var shippingMethod = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Shipping method with ID {request.Id} not found");

        shippingMethod.Deactivate();
        await _repository.UpdateAsync(shippingMethod, cancellationToken);
    }
}

public class AddCostRuleToMethodCommandHandler : ICommandHandler<AddCostRuleToMethodCommand>
{
    private readonly IShippingMethodRepository _repository;

    public AddCostRuleToMethodCommandHandler(IShippingMethodRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(AddCostRuleToMethodCommand request, CancellationToken cancellationToken)
    {
        var shippingMethod = await _repository.GetByIdAsync(request.ShippingMethodId, cancellationToken)
            ?? throw new KeyNotFoundException($"Shipping method with ID {request.ShippingMethodId} not found");

        var ruleType = (RuleType)request.RuleType;
        var costRule = new CostRule(ruleType, request.Value, request.Amount, request.IsPercentage);
        
        shippingMethod.AddCostRule(costRule);
        await _repository.UpdateAsync(shippingMethod, cancellationToken);
    }
}

public class AddRestrictionRuleToMethodCommandHandler : ICommandHandler<AddRestrictionRuleToMethodCommand>
{
    private readonly IShippingMethodRepository _repository;

    public AddRestrictionRuleToMethodCommandHandler(IShippingMethodRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(AddRestrictionRuleToMethodCommand request, CancellationToken cancellationToken)
    {
        var shippingMethod = await _repository.GetByIdAsync(request.ShippingMethodId, cancellationToken)
            ?? throw new KeyNotFoundException($"Shipping method with ID {request.ShippingMethodId} not found");

        var ruleType = (RuleType)request.RuleType;
        var restrictionRule = new RestrictionRule(ruleType, request.Value);
        
        shippingMethod.AddRestrictionRule(restrictionRule);
        await _repository.UpdateAsync(shippingMethod, cancellationToken);
    }
}
