using BuildingBlocks.Application.CQRS.Commands;
using Cart.Application.DTOs;

namespace Cart.Application.Commands;

// Commands for Next Cart
public record SaveItemForLaterCommand(string UserId, string ProductId, string? VariantId) : CommandBase<CartOperationResultDto>;
public record MoveItemToActiveCartCommand(string UserId, string ProductId, string? VariantId) : CommandBase<CartOperationResultDto>;
public record RemoveItemFromNextPurchaseCommand(string UserId, string ProductId, string? VariantId) : CommandBase<NextPurchaseCartDto>;
public record ActivateNextPurchaseItemsCommand(string? UserId, string? GuestId, bool ForceActivation)
    : CommandBase<NextPurchaseCartDto>;

public record AddItemToNextPurchaseCartCommand(
    string? UserId,
    string? GuestId,
    string ProductId,
    int Quantity,
    string? VariantId,
    Dictionary<string, string> Attributes) : CommandBase<NextPurchaseCartDto>;

public record AddItemToNextPurchaseCommand(string UserId, string ProductId, string? VariantId) : CommandBase<NextPurchaseCartDto>;


