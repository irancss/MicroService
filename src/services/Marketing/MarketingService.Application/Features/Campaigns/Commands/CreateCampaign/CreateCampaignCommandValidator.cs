using FluentValidation;

namespace MarketingService.Application.Features.Campaigns.Commands.CreateCampaign;

public class CreateCampaignCommandValidator : AbstractValidator<CreateCampaignCommand>
{
    public CreateCampaignCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Campaign name is required and must not exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(1000)
            .WithMessage("Campaign description is required and must not exceed 1000 characters");

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(100)
            .Matches("^[a-z0-9-]+$")
            .WithMessage("Slug must contain only lowercase letters, numbers, and hyphens");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Valid campaign type is required");

        RuleFor(x => x.StartDate)
            .GreaterThan(DateTime.UtcNow.AddDays(-1))
            .WithMessage("Start date must be today or in the future");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after start date");

        RuleFor(x => x.BudgetAmount)
            .GreaterThan(0)
            .WithMessage("Budget amount must be greater than zero");

        RuleFor(x => x.BudgetCurrency)
            .NotEmpty()
            .Length(3)
            .WithMessage("Budget currency must be a valid 3-letter currency code");

        RuleFor(x => x.CreatedBy)
            .NotEmpty()
            .WithMessage("Created by is required");
    }
}
