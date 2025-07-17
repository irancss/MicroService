using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Commands;
using NotificationService.Domain.Enums;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamplesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExamplesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Example: Send a welcome email to a new user
    /// </summary>
    [HttpPost("welcome-user")]
    public async Task<ActionResult> WelcomeUser([FromBody] WelcomeUserRequest request)
    {
        var command = new SendEmailCommand
        {
            To = request.Email,
            Subject = $"Welcome to Our Platform, {request.FirstName}!",
            Body = $@"
                <h1>Welcome {request.FirstName} {request.LastName}!</h1>
                <p>Thank you for joining our platform. We're excited to have you!</p>
                <p>Get started by exploring our features and setting up your profile.</p>
                <p>Best regards,<br>The Team</p>
            ",
            UserId = request.UserId,
            Priority = NotificationPriority.Normal
        };

        var result = await _mediator.Send(command);
        return Ok(new { success = result, message = "Welcome email sent" });
    }

    /// <summary>
    /// Example: Send order confirmation SMS
    /// </summary>
    [HttpPost("order-confirmation-sms")]
    public async Task<ActionResult> OrderConfirmationSms([FromBody] OrderConfirmationRequest request)
    {
        var command = new SendSmsCommand
        {
            PhoneNumber = request.PhoneNumber,
            Message = $"Order #{request.OrderId} confirmed! Total: ${request.Total:F2}. Thank you for your purchase!",
            UserId = request.UserId,
            Priority = NotificationPriority.High
        };

        var result = await _mediator.Send(command);
        return Ok(new { success = result, message = "Order confirmation SMS sent" });
    }

    /// <summary>
    /// Example: Simulate processing a user registration event
    /// </summary>
    [HttpPost("simulate-user-registered-event")]
    public async Task<ActionResult> SimulateUserRegisteredEvent([FromBody] UserRegisteredEventRequest request)
    {
        var command = new QueueNotificationFromEventCommand
        {
            EventType = "user_registered",
            UserId = request.UserId,
            Payload = new Dictionary<string, object>
            {
                { "email", request.Email },
                { "firstName", request.FirstName },
                { "lastName", request.LastName },
                { "registeredAt", DateTime.UtcNow },
                { "company_name", "Your Company" }
            },
            Priority = NotificationPriority.Normal
        };

        var result = await _mediator.Send(command);
        return Ok(new { success = result, message = "User registration event processed" });
    }

    /// <summary>
    /// Example: Simulate processing an order placed event
    /// </summary>
    [HttpPost("simulate-order-placed-event")]
    public async Task<ActionResult> SimulateOrderPlacedEvent([FromBody] OrderPlacedEventRequest request)
    {
        var command = new QueueNotificationFromEventCommand
        {
            EventType = "order_placed",
            UserId = request.UserId,
            Payload = new Dictionary<string, object>
            {
                { "email", request.Email },
                { "orderId", request.OrderId },
                { "totalAmount", request.TotalAmount },
                { "placedAt", DateTime.UtcNow }
            },
            Priority = NotificationPriority.High
        };

        var result = await _mediator.Send(command);
        return Ok(new { success = result, message = "Order placed event processed" });
    }

    /// <summary>
    /// Example: Send password reset email
    /// </summary>
    [HttpPost("password-reset")]
    public async Task<ActionResult> PasswordReset([FromBody] PasswordResetRequest request)
    {
        var resetToken = Guid.NewGuid().ToString("N");
        var resetUrl = "https://yourapp.com/reset-password";

        var command = new QueueNotificationFromEventCommand
        {
            EventType = "password_reset_requested",
            UserId = request.UserId,
            Payload = new Dictionary<string, object>
            {
                { "email", request.Email },
                { "resetToken", resetToken },
                { "requestedAt", DateTime.UtcNow },
                { "reset_url", resetUrl }
            },
            Priority = NotificationPriority.Critical
        };

        var result = await _mediator.Send(command);
        return Ok(new { success = result, message = "Password reset email sent", resetToken });
    }
}

// Request DTOs
public record WelcomeUserRequest(string UserId, string Email, string FirstName, string LastName);
public record OrderConfirmationRequest(string UserId, string PhoneNumber, string OrderId, decimal Total);
public record UserRegisteredEventRequest(string UserId, string Email, string FirstName, string LastName);
public record OrderPlacedEventRequest(string UserId, string Email, string OrderId, decimal TotalAmount);
public record PasswordResetRequest(string UserId, string Email);
