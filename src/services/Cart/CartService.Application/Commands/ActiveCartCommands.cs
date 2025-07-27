using BuildingBlocks.Application.CQRS.Commands;
using MediatR;
using Cart.Application.DTOs;

namespace Cart.Application.Commands;

// Commands for Active Cart
public record AddItemToActiveCartCommand(string CartId, string UserId, string ProductId, int Quantity, string? VariantId) : CommandBase<CartOperationResultDto>;

public record UpdateItemQuantityInActiveCartCommand(string CartId, string UserId, string ProductId, int NewQuantity, string? VariantId) : CommandBase<CartOperationResultDto>;

public record RemoveItemFromActiveCartCommand(string CartId, string UserId, string ProductId, string? VariantId) : CommandBase<CartOperationResultDto>;
public record ClearActiveCartCommand(string CartId, string UserId) : CommandBase<CartOperationResultDto>;
public record MergeCartsCommand(string UserId, string GuestCartId) : CommandBase<CartOperationResultDto>;

public record MergeGuestActiveCartCommand(string UserId, string GuestId) : CommandBase<CartOperationResultDto>;

public record MoveItemToNextPurchaseCommand(string? UserId, string? GuestId, string ProductId, int? Quantity)
    : CommandBase<CartOperationResultDto>;

public record UpdateActiveCartItemQuantityCommand(
    string? UserId,
    string? GuestId,
    string ProductId,
    int Quantity) : CommandBase<CartOperationResultDto>;

