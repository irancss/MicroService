using FluentValidation;

namespace CustomerService.Application.Commands.CreateCustomer
{
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
            RuleFor(x => x.Street).NotEmpty();
            RuleFor(x => x.City).NotEmpty();
            RuleFor(x => x.Country).NotEmpty();
            RuleFor(x => x.ZipCode).NotEmpty();
        }
    }
}
