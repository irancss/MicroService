namespace Cart.Domain.ValueObjects;

public class CartConfiguration
{
    public bool AutoActivateNextPurchaseEnabled { get; set; } = true;
    public int AbandonmentThresholdMinutes { get; set; } = 30;
    public int AbandonmentMoveToNextPurchaseDays { get; set; } = 7;
    public bool AbandonmentEmailEnabled { get; set; } = true;
    public bool AbandonmentSmsEnabled { get; set; } = true;
    public int MaxAbandonmentNotifications { get; set; } = 3;
    public int AbandonmentNotificationIntervalHours { get; set; } = 24;
    public decimal MinimumOrderValueForAbandonment { get; set; } = 10;
    public int MaxCartItemsPerUser { get; set; } = 100;
    public int CartExpirationDays { get; set; } = 30;
    public bool GuestCartMergeEnabled { get; set; } = true;
    public bool RealTimeStockValidationEnabled { get; set; } = true;
    public bool RealTimePriceValidationEnabled { get; set; } = true;
}
