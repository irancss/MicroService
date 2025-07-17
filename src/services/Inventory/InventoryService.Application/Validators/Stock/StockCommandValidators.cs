using FluentValidation;
using InventoryService.Application.Commands.Stock;

namespace InventoryService.Application.Validators.Stock;

public class AdjustStockCommandValidator : AbstractValidator<AdjustStockCommand>
{
    public AdjustStockCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId is required");

        RuleFor(x => x.Quantity)
            .NotEqual(0)
            .WithMessage("Quantity cannot be zero");

        RuleFor(x => x.Quantity)
            .GreaterThan(-10000)
            .WithMessage("Quantity adjustment cannot be less than -10000");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters");
    }
}

public class ReserveStockCommandValidator : AbstractValidator<ReserveStockCommand>
{
    public ReserveStockCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.Quantity)
            .LessThanOrEqualTo(10000)
            .WithMessage("Cannot reserve more than 10000 items at once");

        RuleFor(x => x.Reference)
            .MaximumLength(100)
            .WithMessage("Reference cannot exceed 100 characters");
    }
}

public class CommitReservedStockCommandValidator : AbstractValidator<CommitReservedStockCommand>
{
    public CommitReservedStockCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.Reference)
            .MaximumLength(100)
            .WithMessage("Reference cannot exceed 100 characters");
    }
}

public class CancelReservedStockCommandValidator : AbstractValidator<CancelReservedStockCommand>
{
    public CancelReservedStockCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.Reference)
            .MaximumLength(100)
            .WithMessage("Reference cannot exceed 100 characters");
    }
}
