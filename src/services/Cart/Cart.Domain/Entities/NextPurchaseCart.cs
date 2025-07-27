using BuildingBlocks.Domain.Entities;
using Cart.Domain.Events;

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
            var existingItem = FindItem(newItem.ProductId, newItem.VariantId);
            if (existingItem is null)
            {
                Items.Add(newItem);
                AddDomainEvent(new ItemAddedToNextPurchaseCartEvent(Id, newItem.ProductId, newItem.VariantId, newItem.SavedPrice));
            }
        }

        public NextPurchaseItem? FindItem(string productId, string? variantId)
        {
            var itemToFind = NextPurchaseItem.Create(productId, "temp", 0, variantId: variantId);
            return Items.FirstOrDefault(i => i.Equals(itemToFind));
        }

        public void RemoveItem(NextPurchaseItem itemToRemove)
        {
            if(Items.Remove(itemToRemove))
            {
                AddDomainEvent(new ItemRemovedFromNextPurchaseCartEvent(Id, itemToRemove.ProductId, itemToRemove.VariantId));
            }
        }
    }
}
