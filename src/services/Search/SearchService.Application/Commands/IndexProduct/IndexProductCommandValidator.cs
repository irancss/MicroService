using FluentValidation;

namespace SearchService.Application.Commands.IndexProduct;

public class IndexProductCommandValidator : AbstractValidator<IndexProductCommand>
{
    public IndexProductCommandValidator()
    {
        RuleFor(x => x.Product.Id)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.Product.Name)
            .NotEmpty()
            .MaximumLength(500)
            .WithMessage("Product name is required and must not exceed 500 characters");

        RuleFor(x => x.Product.Category)
            .NotEmpty()
            .WithMessage("Product category is required");

        RuleFor(x => x.Product.Brand)
            .NotEmpty()
            .WithMessage("Product brand is required");

        RuleFor(x => x.Product.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Product price must be non-negative");

        RuleFor(x => x.Product.AverageRating)
            .InclusiveBetween(0, 5)
            .WithMessage("Average rating must be between 0 and 5");

        RuleFor(x => x.Product.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock quantity must be non-negative");
    }
}
