using System.ComponentModel.DataAnnotations;

namespace Cart.Domain.Entities;

public class ShoppingCart
{
    public string Id { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? GuestId { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime LastModifiedUtc { get; set; }
    public List<CartItem> ActiveItems { get; set; } = new();
    public List<CartItem> NextPurchaseItems { get; set; } = new();
    public bool IsAbandoned { get; set; }
    public DateTime? AbandonedUtc { get; set; }
    public int AbandonmentNotificationsSent { get; set; }

    public string GetCartKey()
    {
        return !string.IsNullOrEmpty(UserId) ? $"cart:user:{UserId}" : $"cart:guest:{GuestId}";
    }

    public bool IsEmpty()
    {
        return !ActiveItems.Any() && !NextPurchaseItems.Any();
    }

    public bool HasActiveItems()
    {
        return ActiveItems.Any();
    }

    public bool HasNextPurchaseItems()
    {
        return NextPurchaseItems.Any();
    }

    public decimal GetActiveTotalAmount()
    {
        return ActiveItems.Sum(item => item.PriceAtTimeOfAddition * item.Quantity);
    }

    public decimal GetNextPurchaseTotalAmount()
    {
        return NextPurchaseItems.Sum(item => item.PriceAtTimeOfAddition * item.Quantity);
    }

    public int GetActiveItemsCount()
    {
        return ActiveItems.Sum(item => item.Quantity);
    }

    public int GetNextPurchaseItemsCount()
    {
        return NextPurchaseItems.Sum(item => item.Quantity);
    }

    public void UpdateLastModified()
    {
        LastModifiedUtc = DateTime.UtcNow;
    }

    public void MarkAsAbandoned()
    {
        IsAbandoned = true;
        AbandonedUtc = DateTime.UtcNow;
    }

    public void MarkAsActive()
    {
        IsAbandoned = false;
        AbandonedUtc = null;
    }

    public void IncrementAbandonmentNotifications()
    {
        AbandonmentNotificationsSent++;
    }

    public void MoveAllNextPurchaseToActive()
    {
        if (NextPurchaseItems.Any())
        {
            ActiveItems.AddRange(NextPurchaseItems);
            NextPurchaseItems.Clear();
            UpdateLastModified();
        }
    }

    public void MoveAllActiveToNextPurchase()
    {
        if (ActiveItems.Any())
        {
            NextPurchaseItems.AddRange(ActiveItems);
            ActiveItems.Clear();
            UpdateLastModified();
        }
    }
}
