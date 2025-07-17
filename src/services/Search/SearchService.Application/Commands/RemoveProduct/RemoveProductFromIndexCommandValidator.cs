using FluentValidation;

namespace SearchService.Application.Commands.RemoveProduct;

public class RemoveProductFromIndexCommandValidator : AbstractValidator<RemoveProductFromIndexCommand>
{
    public RemoveProductFromIndexCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required for removal");
    }
}
