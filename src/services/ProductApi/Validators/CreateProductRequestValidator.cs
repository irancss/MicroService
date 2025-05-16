using FluentValidation;
using ProductApi.Models.Dtos;


namespace ProductApi.Validators
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Sku)
                .NotEmpty().WithMessage("SKU is required.")
                .Length(3, 100).WithMessage("SKU must be between 3 and 100 characters.")
                .Matches("^[a-zA-Z0-9_-]+$").WithMessage("SKU can only contain letters, numbers, underscores, and hyphens."); // الگوی امن‌تر

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .Length(5, 200).WithMessage("Product name must be between 5 and 200 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.Categories)
                .NotEmpty().WithMessage("At least one category is required.");
            // .Must(list => list != null && list.Any()).WithMessage("At least one category is required."); // جایگزین NotEmpty برای List

            RuleForEach(x => x.Categories)
                .NotEmpty().WithMessage("Category name cannot be empty.");

            // اعتبارسنجی‌های بیشتر برای Attributes و ...
        }
    }
    // Validator برای UpdateProductRequest و سایر DTO ها نیز ایجاد شود.
}
