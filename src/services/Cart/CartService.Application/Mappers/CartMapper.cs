using Cart.Application.DTOs;
using Cart.Domain.Entities;

namespace Cart.Application.Mappers
{
    public static class CartMapper
    {
        // Mapper for Active Cart
        public static CartDto ToDto(this ActiveCart cart) =>
            new(cart.Id, cart.UserId, cart.TotalItems, cart.TotalPrice, cart.LastModifiedUtc,
                cart.Items.Select(i => i.ToDto()).ToList());

        public static CartItemDto ToDto(this CartItem item) =>
            new(item.ProductId, item.ProductName, item.Quantity, item.PriceAtTimeOfAddition,
                item.TotalPrice, item.ProductImageUrl, item.VariantId);

        // Mapper for Next-Purchase Cart
        public static NextPurchaseCartDto ToDto(this NextPurchaseCart cart) =>
            new(cart.Id, cart.Items.Count,
                cart.Items.Select(i => i.ToDto()).ToList());

        public static NextPurchaseItemDto ToDto(this NextPurchaseItem item) =>
            new(item.ProductId, item.ProductName, item.SavedPrice,
                item.ProductImageUrl, item.VariantId, item.SavedAtUtc);
    }
}
