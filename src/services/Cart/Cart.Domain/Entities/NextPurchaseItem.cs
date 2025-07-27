using BuildingBlocks.Domain.ValueObjects;

namespace Cart.Domain.Entities
{
    public class NextPurchaseItem : ValueObject
    {
        public string ProductId { get; private set; }
        public string ProductName { get; private set; }
        public decimal SavedPrice { get; private set; }
        public string? ProductImageUrl { get; private set; }
        public string? VariantId { get; private set; }
        public DateTime SavedAtUtc { get; private set; }

        private NextPurchaseItem() { } // For EF Core

        public static NextPurchaseItem Create(string productId, string productName, decimal savedPrice, string? imageUrl = null, string? variantId = null)
        {
            return new NextPurchaseItem
            {
                ProductId = productId,
                ProductName = productName,
                SavedPrice = savedPrice,
                ProductImageUrl = imageUrl,
                VariantId = variantId,
                SavedAtUtc = DateTime.UtcNow
            };
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ProductId;
            yield return VariantId ?? string.Empty;
        }
    }
}
