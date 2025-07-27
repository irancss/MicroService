using Cart.Domain.Entities;

namespace Cart.Application.Interfaces;

public interface INotificationService
{
    Task SendCartAbandonmentEmailAsync(string userId, ActiveCart cart, int notificationNumber);
    Task SendCartAbandonmentSmsAsync(string userId, ActiveCart cart, int notificationNumber);
    Task SendNextPurchaseActivatedNotificationAsync(string userId, int itemsCount);
}