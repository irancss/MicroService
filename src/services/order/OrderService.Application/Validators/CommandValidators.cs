using FluentValidation;
using OrderService.Application.Commands;

namespace OrderService.Application.Validators
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage("Customer ID is required");

            RuleFor(x => x.TotalPrice)
                .GreaterThan(0)
                .WithMessage("Total price must be greater than 0");

            RuleFor(x => x.TotalDiscount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Total discount cannot be negative");

            RuleFor(x => x.ShippingAddress)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Shipping address is required and must be less than 500 characters");

            RuleFor(x => x.BillingAddress)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Billing address is required and must be less than 500 characters");

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("Order must contain at least one item");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(x => x.ProductId)
                    .NotEmpty()
                    .WithMessage("Product ID is required for each item");

                item.RuleFor(x => x.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be greater than 0");

                item.RuleFor(x => x.UnitPrice)
                    .GreaterThan(0)
                    .WithMessage("Unit price must be greater than 0");

                item.RuleFor(x => x.Discount)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Discount cannot be negative");
            });

            // Business rule: Total discount cannot exceed total price
            RuleFor(x => x)
                .Must(command => command.TotalDiscount <= command.TotalPrice)
                .WithMessage("Total discount cannot exceed total price");
        }
    }

    public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty()
                .WithMessage("Order ID is required");

            RuleFor(x => x.Reason)
                .NotEmpty()
                .MaximumLength(1000)
                .WithMessage("Cancellation reason is required and must be less than 1000 characters");

            RuleFor(x => x.CancelledBy)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Cancelled by field is required and must be less than 100 characters");
        }
    }

    public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
    {
        public UpdateOrderStatusCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty()
                .WithMessage("Order ID is required");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid order status");

            RuleFor(x => x.UpdatedBy)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Updated by field is required and must be less than 100 characters");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrEmpty(x.Notes))
                .WithMessage("Notes must be less than 1000 characters");
        }
    }
}
