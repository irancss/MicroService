using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using ShippingService.Domain.Services;
using ShippingService.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ShippingService.Infrastructure.Services;

public class TwilioNotificationService : INotificationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TwilioNotificationService> _logger;
    private readonly string _twilioAccountSid;
    private readonly string _twilioAuthToken;
    private readonly string _twilioPhoneNumber;

    public TwilioNotificationService(
        IConfiguration configuration,
        ILogger<TwilioNotificationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _twilioAccountSid = _configuration["Twilio:AccountSid"] ?? "";
        _twilioAuthToken = _configuration["Twilio:AuthToken"] ?? "";
        _twilioPhoneNumber = _configuration["Twilio:PhoneNumber"] ?? "";

        if (!string.IsNullOrEmpty(_twilioAccountSid) && !string.IsNullOrEmpty(_twilioAuthToken))
        {
            TwilioClient.Init(_twilioAccountSid, _twilioAuthToken);
        }
    }

    public async Task SendShipmentStatusUpdateAsync(string customerId, string trackingNumber, ShipmentStatus status, string? message = null)
    {
        var statusMessage = GetShipmentStatusMessage(status);
        var fullMessage = $"Shipment Update - {trackingNumber}: {statusMessage}";
        
        if (!string.IsNullOrEmpty(message))
        {
            fullMessage += $" - {message}";
        }

        // In a real implementation, you would get customer contact info from user service
        var phoneNumber = GetCustomerPhoneNumber(customerId);
        if (!string.IsNullOrEmpty(phoneNumber))
        {
            await SendSmsAsync(phoneNumber, fullMessage);
        }

        _logger.LogInformation("Shipment status notification sent for {TrackingNumber}: {Status}", trackingNumber, status);
    }

    public async Task SendReturnStatusUpdateAsync(string customerId, string returnTrackingNumber, ReturnStatus status, string? message = null)
    {
        var statusMessage = GetReturnStatusMessage(status);
        var fullMessage = $"Return Update - {returnTrackingNumber}: {statusMessage}";
        
        if (!string.IsNullOrEmpty(message))
        {
            fullMessage += $" - {message}";
        }

        var phoneNumber = GetCustomerPhoneNumber(customerId);
        if (!string.IsNullOrEmpty(phoneNumber))
        {
            await SendSmsAsync(phoneNumber, fullMessage);
        }

        _logger.LogInformation("Return status notification sent for {ReturnTrackingNumber}: {Status}", returnTrackingNumber, status);
    }

    public async Task SendDeliveryNotificationAsync(string customerId, string trackingNumber, DateTime estimatedDeliveryTime)
    {
        var message = $"Your package {trackingNumber} is scheduled for delivery on {estimatedDeliveryTime:MMM dd, yyyy} at approximately {estimatedDeliveryTime:HH:mm}";
        
        var phoneNumber = GetCustomerPhoneNumber(customerId);
        if (!string.IsNullOrEmpty(phoneNumber))
        {
            await SendSmsAsync(phoneNumber, message);
        }

        _logger.LogInformation("Delivery notification sent for {TrackingNumber}", trackingNumber);
    }

    public async Task SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            if (string.IsNullOrEmpty(_twilioAccountSid) || string.IsNullOrEmpty(_twilioAuthToken))
            {
                _logger.LogWarning("Twilio credentials not configured. SMS not sent to {PhoneNumber}", phoneNumber);
                return;
            }

            var messageResource = await MessageResource.CreateAsync(
                body: message,
                from: new PhoneNumber(_twilioPhoneNumber),
                to: new PhoneNumber(phoneNumber)
            );

            _logger.LogInformation("SMS sent successfully to {PhoneNumber}. SID: {MessageSid}", phoneNumber, messageResource.Sid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SMS to {PhoneNumber}", phoneNumber);
            throw;
        }
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        // This is a placeholder - you would integrate with an email service like SendGrid
        await Task.Delay(100);
        _logger.LogInformation("Email sent to {Email} with subject: {Subject}", email, subject);
    }

    public async Task SendPushNotificationAsync(string userId, string title, string message, object? data = null)
    {
        // This is a placeholder - you would integrate with a push notification service like Firebase
        await Task.Delay(100);
        _logger.LogInformation("Push notification sent to user {UserId}: {Title}", userId, title);
    }

    private static string GetShipmentStatusMessage(ShipmentStatus status)
    {
        return status switch
        {
            ShipmentStatus.Created => "Your shipment has been created and is being processed.",
            ShipmentStatus.PickupScheduled => "Pickup has been scheduled for your shipment.",
            ShipmentStatus.PickedUp => "Your shipment has been picked up.",
            ShipmentStatus.InTransit => "Your shipment is in transit.",
            ShipmentStatus.OutForDelivery => "Your shipment is out for delivery.",
            ShipmentStatus.Delivered => "Your shipment has been delivered successfully.",
            ShipmentStatus.Failed => "Delivery attempt failed. We will retry soon.",
            ShipmentStatus.Cancelled => "Your shipment has been cancelled.",
            ShipmentStatus.Returned => "Your shipment is being returned.",
            _ => "Shipment status updated."
        };
    }

    private static string GetReturnStatusMessage(ReturnStatus status)
    {
        return status switch
        {
            ReturnStatus.Requested => "Your return request has been submitted and is under review.",
            ReturnStatus.Approved => "Your return request has been approved.",
            ReturnStatus.Rejected => "Your return request has been rejected.",
            ReturnStatus.InTransit => "Your return is in transit to our facility.",
            ReturnStatus.Completed => "Your return has been completed.",
            ReturnStatus.Cancelled => "Your return request has been cancelled.",
            _ => "Return status updated."
        };
    }

    private string GetCustomerPhoneNumber(string customerId)
    {
        // In a real implementation, this would call the user service to get contact info
        // For now, return a placeholder
        return "+1234567890"; // This should be retrieved from user service
    }
}
