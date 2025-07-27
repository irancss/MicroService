
namespace Cart.Application.DTOs;

//public class CartDto
//{
//    public string Id { get; set; } = string.Empty;
//    public string? UserId { get; set; }
//    public string? GuestId { get; set; }
//    public DateTime CreatedUtc { get; set; }
//    public DateTime LastModifiedUtc { get; set; }
//    public List<CartItemDto> ActiveItems { get; set; } = new();
//    public List<CartItemDto> NextPurchaseItems { get; set; } = new();
//    public decimal ActiveTotalAmount { get; set; }
//    public decimal NextPurchaseTotalAmount { get; set; }
//    public int ActiveItemsCount { get; set; }
//    public int NextPurchaseItemsCount { get; set; }
//    public bool NextPurchaseItemsActivated { get; set; }
//    public string? ActivationMessage { get; set; }
//}
// فقط یک تعریف از CartDto وجود دارد
public record CartDto(
    string Id,
    string? UserId,
    int TotalItems,
    decimal TotalPrice,
    DateTime LastModifiedUtc,
    List<CartItemDto> Items
)
{
    // متد استاتیک را به داخل بدنه رکورد منتقل می‌کنیم
    public static CartDto CreateEmpty(string cartId, string? userId)
    {
        return new CartDto(cartId, userId, 0, 0m, DateTime.UtcNow, new List<CartItemDto>());
    }
}

public record CartItemDto(
    string ProductId,
    string ProductName,
    int Quantity,
    decimal Price,
    decimal TotalPrice,
    string? ProductImageUrl,
    string? VariantId
);

public record CartOperationResultDto(
    bool IsSuccess,
    string? ErrorMessage,
    CartDto? Cart
);

public record NextPurchaseCartDto(
    string UserId,
    int TotalItems,
    List<NextPurchaseItemDto> Items
);

public record NextPurchaseItemDto(
    string ProductId,
    string ProductName,
    decimal SavedPrice,
    string? ProductImageUrl,
    string? VariantId,
    DateTime SavedAtUtc
);

//public class CartItemDto
//{
//    public string ProductId { get; set; } = string.Empty;
//    public string ProductName { get; set; } = string.Empty;
//    public string? ProductImageUrl { get; set; }
//    public int Quantity { get; set; }
//    public decimal PriceAtTimeOfAddition { get; set; }
//    public decimal CurrentPrice { get; set; }
//    public bool PriceChanged { get; set; }
//    public bool InStock { get; set; }
//    public DateTime AddedUtc { get; set; }
//    public DateTime LastUpdatedUtc { get; set; }
//    public string? VariantId { get; set; }
//    public Dictionary<string, string> Attributes { get; set; } = new();
//    public decimal TotalPrice => PriceAtTimeOfAddition * Quantity;
//}


//public class AddItemToCartDto
//{
//    public string ProductId { get; set; } = string.Empty;
//    public int Quantity { get; set; }
//    public string? VariantId { get; set; }
//    public Dictionary<string, string> Attributes { get; set; } = new();
//    public CartType CartType { get; set; } = CartType.Active;
//}

//public class UpdateCartItemDto
//{
//    public string ProductId { get; set; } = string.Empty;
//    public int Quantity { get; set; }
//    public CartType CartType { get; set; }
//}

//public class MoveItemDto
//{
//    public string ProductId { get; set; } = string.Empty;
//    public int? Quantity { get; set; } // If null, move all quantity
//    public CartType SourceCartType { get; set; }
//    public CartType DestinationCartType { get; set; }
//}

//public class CartOperationResult
//{
//    public bool Success { get; set; }
//    public string? ErrorMessage { get; set; }
//    public CartDto? Cart { get; set; }
//    public List<string> Warnings { get; set; } = new();
//    public bool NextPurchaseItemsActivated { get; set; }
//    public string? ActivationMessage { get; set; }

//    public static CartOperationResult SuccessResult(CartDto cart, bool nextPurchaseActivated = false, string? activationMessage = null)
//    {
//        return new CartOperationResult
//        {
//            Success = true,
//            Cart = cart,
//            NextPurchaseItemsActivated = nextPurchaseActivated,
//            ActivationMessage = activationMessage
//        };
//    }

//    public static CartOperationResult ErrorResult(string errorMessage)
//    {
//        return new CartOperationResult
//        {
//            Success = false,
//            ErrorMessage = errorMessage
//        };
//    }
//}
