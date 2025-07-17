namespace Cart.Domain.Entities;

public class CartItem
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal PriceAtTimeOfAddition { get; set; }
    public DateTime AddedUtc { get; set; }
    public DateTime LastUpdatedUtc { get; set; }
    public string? VariantId { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new();

    public decimal TotalPrice => PriceAtTimeOfAddition * Quantity;

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        Quantity = newQuantity;
        LastUpdatedUtc = DateTime.UtcNow;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentException("Price cannot be negative");

        PriceAtTimeOfAddition = newPrice;
        LastUpdatedUtc = DateTime.UtcNow;
    }

    public CartItem Clone()
    {
        return new CartItem
        {
            ProductId = ProductId,
            ProductName = ProductName,
            ProductImageUrl = ProductImageUrl,
            Quantity = Quantity,
            PriceAtTimeOfAddition = PriceAtTimeOfAddition,
            AddedUtc = AddedUtc,
            LastUpdatedUtc = LastUpdatedUtc,
            VariantId = VariantId,
            Attributes = new Dictionary<string, string>(Attributes)
        };
    }
}
