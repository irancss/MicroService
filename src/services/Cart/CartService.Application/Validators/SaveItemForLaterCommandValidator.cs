using Cart.Application.Commands;
using FluentValidation;

namespace Cart.Application.Validators;

public class SaveItemForLaterCommandValidator : AbstractValidator<SaveItemForLaterCommand>
{
    public SaveItemForLaterCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");
    }
}