using FluentValidation;
using Cart.Application.Commands;

namespace Cart.Application.Validators;

public class AddItemToActiveCartCommandValidator : AbstractValidator<AddItemToActiveCartCommand>
{
    public AddItemToActiveCartCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Quantity cannot exceed 100 per item");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.UserId) || !string.IsNullOrEmpty(x.GuestId))
            .WithMessage("Either UserId or GuestId must be provided");
    }
}

public class MoveItemToNextPurchaseCommandValidator : AbstractValidator<MoveItemToNextPurchaseCommand>
{
    public MoveItemToNextPurchaseCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .When(x => x.Quantity.HasValue)
            .WithMessage("Quantity must be greater than 0 when specified");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.UserId) || !string.IsNullOrEmpty(x.GuestId))
            .WithMessage("Either UserId or GuestId must be provided");
    }
}

public class UpdateCartItemQuantityCommandValidator : AbstractValidator<UpdateCartItemQuantityCommand>
{
    public UpdateCartItemQuantityCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Quantity cannot exceed 100 per item");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.UserId) || !string.IsNullOrEmpty(x.GuestId))
            .WithMessage("Either UserId or GuestId must be provided");
    }
}
