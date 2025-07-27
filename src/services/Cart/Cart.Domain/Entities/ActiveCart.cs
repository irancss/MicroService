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
                Id = cartId,
                UserId = userId,
                CreatedUtc = DateTime.UtcNow,
                LastModifiedUtc = DateTime.UtcNow
            };
            
            // تولید رویداد دامنه برای ایجاد سبد
            cart.AddDomainEvent(new ActiveCartCreatedEvent(cart.Id, cart.UserId));
            return cart;
        }

        public void AddItem(CartItem newItem)
        {
           var existingItem = FindItem(newItem.ProductId, newItem.VariantId);

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
            AddDomainEvent(new ItemAddedToActiveCartEvent(Id, UserId, newItem.ProductId, newItem.VariantId, newItem.Quantity, newItem.PriceAtTimeOfAddition));
        }

         public void UpdateItemQuantity(string productId, string? variantId, int newQuantity)
        {
            var itemToUpdate = FindItem(productId, variantId);
            if (itemToUpdate is null)
            {
                // آیتم وجود ندارد، کاری انجام نمی‌دهیم یا می‌توان خطا برگرداند.
                return;
            }

            if (newQuantity <= 0)
            {
                RemoveItem(itemToUpdate);
                return;
            }

            var updatedItem = CartItem.Create(
                itemToUpdate.ProductId, 
                itemToUpdate.ProductName, 
                newQuantity, 
                itemToUpdate.PriceAtTimeOfAddition, 
                itemToUpdate.ProductImageUrl, 
                itemToUpdate.VariantId);

            Items.Remove(itemToUpdate);
            Items.Add(updatedItem);

            Touch();
            AddDomainEvent(new ItemQuantityUpdatedInActiveCartEvent(Id, UserId, productId, variantId, newQuantity, itemToUpdate.Quantity));
        }

       

        public void RemoveItem(CartItem itemToRemove)
        {
            if (Items.Remove(itemToRemove))
            {
                Touch();
                AddDomainEvent(new ItemRemovedFromActiveCartEvent(Id, UserId, itemToRemove.ProductId, itemToRemove.VariantId, itemToRemove.Quantity));
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

         public CartItem? FindItem(string productId, string? variantId)
        {
            var itemToFind = CartItem.Create(productId, "temp", 1, 0, variantId: variantId);
            return Items.FirstOrDefault(i => i.Equals(itemToFind));
        }

        public decimal TotalPrice => Items.Sum(i => i.TotalPrice);
        public int TotalItems => Items.Sum(i => i.Quantity);

        private void Touch() => LastModifiedUtc = DateTime.UtcNow;
    }
}
