using BuildingBlocks.Domain.Entities;
using Cart.Domain.Events;

namespace Cart.Domain.Entities;

// ShoppingCart اکنون یک Aggregate Root است و از BaseEntity ارث‌بری می‌کند
public class ShoppingCart : BaseEntity<string>, IAggregateRoot
{
    public string UserId { get; private set; }
    public List<CartItem> Items { get; private set; } = new();
    public DateTime CreatedUtc { get; private set; }
    public DateTime LastModifiedUtc { get; private set; }

    private ShoppingCart() { } // For serialization

    public static ShoppingCart Create(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));

        var cart = new ShoppingCart
        {
            Id = userId, // The cart ID is the same as the User ID
            UserId = userId,
            CreatedUtc = DateTime.UtcNow,
            LastModifiedUtc = DateTime.UtcNow
        };

        cart.AddDomainEvent(new ActiveCartCreatedEvent(cart.Id, cart.UserId));
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

        LastModifiedUtc = DateTime.UtcNow;
        AddDomainEvent(new ItemAddedToActiveCartEvent(this.Id, this.UserId, newItem.ProductId, newItem.VariantId,newItem.Quantity, newItem.PriceAtTimeOfAddition));
    }

    public void RemoveItem(string productId, string? variantId)
    {
        // Create a temporary CartItem to find the one to remove using ValueObject equality
        var itemToRemove = CartItem.Create(productId, "temp", 1, 0, variantId: variantId);

        var foundItem = Items.FirstOrDefault(i => i.Equals(itemToRemove));

        if (foundItem != null)
        {
            Items.Remove(foundItem);
            LastModifiedUtc = DateTime.UtcNow;
            AddDomainEvent(new ItemRemovedFromActiveCartEvent(this.Id, this.UserId, productId, foundItem.VariantId, foundItem.Quantity));
        }
    }

    public void Clear()
    {
        var itemCount = Items.Count;
        if (itemCount > 0)
        {
            Items.Clear();
            LastModifiedUtc = DateTime.UtcNow;
            AddDomainEvent(new ActiveCartClearedEvent(this.Id, this.UserId ));
        }
    }

    public void MergeWith(ShoppingCart guestCart)
    {
        foreach (var guestItem in guestCart.Items)
        {
            this.AddItem(guestItem);
        }
        LastModifiedUtc = DateTime.UtcNow;
        AddDomainEvent(new CartsMergedEvent(this.Id, this.UserId, guestCart.Id));
    }

    public decimal TotalPrice => Items.Sum(i => i.TotalPrice);
    public int TotalItems => Items.Sum(i => i.Quantity);
}