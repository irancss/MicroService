using FluentValidation;
using Payment.Application.DTOs;

namespace Payment.Application.Validators;

public class CreateTransactionDtoValidator : AbstractValidator<CreateTransactionDto>
{
    public CreateTransactionDtoValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required")
            .MaximumLength(100)
            .WithMessage("Order ID cannot exceed 100 characters");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero")
            .LessThanOrEqualTo(1000000000) // 1 billion
            .WithMessage("Amount is too large");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Must(c => c == "IRR" || c == "IRT")
            .WithMessage("Currency must be IRR or IRT");

        RuleFor(x => x.GatewayName)
            .NotEmpty()
            .WithMessage("Gateway name is required")
            .MaximumLength(50)
            .WithMessage("Gateway name cannot exceed 50 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.CallbackUrl)
            .NotEmpty()
            .WithMessage("Callback URL is required")
            .Must(BeAValidUrl)
            .WithMessage("Callback URL is not valid");

        RuleFor(x => x.Mobile)
            .Matches(@"^09\d{9}$")
            .When(x => !string.IsNullOrEmpty(x.Mobile))
            .WithMessage("Mobile number format is invalid");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Email format is invalid");
    }

    private static bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}

public class WalletDepositDtoValidator : AbstractValidator<WalletDepositDto>
{
    public WalletDepositDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero")
            .LessThanOrEqualTo(100000000) // 100 million
            .WithMessage("Amount is too large");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Must(c => c == "IRR" || c == "IRT")
            .WithMessage("Currency must be IRR or IRT");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(250)
            .WithMessage("Description cannot exceed 250 characters");
    }
}

public class WalletWithdrawalDtoValidator : AbstractValidator<WalletWithdrawalDto>
{
    public WalletWithdrawalDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero")
            .LessThanOrEqualTo(100000000) // 100 million
            .WithMessage("Amount is too large");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Must(c => c == "IRR" || c == "IRT")
            .WithMessage("Currency must be IRR or IRT");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(250)
            .WithMessage("Description cannot exceed 250 characters");
    }
}

public class RefundRequestDtoValidator : AbstractValidator<RefundRequestDto>
{
    public RefundRequestDtoValidator()
    {
        RuleFor(x => x.TransactionId)
            .NotEmpty()
            .WithMessage("Transaction ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .When(x => x.Amount.HasValue)
            .WithMessage("Refund amount must be greater than zero");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Refund reason is required")
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters");
    }
}
