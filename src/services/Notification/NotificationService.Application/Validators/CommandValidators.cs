using FluentValidation;
using NotificationService.Application.Commands;

namespace NotificationService.Application.Validators;

public class SendEmailCommandValidator : AbstractValidator<SendEmailCommand>
{
    public SendEmailCommandValidator()
    {
        RuleFor(x => x.To)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Valid email address is required");

        RuleFor(x => x.Subject)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Subject is required and must not exceed 200 characters");

        RuleFor(x => x.Body)
            .NotEmpty()
            .WithMessage("Email body is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");

        RuleFor(x => x.FromEmail)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.FromEmail))
            .WithMessage("From email must be a valid email address");
    }
}

public class SendSmsCommandValidator : AbstractValidator<SendSmsCommand>
{
    public SendSmsCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage("Valid phone number is required");

        RuleFor(x => x.Message)
            .NotEmpty()
            .MaximumLength(1600)
            .WithMessage("SMS message is required and must not exceed 1600 characters");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");
    }
}

public class CreateTemplateCommandValidator : AbstractValidator<CreateTemplateCommand>
{
    public CreateTemplateCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Template name is required and must not exceed 100 characters");

        RuleFor(x => x.Subject)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Subject is required and must not exceed 200 characters");

        RuleFor(x => x.Body)
            .NotEmpty()
            .WithMessage("Template body is required");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Valid notification type is required");

        RuleFor(x => x.CreatedBy)
            .NotEmpty()
            .WithMessage("CreatedBy is required");
    }
}

public class UpdateTemplateCommandValidator : AbstractValidator<UpdateTemplateCommand>
{
    public UpdateTemplateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Template ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Template name is required and must not exceed 100 characters");

        RuleFor(x => x.Subject)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Subject is required and must not exceed 200 characters");

        RuleFor(x => x.Body)
            .NotEmpty()
            .WithMessage("Template body is required");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Valid notification type is required");

        RuleFor(x => x.UpdatedBy)
            .NotEmpty()
            .WithMessage("UpdatedBy is required");
    }
}

public class QueueNotificationFromEventCommandValidator : AbstractValidator<QueueNotificationFromEventCommand>
{
    public QueueNotificationFromEventCommandValidator()
    {
        RuleFor(x => x.EventType)
            .NotEmpty()
            .WithMessage("Event type is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");

        RuleFor(x => x.Payload)
            .NotNull()
            .WithMessage("Payload is required");
    }
}
