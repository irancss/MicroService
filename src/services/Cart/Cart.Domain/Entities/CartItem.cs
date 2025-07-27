using BuildingBlocks.Domain.ValueObjects;

namespace Cart.Domain.Entities;

// CartItem یک ValueObject است زیرا هویت مستقلی ندارد و بر اساس مقادیرش تعریف می‌شود.
public class CartItem : ValueObject
{
    public string ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public decimal PriceAtTimeOfAddition { get; private set; }
    public string? ProductImageUrl { get; private set; }
    public string? VariantId { get; private set; }
    public DateTime AddedUtc { get; private set; }
    public decimal TotalPrice => PriceAtTimeOfAddition * Quantity;

    private CartItem() { } // For serialization

    public static CartItem Create(string productId, string productName, int quantity, decimal price, string? imageUrl = null, string? variantId = null)
    {
        if (string.IsNullOrWhiteSpace(productId)) throw new ArgumentNullException(nameof(productId));
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        if (price < 0) throw new ArgumentException("Price cannot be negative.", nameof(price));

        return new CartItem
        {
            ProductId = productId,
            ProductName = productName,
            Quantity = quantity,
            PriceAtTimeOfAddition = price,
            ProductImageUrl = imageUrl,
            VariantId = variantId,
            AddedUtc = DateTime.UtcNow
        };
    }

    public CartItem WithIncreasedQuantity(int quantityToAdd)
    {
        var newQuantity = Quantity + quantityToAdd;
        return Create(ProductId, ProductName, newQuantity, PriceAtTimeOfAddition, ProductImageUrl, VariantId);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ProductId;
        yield return VariantId ?? string.Empty;
    }
}