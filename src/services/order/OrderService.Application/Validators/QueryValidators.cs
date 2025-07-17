using FluentValidation;
using OrderService.Application.Queries;

namespace OrderService.Application.Validators
{
    public class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
    {
        public GetOrderByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Order ID is required");
        }
    }

    public class GetUserOrdersQueryValidator : AbstractValidator<GetUserOrdersQuery>
    {
        public GetUserOrdersQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required");

            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(100)
                .WithMessage("Page size must be between 1 and 100");

            RuleFor(x => x.SortBy)
                .MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.SortBy))
                .WithMessage("Sort field must be less than 50 characters");
        }
    }

    public class GetDashboardOrdersQueryValidator : AbstractValidator<GetDashboardOrdersQuery>
    {
        public GetDashboardOrdersQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(100)
                .WithMessage("Page size must be between 1 and 100");

            RuleFor(x => x.SortBy)
                .MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.SortBy))
                .WithMessage("Sort field must be less than 50 characters");

            // Ensure FromDate is before ToDate when both are provided
            RuleFor(x => x)
                .Must(query => !query.FromDate.HasValue || !query.ToDate.HasValue || query.FromDate <= query.ToDate)
                .WithMessage("From date must be before or equal to To date");

            // Prevent queries for too large date ranges (optional business rule)
            RuleFor(x => x)
                .Must(query => !query.FromDate.HasValue || !query.ToDate.HasValue || 
                              (query.ToDate.Value - query.FromDate.Value).TotalDays <= 365)
                .WithMessage("Date range cannot exceed 365 days");
        }
    }
}
