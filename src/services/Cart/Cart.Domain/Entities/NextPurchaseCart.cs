using BuildingBlocks.Domain.Entities;

namespace Cart.Domain.Entities
{
    public class NextPurchaseCart : AuditableEntity<string>, IAggregateRoot
    {
        public List<NextPurchaseItem> Items { get; private set; } = new();

        private NextPurchaseCart() { } // For EF Core

        public static NextPurchaseCart Create(string userId)
        {
            return new NextPurchaseCart
            {
                Id = userId, // ID is the UserId
            };
        }

        public void AddItem(NextPurchaseItem newItem)
        {
            var existingItem = Items.FirstOrDefault(i => i.Equals(newItem));
            if (existingItem is null)
            {
                Items.Add(newItem);
            }
            // If it exists, we do nothing. We don't stack quantities here.
        }

        public NextPurchaseItem? FindItem(string productId, string? variantId)
        {
            var itemToFind = NextPurchaseItem.Create(productId, "temp", 0, variantId: variantId);
            return Items.FirstOrDefault(i => i.Equals(itemToFind));
        }

        public void RemoveItem(NextPurchaseItem item)
        {
            Items.Remove(item);
        }
    }
}
