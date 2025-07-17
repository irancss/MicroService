using FluentValidation;

namespace ReportingService.Application.Queries.GetDailySalesReport;

public class GetDailySalesReportValidator : AbstractValidator<GetDailySalesReportQuery>
{
    public GetDailySalesReportValidator()
    {
        RuleFor(x => x.FromDate)
            .NotEmpty()
            .WithMessage("From date is required")
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("From date cannot be in the future");

        RuleFor(x => x.ToDate)
            .NotEmpty()
            .WithMessage("To date is required")
            .GreaterThanOrEqualTo(x => x.FromDate)
            .WithMessage("To date must be greater than or equal to from date")
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("To date cannot be in the future");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Length(3)
            .WithMessage("Currency must be a 3-character code");

        RuleFor(x => x)
            .Must(x => (x.ToDate - x.FromDate).TotalDays <= 365)
            .WithMessage("Date range cannot exceed 365 days");
    }
}
