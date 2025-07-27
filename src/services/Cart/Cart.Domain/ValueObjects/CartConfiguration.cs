using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the global configuration for cart behaviors across the system.
/// This object is typically managed by an administrator and stored in a persistent cache like Redis.
/// </summary>
public class CartConfiguration
{
    // =================================================================
    // Active Cart Settings (Primarily for Redis-based carts)
    // =================================================================

    /// <summary>
    /// The duration in minutes after which an inactive active cart is considered expired.
    /// This is the TTL (Time-To-Live) for the cart in Redis.
    /// After this time, a background job will trigger stock release.
    /// </summary>
    [Range(5, 1440)] // 5 minutes to 1 day
    public int ActiveCartExpiryMinutes { get; set; } = 60;

    /// <summary>
    /// The time in minutes to wait after a cart expires before releasing its reserved stock.
    /// This provides a grace period in case the user returns just after expiration.
    /// Example: If ActiveCartExpiryMinutes is 60 and this is 5, stock is released at 65 minutes.
    /// </summary>
    [Range(1, 60)]
    public int StockReleaseGracePeriodMinutes { get; set; } = 5;

    // =================================================================
    // Real-time Validation Settings
    // =================================================================

    /// <summary>
    /// If true, the system will call the Inventory service to check stock availability
    /// every time an item is added or its quantity is updated in the active cart.
    /// </summary>
    public bool RealTimeStockValidationEnabled { get; set; } = true;

    /// <summary>
    /// If true, the system will call the Inventory service to get the latest price
    /// when an item is added to the cart.
    /// </summary>
    public bool RealTimePriceValidationEnabled { get; set; } = true;


    // =================================================================
    // User Experience Settings
    // =================================================================

    /// <summary>
    /// If true, a guest's cart items will be automatically merged into the user's cart upon login.
    /// </summary>
    public bool GuestCartMergeEnabled { get; set; } = true;

    /// <summary>
    /// The maximum number of distinct items a user can have in their active cart.
    /// Prevents abuse and performance issues.
    /// </summary>
    [Range(10, 200)]
    public int MaxDistinctItemsInActiveCart { get; set; } = 50;


    // =================================================================
    // Abandoned Cart Notification Settings (For NextPurchaseCart in PostgreSQL)
    // =================================================================

    /// <summary>
    /// Enables or disables the entire abandoned cart notification feature.
    /// </summary>
    public bool AbandonedCartNotificationsEnabled { get; set; } = true;

    /// <summary>
    /// The duration in hours after the last activity in an active cart before it's moved
    /// to the "Next Purchase" (abandoned) cart in the persistent database.
    /// </summary>
    [Range(1, 72)] // 1 hour to 3 days
    public int AbandonmentThresholdHours { get; set; } = 24;

    /// <summary>
    /// The maximum number of reminder notifications (e.g., email, SMS) to send for an abandoned cart.
    /// </summary>
    [Range(0, 5)]
    public int MaxAbandonmentNotifications { get; set; } = 2;

    /// <summary>
    /// The interval in hours between sending abandonment reminder notifications.
    /// </summary>
    [Range(1, 168)] // 1 hour to 1 week
    public int AbandonmentNotificationIntervalHours { get; set; } = 48;
}