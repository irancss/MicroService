using MediatR;
using Microsoft.AspNetCore.Mvc;
using Cart.Application.Interfaces;
using Cart.Domain.ValueObjects;

namespace Cart.API.Controllers;

[ApiController]
[Route("api/v1/admin/cart-config")]
[Produces("application/json")]
public class CartConfigurationController : ControllerBase
{
    private readonly ICartConfigurationService _configService;
    private readonly ILogger<CartConfigurationController> _logger;

    public CartConfigurationController(
        ICartConfigurationService configService,
        ILogger<CartConfigurationController> logger)
    {
        _configService = configService;
        _logger = logger;
    }

    /// <summary>
    /// Get current cart configuration settings
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<CartConfiguration>> GetConfiguration()
    {
        try
        {
            var config = await _configService.GetConfigurationAsync();
            return Ok(config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart configuration");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Update cart configuration settings
    /// </summary>
    [HttpPut]
    public async Task<ActionResult> UpdateConfiguration([FromBody] CartConfiguration configuration)
    {
        try
        {
            await _configService.UpdateConfigurationAsync(configuration);
            _logger.LogInformation("Cart configuration updated successfully");
            return Ok(new { message = "Configuration updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cart configuration");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Enable or disable automatic next-purchase activation
    /// </summary>
    [HttpPatch("auto-activate-next-purchase")]
    public async Task<ActionResult> UpdateAutoActivateNextPurchase([FromBody] UpdateAutoActivateRequest request)
    {
        try
        {
            var config = await _configService.GetConfigurationAsync();
            config.AutoActivateNextPurchaseEnabled = request.Enabled;
            await _configService.UpdateConfigurationAsync(config);
            
            _logger.LogInformation("Auto activate next purchase setting updated to {Enabled}", request.Enabled);
            return Ok(new { message = "Auto activate setting updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating auto activate next purchase setting");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Update abandonment behavior settings
    /// </summary>
    [HttpPatch("abandonment-settings")]
    public async Task<ActionResult> UpdateAbandonmentSettings([FromBody] UpdateAbandonmentSettingsRequest request)
    {
        try
        {
            var config = await _configService.GetConfigurationAsync();
            
            if (request.ThresholdMinutes.HasValue)
                config.AbandonmentThresholdMinutes = request.ThresholdMinutes.Value;
            
            if (request.EmailEnabled.HasValue)
                config.AbandonmentEmailEnabled = request.EmailEnabled.Value;
            
            if (request.SmsEnabled.HasValue)
                config.AbandonmentSmsEnabled = request.SmsEnabled.Value;
            
            if (request.MaxNotifications.HasValue)
                config.MaxAbandonmentNotifications = request.MaxNotifications.Value;
            
            if (request.NotificationIntervalHours.HasValue)
                config.AbandonmentNotificationIntervalHours = request.NotificationIntervalHours.Value;
            
            if (request.MoveToNextPurchaseDays.HasValue)
                config.AbandonmentMoveToNextPurchaseDays = request.MoveToNextPurchaseDays.Value;

            await _configService.UpdateConfigurationAsync(config);
            
            _logger.LogInformation("Abandonment settings updated successfully");
            return Ok(new { message = "Abandonment settings updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating abandonment settings");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}

// Request DTOs
public class UpdateAutoActivateRequest
{
    public bool Enabled { get; set; }
}

public class UpdateAbandonmentSettingsRequest
{
    public int? ThresholdMinutes { get; set; }
    public bool? EmailEnabled { get; set; }
    public bool? SmsEnabled { get; set; }
    public int? MaxNotifications { get; set; }
    public int? NotificationIntervalHours { get; set; }
    public int? MoveToNextPurchaseDays { get; set; }
}
