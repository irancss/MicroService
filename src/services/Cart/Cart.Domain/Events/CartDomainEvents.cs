using Cart.Domain.Enums;

namespace Cart.Domain.Events;

public abstract class CartDomainEvent
{
    public string CartId { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? GuestId { get; set; }
    public DateTime OccurredUtc { get; set; } = DateTime.UtcNow;
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public CartEventType EventType { get; set; }
}

public class ItemAddedToCartEvent : CartDomainEvent
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public CartType CartType { get; set; }

    public ItemAddedToCartEvent()
    {
        EventType = CartEventType.ItemAdded;
    }
}

public class ItemRemovedFromCartEvent : CartDomainEvent
{
    public string ProductId { get; set; } = string.Empty;
    public int RemovedQuantity { get; set; }
    public CartType CartType { get; set; }

    public ItemRemovedFromCartEvent()
    {
        EventType = CartEventType.ItemRemoved;
    }
}

public class ItemMovedBetweenCartsEvent : CartDomainEvent
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public CartType SourceCartType { get; set; }
    public CartType DestinationCartType { get; set; }

    public ItemMovedBetweenCartsEvent()
    {
        EventType = CartEventType.ItemMoved;
    }
}

public class CartAbandonedEvent : CartDomainEvent
{
    public decimal TotalValue { get; set; }
    public int ItemCount { get; set; }
    public List<string> ProductIds { get; set; } = new();

    public CartAbandonedEvent()
    {
        EventType = CartEventType.CartAbandoned;
    }
}

public class NextPurchaseActivatedEvent : CartDomainEvent
{
    public int ItemsMovedCount { get; set; }
    public decimal TotalValue { get; set; }
    public List<string> ProductIds { get; set; } = new();

    public NextPurchaseActivatedEvent()
    {
        EventType = CartEventType.NextPurchaseActivated;
    }
}
public class CartClearedEvent : CartDomainEvent
{
    public CartType CartType { get; set; }

    public CartClearedEvent()
    {
        EventType = CartEventType.CartCleared;
    }
}
public class CartMergedEvent : CartDomainEvent
{
    public string GuestCartId { get; set; } = string.Empty;
    public string UserCartId { get; set; } = string.Empty;
    public int MergedItemsCount { get; set; }

    public CartMergedEvent()
    {
        EventType = CartEventType.CartMerged;
    }
}
