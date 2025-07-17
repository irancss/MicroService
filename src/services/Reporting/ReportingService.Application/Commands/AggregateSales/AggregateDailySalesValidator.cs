using FluentValidation;

namespace ReportingService.Application.Commands.AggregateSales;

public class AggregateDailySalesValidator : AbstractValidator<AggregateDailySalesCommand>
{
    public AggregateDailySalesValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required")
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("Cannot aggregate future dates");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Length(3)
            .WithMessage("Currency must be a 3-character code");
    }
}
