using FluentValidation;
using DiscountService.Application.Features.Discounts.Commands;
using DiscountService.Domain.Enums;

namespace DiscountService.Application.Validators;

/// <summary>
/// Validator for CalculateDiscountCommand
/// </summary>
public class CalculateDiscountCommandValidator : AbstractValidator<CalculateDiscountCommand>
{
    public CalculateDiscountCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Cart items are required");

        RuleForEach(x => x.Items).SetValidator(new CartItemDtoValidator());

        RuleFor(x => x.ShippingCost)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Shipping cost must be non-negative");
    }
}

/// <summary>
/// Validator for CreateDiscountCommand
/// </summary>
public class CreateDiscountCommandValidator : AbstractValidator<CreateDiscountCommand>
{
    public CreateDiscountCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Discount name is required")
            .MaximumLength(200)
            .WithMessage("Discount name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Discount description is required")
            .MaximumLength(1000)
            .WithMessage("Discount description must not exceed 1000 characters");

        RuleFor(x => x.Value)
            .GreaterThan(0)
            .WithMessage("Discount value must be greater than 0");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required")
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after start date");

        When(x => x.Type == DiscountType.Percentage, () =>
        {
            RuleFor(x => x.Value)
                .LessThanOrEqualTo(100)
                .WithMessage("Percentage discount cannot exceed 100%");
        });

        When(x => x.Type == DiscountType.BuyXGetYFree, () =>
        {
            RuleFor(x => x.BuyQuantity)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Buy quantity must be specified and greater than 0 for BOGO discounts");

            RuleFor(x => x.GetQuantity)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Get quantity must be specified and greater than 0 for BOGO discounts");
        });

        When(x => x.Applicability == DiscountApplicability.SpecificProducts, () =>
        {
            RuleFor(x => x.ApplicableProductIds)
                .NotNull()
                .NotEmpty()
                .WithMessage("Product IDs must be specified when applicability is specific products");
        });

        When(x => x.Applicability == DiscountApplicability.SpecificCategories, () =>
        {
            RuleFor(x => x.ApplicableCategoryIds)
                .NotNull()
                .NotEmpty()
                .WithMessage("Category IDs must be specified when applicability is specific categories");
        });

        RuleFor(x => x.MaxTotalUsage)
            .GreaterThan(0)
            .When(x => x.MaxTotalUsage.HasValue)
            .WithMessage("Max total usage must be greater than 0");

        RuleFor(x => x.MaxUsagePerUser)
            .GreaterThan(0)
            .When(x => x.MaxUsagePerUser.HasValue)
            .WithMessage("Max usage per user must be greater than 0");

        RuleFor(x => x.MinimumCartAmount)
            .GreaterThan(0)
            .When(x => x.MinimumCartAmount.HasValue)
            .WithMessage("Minimum cart amount must be greater than 0");

        RuleFor(x => x.MaximumDiscountAmount)
            .GreaterThan(0)
            .When(x => x.MaximumDiscountAmount.HasValue)
            .WithMessage("Maximum discount amount must be greater than 0");
    }
}

/// <summary>
/// Validator for UpdateDiscountCommand
/// </summary>
public class UpdateDiscountCommandValidator : AbstractValidator<UpdateDiscountCommand>
{
    public UpdateDiscountCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Discount ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Discount name is required")
            .MaximumLength(200)
            .WithMessage("Discount name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Discount description is required")
            .MaximumLength(1000)
            .WithMessage("Discount description must not exceed 1000 characters");

        RuleFor(x => x.Value)
            .GreaterThan(0)
            .WithMessage("Discount value must be greater than 0");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required")
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after start date");

        When(x => x.Type == DiscountType.Percentage, () =>
        {
            RuleFor(x => x.Value)
                .LessThanOrEqualTo(100)
                .WithMessage("Percentage discount cannot exceed 100%");
        });

        When(x => x.Type == DiscountType.BuyXGetYFree, () =>
        {
            RuleFor(x => x.BuyQuantity)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Buy quantity must be specified and greater than 0 for BOGO discounts");

            RuleFor(x => x.GetQuantity)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Get quantity must be specified and greater than 0 for BOGO discounts");
        });

        When(x => x.Applicability == DiscountApplicability.SpecificProducts, () =>
        {
            RuleFor(x => x.ApplicableProductIds)
                .NotNull()
                .NotEmpty()
                .WithMessage("Product IDs must be specified when applicability is specific products");
        });

        When(x => x.Applicability == DiscountApplicability.SpecificCategories, () =>
        {
            RuleFor(x => x.ApplicableCategoryIds)
                .NotNull()
                .NotEmpty()
                .WithMessage("Category IDs must be specified when applicability is specific categories");
        });
    }
}

/// <summary>
/// Validator for CartItemDto
/// </summary>
public class CartItemDtoValidator : AbstractValidator<DiscountService.Application.DTOs.CartItemDto>
{
    public CartItemDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category ID is required");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .WithMessage("Unit price must be greater than 0");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");
    }
}
