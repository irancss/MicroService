using FluentValidation;
using Cart.Application.Commands;

namespace Cart.Application.Validators;

public class AddItemToActiveCartCommandValidator : AbstractValidator<AddItemToActiveCartCommand>
{
    public AddItemToActiveCartCommandValidator()
    {
        RuleFor(x => x.CartId)
            .NotEmpty().WithMessage("Cart ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}

