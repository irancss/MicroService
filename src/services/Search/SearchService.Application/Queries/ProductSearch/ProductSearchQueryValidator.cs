using FluentValidation;

namespace SearchService.Application.Queries.ProductSearch;

public class ProductSearchQueryValidator : AbstractValidator<ProductSearchQuery>
{
    public ProductSearchQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.Size)
            .InclusiveBetween(1, 100)
            .WithMessage("Size must be between 1 and 100");

        RuleFor(x => x.Query)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Query))
            .WithMessage("Query must not exceed 500 characters");

        RuleFor(x => x.PriceRange)
            .Must(x => x == null || x.IsValid)
            .WithMessage("Invalid price range");

        RuleFor(x => x.RatingFilter)
            .Must(x => x == null || x.IsValid)
            .WithMessage("Invalid rating filter");
    }
}
