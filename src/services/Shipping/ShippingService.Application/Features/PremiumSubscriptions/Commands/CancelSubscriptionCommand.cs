using MediatR;

namespace ShippingService.Application.Features.PremiumSubscriptions.Commands;

/// <summary>
/// دستور لغو اشتراک
/// </summary>
/// <param name="SubscriptionId">شناسه اشتراک</param>
/// <param name="Reason">دلیل لغو</param>
public record CancelSubscriptionCommand(
    Guid SubscriptionId,
    string? Reason = null
) : IRequest<bool>;
