using MassTransit;
using MediatR;
using MarketingService.Application.Features.UserSegments.Commands.AssignUserToSegment;

namespace MarketingService.Infrastructure.Consumers;

public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
{
    private readonly IMediator _mediator;

    public UserRegisteredConsumer(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var userEvent = context.Message;
        
        // Example: Automatically assign new users to a "New Users" segment
        var command = new AssignUserToSegmentCommand(
            userEvent.UserId,
            userEvent.DefaultSegmentId,
            "system-user-registration");

        await _mediator.Send(command);
    }
}

public record UserRegisteredEvent(
    Guid UserId,
    string Email,
    DateTime RegistrationDate,
    Guid DefaultSegmentId);
