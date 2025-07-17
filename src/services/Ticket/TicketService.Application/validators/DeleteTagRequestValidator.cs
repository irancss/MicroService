using FluentValidation;
using TicketService.Application.DTOs;

namespace TicketService.Application.validators;

public class DeleteTagRequestValidator : AbstractValidator<DeleteTagRequest>
{
    public DeleteTagRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}