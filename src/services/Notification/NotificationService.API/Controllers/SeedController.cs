using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Commands;
using NotificationService.Domain.Enums;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly IMediator _mediator;

    public SeedController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("templates")]
    public async Task<ActionResult> SeedTemplates()
    {
        // Welcome Email Template
        await _mediator.Send(new CreateTemplateCommand
        {
            Name = "user_registered_notification",
            Type = NotificationType.Email,
            Subject = "Welcome to {{ company_name }}!",
            Body = @"
                <h1>Welcome {{ firstName }} {{ lastName }}!</h1>
                <p>Thank you for registering with us on {{ registeredAt | date.to_string '%Y-%m-%d' }}.</p>
                <p>We're excited to have you as part of our community!</p>
                <p>Best regards,<br>The {{ company_name }} Team</p>
            ",
            Language = "en",
            Parameters = new Dictionary<string, string>
            {
                { "firstName", "User's first name" },
                { "lastName", "User's last name" },
                { "registeredAt", "Registration date" },
                { "company_name", "Company name" }
            },
            CreatedBy = "System"
        });

        // Order Confirmation Email
        await _mediator.Send(new CreateTemplateCommand
        {
            Name = "order_placed_notification",
            Type = NotificationType.Email,
            Subject = "Order Confirmation #{{ orderId }}",
            Body = @"
                <h1>Order Confirmation</h1>
                <p>Dear Customer,</p>
                <p>Thank you for your order! Your order #{{ orderId }} has been successfully placed on {{ placedAt | date.to_string '%Y-%m-%d %H:%M' }}.</p>
                <p><strong>Order Total: ${{ totalAmount }}</strong></p>
                <p>We'll send you another email once your order ships.</p>
                <p>Best regards,<br>Your Store Team</p>
            ",
            Language = "en",
            Parameters = new Dictionary<string, string>
            {
                { "orderId", "Order ID" },
                { "totalAmount", "Order total amount" },
                { "placedAt", "Order placement date" }
            },
            CreatedBy = "System"
        });

        // Password Reset Email
        await _mediator.Send(new CreateTemplateCommand
        {
            Name = "password_reset_requested_notification",
            Type = NotificationType.Email,
            Subject = "Password Reset Request",
            Body = @"
                <h1>Password Reset</h1>
                <p>We received a request to reset your password on {{ requestedAt | date.to_string '%Y-%m-%d %H:%M' }}.</p>
                <p>Click the link below to reset your password:</p>
                <p><a href='{{ reset_url }}?token={{ resetToken }}'>Reset Password</a></p>
                <p>If you didn't request this, please ignore this email.</p>
                <p>Best regards,<br>Support Team</p>
            ",
            Language = "en",
            Parameters = new Dictionary<string, string>
            {
                { "resetToken", "Password reset token" },
                { "requestedAt", "Reset request date" },
                { "reset_url", "Password reset URL" }
            },
            CreatedBy = "System"
        });

        // Order SMS Notification
        await _mediator.Send(new CreateTemplateCommand
        {
            Name = "order_placed_sms_notification",
            Type = NotificationType.Sms,
            Subject = "Order SMS",
            Body = "Your order #{{ orderId }} for ${{ totalAmount }} has been confirmed. Thank you for your purchase!",
            Language = "en",
            Parameters = new Dictionary<string, string>
            {
                { "orderId", "Order ID" },
                { "totalAmount", "Order total amount" }
            },
            CreatedBy = "System"
        });

        return Ok("Templates seeded successfully");
    }
}
