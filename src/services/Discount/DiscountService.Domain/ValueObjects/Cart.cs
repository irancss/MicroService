namespace DiscountService.Domain.ValueObjects;

/// <summary>
/// Represents a cart item for discount calculation
/// </summary>
public class CartItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice => UnitPrice * Quantity;

    public CartItem() { }

    public CartItem(Guid productId, string productName, Guid categoryId, string categoryName, decimal unitPrice, int quantity)
    {
        ProductId = productId;
        ProductName = productName;
        CategoryId = categoryId;
        CategoryName = categoryName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}

/// <summary>
/// Represents the complete cart for discount calculation
/// </summary>
public class Cart
{
    public Guid UserId { get; set; }
    public List<CartItem> Items { get; set; } = new();
    public decimal ShippingCost { get; set; }
    public string? CouponCode { get; set; }

    public decimal SubTotal => Items.Sum(item => item.TotalPrice);
    public decimal TotalWithShipping => SubTotal + ShippingCost;

    public bool ContainsProduct(Guid productId) => Items.Any(i => i.ProductId == productId);
    public bool ContainsCategory(Guid categoryId) => Items.Any(i => i.CategoryId == categoryId);
    public int GetProductQuantity(Guid productId) => Items.Where(i => i.ProductId == productId).Sum(i => i.Quantity);
}
