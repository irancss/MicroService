using BuildingBlocks.Domain.Events;

namespace Cart.Domain.Events;



// =================================================================
// Events for ActiveCart Aggregate
// =================================================================

/// <summary>
/// Published when a new active cart is created for a user or a guest.
/// </summary>
public record ActiveCartCreatedEvent(
    string CartId,
    string? UserId) : DomainEvent;

/// <summary>
/// Published when an item is added to an active cart.
/// </summary>
public record ItemAddedToActiveCartEvent(
    string CartId,
    string? UserId,
    string ProductId,
    string? VariantId,
    int Quantity,
    decimal Price) : DomainEvent;

/// <summary>
/// Published when an item is removed from an active cart.
/// </summary>
public record ItemRemovedFromActiveCartEvent(
    string CartId,
    string? UserId,
    string ProductId,
    string? VariantId,
    int RemovedQuantity) : DomainEvent;



/// <summary>
/// Published when an active cart is completely emptied.
/// </summary>
public record ActiveCartClearedEvent(
    string CartId,
    string? UserId
    ) : DomainEvent;






/// <summary>
/// Published when a guest's active cart is merged into a logged-in user's cart.
/// </summary>
public record CartsMergedEvent(
    string UserCartId, // The cart that received the items
    string UserId,
    string GuestCartId) : DomainEvent; // The cart that was merged and will be deleted

/// <summary>
/// Published when an active cart is considered abandoned and its stock reservation should be released.
/// </summary>
public record ActiveCartAbandonedEvent(
    string CartId,
    string? UserId) : DomainEvent;

// =================================================================
// Events for NextPurchaseCart Aggregate
// =================================================================

/// <summary>
/// Published when a new next-purchase cart is created for a user.
/// </summary>
public record NextPurchaseCartCreatedEvent(
    string UserId) : DomainEvent;

/// <summary>
/// Published when an item is added to the next-purchase cart.
/// </summary>
public record ItemAddedToNextPurchaseCartEvent(
    string UserId,
    string ProductId,
    string? VariantId,
    decimal SavedPrice) : DomainEvent;

/// <summary>
/// Published when an item is removed from the next-purchase cart.
/// </summary>
public record ItemRemovedFromNextPurchaseCartEvent(
    string UserId,
    string ProductId,
    string? VariantId) : DomainEvent;


// =================================================================
// Events representing interaction BETWEEN the two aggregates
// These are typically raised by command handlers, not the aggregates themselves.
// =================================================================

/// <summary>
/// Published when an item is moved from the active cart to the next-purchase cart.
/// </summary>
public record ItemSavedForLaterEvent(
    string UserId,
    string ProductId,
    string? VariantId) : DomainEvent;

/// <summary>
/// Published when an item is moved from the next-purchase cart to the active cart.
/// </summary>
public record ItemMovedToActiveCartEvent(
    string UserId,
    string ProductId,
    string? VariantId) : DomainEvent;