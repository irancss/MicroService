using FluentValidation;

namespace ReportingService.Application.Commands.ProcessOrderData;

public class ProcessOrderCompletedEventValidator : AbstractValidator<ProcessOrderCompletedEventCommand>
{
    public ProcessOrderCompletedEventValidator()
    {
        RuleFor(x => x.OrderEvent)
            .NotNull()
            .WithMessage("Order event is required");

        RuleFor(x => x.OrderEvent.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.OrderEvent.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.OrderEvent.TotalAmount)
            .GreaterThan(0)
            .WithMessage("Total amount must be greater than zero");

        RuleFor(x => x.OrderEvent.Currency)
            .NotEmpty()
            .WithMessage("Currency is required");

        RuleFor(x => x.OrderEvent.Items)
            .NotEmpty()
            .WithMessage("Order must have at least one item");
    }
}
