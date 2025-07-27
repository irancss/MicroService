using BuildingBlocks.Messaging;
using Cart.Application.Interfaces;
using Cart.Domain.Entities;

namespace Cart.API.Infrastructure.Services
{
    // این یک رویداد یکپارچه‌سازی است که برای سرویس Notification ارسال می‌شود.
    // این باید در یک پروژه مشترک (Contracts) تعریف شود.
    public record SendNotificationCommand(string UserId, string Message, string Channel);

    public class NotificationService : INotificationService
    {
        private readonly IMessageBus _messageBus; // برای ارسال Command به یک صف مشخص

        public NotificationService(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        public Task SendCartAbandonmentEmailAsync(string userId, ShoppingCart cart, int notificationNumber)
        {
            var message = $"Email #{notificationNumber}: Hey! You left {cart.TotalItems} items in your cart. Come back and complete your purchase!";
            var command = new SendNotificationCommand(userId, message, "Email");
            return _messageBus.SendAsync(command);
        }

        public Task SendCartAbandonmentSmsAsync(string userId, ShoppingCart cart, int notificationNumber)
        {
            var message = $"SMS #{notificationNumber}: You have items waiting in your cart!";
            var command = new SendNotificationCommand(userId, message, "Sms");
            return _messageBus.SendAsync(command);
        }

        public Task SendNextPurchaseActivatedNotificationAsync(string userId, int itemsCount)
        {
            // این می‌تواند یک Push Notification باشد
            var message = $"We've moved {itemsCount} of your saved items to your active cart!";
            var command = new SendNotificationCommand(userId, message, "Push");
            return _messageBus.SendAsync(command);
        }
    }
}
