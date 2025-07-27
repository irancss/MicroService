using Cart.Application.Commands;
using FluentValidation;

namespace Cart.Application.Validators;

public class RemoveItemFromActiveCartCommandValidator : AbstractValidator<RemoveItemFromActiveCartCommand>
{
    public RemoveItemFromActiveCartCommandValidator()
    {
        RuleFor(x => x.CartId)
            .NotEmpty().WithMessage("Cart ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");
    }
}