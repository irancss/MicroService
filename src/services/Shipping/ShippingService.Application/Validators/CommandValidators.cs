using FluentValidation;
using ShippingService.Application.Commands;

namespace ShippingService.Application.Validators;

public class CreateShippingMethodCommandValidator : AbstractValidator<CreateShippingMethodCommand>
{
    public CreateShippingMethodCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .Length(1, 100).WithMessage("Name must be between 1 and 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.BaseCost)
            .GreaterThanOrEqualTo(0).WithMessage("Base cost cannot be negative");
    }
}

public class UpdateShippingMethodCommandValidator : AbstractValidator<UpdateShippingMethodCommand>
{
    public UpdateShippingMethodCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .Length(1, 100).WithMessage("Name must be between 1 and 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.BaseCost)
            .GreaterThanOrEqualTo(0).WithMessage("Base cost cannot be negative");
    }
}

public class AddCostRuleToMethodCommandValidator : AbstractValidator<AddCostRuleToMethodCommand>
{
    public AddCostRuleToMethodCommandValidator()
    {
        RuleFor(x => x.ShippingMethodId)
            .NotEmpty().WithMessage("Shipping method ID is required");

        RuleFor(x => x.RuleType)
            .IsInEnum().WithMessage("Invalid rule type");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Value is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero");

        RuleFor(x => x.Amount)
            .LessThanOrEqualTo(100)
            .When(x => x.IsPercentage)
            .WithMessage("Percentage amount cannot exceed 100%");
    }
}

public class CreateTimeSlotTemplateCommandValidator : AbstractValidator<CreateTimeSlotTemplateCommand>
{
    public CreateTimeSlotTemplateCommandValidator()
    {
        RuleFor(x => x.ShippingMethodId)
            .NotEmpty().WithMessage("Shipping method ID is required");

        RuleFor(x => x.DayOfWeek)
            .InclusiveBetween(0, 6).WithMessage("Day of week must be between 0 and 6");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than zero");

        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime).WithMessage("Start time must be before end time");
    }
}

public class ReserveTimeSlotCommandValidator : AbstractValidator<ReserveTimeSlotCommand>
{
    public ReserveTimeSlotCommandValidator()
    {
        RuleFor(x => x.ShippingMethodId)
            .NotEmpty().WithMessage("Shipping method ID is required");

        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.Date)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Date cannot be in the past");

        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime).WithMessage("Start time must be before end time");
    }
}
