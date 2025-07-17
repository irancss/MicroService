using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Commands;
using NotificationService.Domain.Enums;

namespace NotificationService.Infrastructure.Consumers;

// Example event contracts - these would typically be in a shared library
public interface IUserRegisteredEvent
{
    Guid UserId { get; }
    string Email { get; }
    string FirstName { get; }
    string LastName { get; }
    DateTime RegisteredAt { get; }
}

public interface IOrderPlacedEvent
{
    Guid OrderId { get; }
    Guid UserId { get; }
    string UserEmail { get; }
    decimal TotalAmount { get; }
    DateTime PlacedAt { get; }
}

public interface IPasswordResetRequestedEvent
{
    Guid UserId { get; }
    string Email { get; }
    string ResetToken { get; }
    DateTime RequestedAt { get; }
}

// Consumers
public class UserRegisteredConsumer : IConsumer<IUserRegisteredEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserRegisteredConsumer> _logger;

    public UserRegisteredConsumer(IMediator mediator, ILogger<UserRegisteredConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IUserRegisteredEvent> context)
    {
        var message = context.Message;
        
        _logger.LogInformation("Processing user registered event for user {UserId}", message.UserId);

        var payload = new Dictionary<string, object>
        {
            { "email", message.Email },
            { "firstName", message.FirstName },
            { "lastName", message.LastName },
            { "registeredAt", message.RegisteredAt }
        };

        var command = new QueueNotificationFromEventCommand
        {
            EventType = "user_registered",
            UserId = message.UserId.ToString(),
            Payload = payload,
            Priority = NotificationPriority.Normal
        };

        await _mediator.Send(command);
    }
}

public class OrderPlacedConsumer : IConsumer<IOrderPlacedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderPlacedConsumer> _logger;

    public OrderPlacedConsumer(IMediator mediator, ILogger<OrderPlacedConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IOrderPlacedEvent> context)
    {
        var message = context.Message;
        
        _logger.LogInformation("Processing order placed event for order {OrderId}", message.OrderId);

        var payload = new Dictionary<string, object>
        {
            { "email", message.UserEmail },
            { "orderId", message.OrderId },
            { "totalAmount", message.TotalAmount },
            { "placedAt", message.PlacedAt }
        };

        var command = new QueueNotificationFromEventCommand
        {
            EventType = "order_placed",
            UserId = message.UserId.ToString(),
            Payload = payload,
            Priority = NotificationPriority.High
        };

        await _mediator.Send(command);
    }
}

public class PasswordResetRequestedConsumer : IConsumer<IPasswordResetRequestedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<PasswordResetRequestedConsumer> _logger;

    public PasswordResetRequestedConsumer(IMediator mediator, ILogger<PasswordResetRequestedConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IPasswordResetRequestedEvent> context)
    {
        var message = context.Message;
        
        _logger.LogInformation("Processing password reset requested event for user {UserId}", message.UserId);

        var payload = new Dictionary<string, object>
        {
            { "email", message.Email },
            { "resetToken", message.ResetToken },
            { "requestedAt", message.RequestedAt }
        };

        var command = new QueueNotificationFromEventCommand
        {
            EventType = "password_reset_requested",
            UserId = message.UserId.ToString(),
            Payload = payload,
            Priority = NotificationPriority.Critical
        };

        await _mediator.Send(command);
    }
}
