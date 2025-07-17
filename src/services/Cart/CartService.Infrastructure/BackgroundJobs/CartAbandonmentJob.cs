using Hangfire;
using Microsoft.Extensions.Logging;
using Cart.Application.Interfaces;
using Cart.Domain.Events;

namespace Cart.Infrastructure.BackgroundJobs;

public class CartAbandonmentJob
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartConfigurationService _configService;
    private readonly INotificationService _notificationService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<CartAbandonmentJob> _logger;

    public CartAbandonmentJob(
        ICartRepository cartRepository,
        ICartConfigurationService configService,
        INotificationService notificationService,
        IEventPublisher eventPublisher,
        ILogger<CartAbandonmentJob> logger)
    {
        _cartRepository = cartRepository;
        _configService = configService;
        _notificationService = notificationService;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task ProcessAbandonedCartsAsync()
    {
        try
        {
            _logger.LogInformation("Starting abandoned cart processing job");

            var config = await _configService.GetConfigurationAsync();
            var abandonmentThreshold = DateTime.UtcNow.AddMinutes(-config.AbandonmentThresholdMinutes);
            
            var abandonedCarts = await _cartRepository.GetAbandonedCartsAsync(abandonmentThreshold);
            
            _logger.LogInformation("Found {Count} abandoned carts to process", abandonedCarts.Count);

            foreach (var cart in abandonedCarts)
            {
                await ProcessIndividualAbandonedCart(cart, config);
            }

            _logger.LogInformation("Completed abandoned cart processing job");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in abandoned cart processing job");
            throw;
        }
    }

    private async Task ProcessIndividualAbandonedCart(Domain.Entities.ShoppingCart cart, Domain.ValueObjects.CartConfiguration config)
    {
        try
        {
            if (cart.AbandonmentNotificationsSent >= config.MaxAbandonmentNotifications)
            {
                _logger.LogDebug("Max notifications reached for cart {CartId}, skipping", cart.Id);
                return;
            }

            // Check if minimum order value is met
            if (cart.GetActiveTotalAmount() < config.MinimumOrderValueForAbandonment)
            {
                _logger.LogDebug("Cart {CartId} value below minimum threshold, skipping", cart.Id);
                return;
            }

            // Mark as abandoned if not already
            if (!cart.IsAbandoned)
            {
                cart.MarkAsAbandoned();
                
                // Publish abandonment event
                await _eventPublisher.PublishAsync(new CartAbandonedEvent
                {
                    CartId = cart.Id,
                    UserId = cart.UserId,
                    GuestId = cart.GuestId,
                    TotalValue = cart.GetActiveTotalAmount(),
                    ItemCount = cart.GetActiveItemsCount(),
                    ProductIds = cart.ActiveItems.Select(i => i.ProductId).ToList()
                });
            }

            // Send notifications based on configuration
            if (!string.IsNullOrEmpty(cart.UserId))
            {
                var notificationNumber = cart.AbandonmentNotificationsSent + 1;

                if (config.AbandonmentEmailEnabled)
                {
                    await _notificationService.SendCartAbandonmentEmailAsync(cart.UserId, cart, notificationNumber);
                }

                if (config.AbandonmentSmsEnabled)
                {
                    await _notificationService.SendCartAbandonmentSmsAsync(cart.UserId, cart, notificationNumber);
                }

                cart.IncrementAbandonmentNotifications();
            }

            // Save updated cart
            await _cartRepository.SaveAsync(cart);

            // Schedule next notification if not reached max
            if (cart.AbandonmentNotificationsSent < config.MaxAbandonmentNotifications)
            {
                var nextNotificationTime = DateTime.UtcNow.AddHours(config.AbandonmentNotificationIntervalHours);
                BackgroundJob.Schedule(() => ProcessIndividualAbandonedCart(cart, config), nextNotificationTime);
            }

            _logger.LogDebug("Processed abandoned cart {CartId}", cart.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing individual abandoned cart {CartId}", cart.Id);
        }
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task MoveAbandonedCartsToNextPurchaseAsync()
    {
        try
        {
            _logger.LogInformation("Starting move abandoned carts to next purchase job");

            var config = await _configService.GetConfigurationAsync();
            var moveThreshold = DateTime.UtcNow.AddDays(-config.AbandonmentMoveToNextPurchaseDays);
            
            var abandonedCarts = await _cartRepository.GetAbandonedCartsAsync(moveThreshold);
            
            foreach (var cart in abandonedCarts.Where(c => c.IsAbandoned && c.AbandonedUtc <= moveThreshold))
            {
                if (cart.HasActiveItems())
                {
                    _logger.LogInformation("Moving abandoned cart {CartId} items to next purchase", cart.Id);
                    
                    cart.MoveAllActiveToNextPurchase();
                    cart.MarkAsActive(); // Reset abandonment status
                    cart.AbandonmentNotificationsSent = 0;
                    
                    await _cartRepository.SaveAsync(cart);
                    
                    // Send notification to user if available
                    if (!string.IsNullOrEmpty(cart.UserId))
                    {
                        // TODO: Send "items saved for later" notification
                        _logger.LogInformation("Should send 'items saved for later' notification to user {UserId}", cart.UserId);
                    }
                }
            }

            _logger.LogInformation("Completed move abandoned carts to next purchase job");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in move abandoned carts to next purchase job");
            throw;
        }
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task CleanupExpiredCartsAsync()
    {
        try
        {
            _logger.LogInformation("Starting cleanup expired carts job");

            var config = await _configService.GetConfigurationAsync();
            var expirationThreshold = DateTime.UtcNow.AddDays(-config.CartExpirationDays);
            
            var expiredCarts = await _cartRepository.GetExpiredCartsAsync(expirationThreshold);
            
            _logger.LogInformation("Found {Count} expired carts to cleanup", expiredCarts.Count);

            foreach (var cart in expiredCarts)
            {
                await _cartRepository.DeleteAsync(cart.Id);
                _logger.LogDebug("Deleted expired cart {CartId}", cart.Id);
            }

            _logger.LogInformation("Completed cleanup expired carts job");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in cleanup expired carts job");
            throw;
        }
    }
}
