using BuildingBlocks.Domain.Entities;
using Cart.Domain.Events;

namespace Cart.Domain.Entities
{
    public class ActiveCart : BaseEntity<string>, IAggregateRoot
    {
        public string? UserId { get; private set; }
        public List<CartItem> Items { get; private set; } = new();
        public DateTime CreatedUtc { get; private set; }
        public DateTime LastModifiedUtc { get; private set; }

        private ActiveCart() { } // For persistence

        public static ActiveCart Create(string cartId, string? userId = null)
        {
            if (string.IsNullOrWhiteSpace(cartId))
                throw new ArgumentNullException(nameof(cartId));

            var cart = new ActiveCart
            {
                Id = cartId, // CartId can be UserId for logged-in users or a Guid for guests
                UserId = userId,
                CreatedUtc = DateTime.UtcNow,
                LastModifiedUtc = DateTime.UtcNow
            };

            // Note: Domain events are handled within the aggregate's methods.
            return cart;
        }

        public void AddItem(CartItem newItem)
        {
            var existingItem = Items.FirstOrDefault(i => i.Equals(newItem));

            if (existingItem != null)
            {
                var updatedItem = existingItem.WithIncreasedQuantity(newItem.Quantity);
                Items.Remove(existingItem);
                Items.Add(updatedItem);
            }
            else
            {
                Items.Add(newItem);
            }

            Touch();
        }

        public CartItem? FindItem(string productId, string? variantId)
        {
            var itemToFind = CartItem.Create(productId, "temp", 1, 0, variantId: variantId);
            return Items.FirstOrDefault(i => i.Equals(itemToFind));
        }

        public void RemoveItem(CartItem item)
        {
            if (Items.Remove(item))
            {
                Touch();
            }
        }

        public void Clear()
        {
            if (Items.Any())
            {
                Items.Clear();
                Touch();
                AddDomainEvent(new ActiveCartClearedEvent(Id, UserId));
            }
        }

        public void MergeItemsFrom(ActiveCart guestCart)
        {
            foreach (var guestItem in guestCart.Items)
            {
                AddItem(guestItem);
            }
            AddDomainEvent(new CartsMergedEvent(Id, UserId!, guestCart.Id));
        }

        public decimal TotalPrice => Items.Sum(i => i.TotalPrice);
        public int TotalItems => Items.Sum(i => i.Quantity);

        private void Touch() => LastModifiedUtc = DateTime.UtcNow;
    }
}
