using FluentValidation;
using InventoryService.Application.Commands.Thresholds;

namespace InventoryService.Application.Validators.Thresholds;

public class UpdateThresholdsCommandValidator : AbstractValidator<UpdateThresholdsCommand>
{
    public UpdateThresholdsCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId is required");

        RuleFor(x => x.LowStockThreshold)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Low stock threshold must be 0 or greater");

        RuleFor(x => x.ExcessStockThreshold)
            .GreaterThan(0)
            .WithMessage("Excess stock threshold must be greater than 0");

        RuleFor(x => x.ExcessStockThreshold)
            .GreaterThan(x => x.LowStockThreshold)
            .WithMessage("Excess stock threshold must be greater than low stock threshold");
    }
}
