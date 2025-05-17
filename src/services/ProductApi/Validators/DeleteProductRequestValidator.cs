using FluentValidation;
using ProductApi.Models.Dtos;

namespace ProductApi.Validators
{
    public class DeleteProductRequestValidator : AbstractValidator<DeleteProductRequest>
    {
        public DeleteProductRequestValidator()
        {
            RuleFor(x => x.Sku)
                .NotEmpty().WithMessage("SKU is required.")
                .Length(3, 100).WithMessage("SKU must be between 3 and 100 characters.")
                .Matches("^[a-zA-Z0-9_-]+$").WithMessage("SKU can only contain letters, numbers, underscores, and hyphens.");
        }
    }
}
