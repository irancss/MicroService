using MediatR;
using Cart.Application.DTOs;
using Cart.Domain.Enums;

namespace Cart.Application.Commands;

public class AddItemToActiveCartCommand : IRequest<CartOperationResult>
{
    public string? UserId { get; set; }
    public string? GuestId { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? VariantId { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new();
}

public class AddItemToNextPurchaseCartCommand : IRequest<CartOperationResult>
{
    public string? UserId { get; set; }
    public string? GuestId { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? VariantId { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new();
}

public class RemoveItemFromCartCommand : IRequest<CartOperationResult>
{
    public string? UserId { get; set; }
    public string? GuestId { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public CartType CartType { get; set; }
}

public class UpdateCartItemQuantityCommand : IRequest<CartOperationResult>
{
    public string? UserId { get; set; }
    public string? GuestId { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public CartType CartType { get; set; }
}

public class MoveItemToNextPurchaseCommand : IRequest<CartOperationResult>
{
    public string? UserId { get; set; }
    public string? GuestId { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public int? Quantity { get; set; } // If null, move all
}

public class MoveItemToActiveCartCommand : IRequest<CartOperationResult>
{
    public string? UserId { get; set; }
    public string? GuestId { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public int? Quantity { get; set; } // If null, move all
}

public class ClearCartCommand : IRequest<CartOperationResult>
{
    public string? UserId { get; set; }
    public string? GuestId { get; set; }
    public CartType CartType { get; set; }
}

public class MergeGuestCartCommand : IRequest<CartOperationResult>
{
    public string UserId { get; set; } = string.Empty;
    public string GuestId { get; set; } = string.Empty;
}

public class ActivateNextPurchaseItemsCommand : IRequest<CartOperationResult>
{
    public string? UserId { get; set; }
    public string? GuestId { get; set; }
    public bool ForceActivation { get; set; } = false;
}
