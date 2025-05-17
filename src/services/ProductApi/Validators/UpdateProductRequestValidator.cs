using FluentValidation;
using ProductApi.Models.Dtos;

namespace ProductApi.Validators
{
    public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductRequestValidator()
        {
            RuleFor(x => x.Sku)
                .NotEmpty().WithMessage("SKU is required.")
                .Length(3, 100).WithMessage("SKU must be between 3 and 100 characters.")
                .Matches("^[a-zA-Z0-9_-]+$").WithMessage("SKU can only contain letters, numbers, underscores, and hyphens.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .Length(5, 200).WithMessage("Product name must be between 5 and 200 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.Categories)
                .NotEmpty().WithMessage("At least one category is required.");

            RuleForEach(x => x.Categories)
                .NotEmpty().WithMessage("Category name cannot be empty.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.Attributes)
                .Must(attrs => attrs == null || attrs.Count <= 20)
                .WithMessage("A maximum of 20 attributes is allowed.");

            RuleForEach(x => x.Attributes)
                .Must(attr => !string.IsNullOrWhiteSpace(attr.Key))
                .WithMessage("Attribute key cannot be empty.")
                .Must(attr => attr.Value != null)
                .WithMessage("Attribute value cannot be null.");

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("IsActive must be specified.");
        }
    }
}

