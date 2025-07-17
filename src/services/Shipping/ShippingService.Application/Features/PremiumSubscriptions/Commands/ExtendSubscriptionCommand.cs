using MediatR;

namespace ShippingService.Application.Features.PremiumSubscriptions.Commands;

/// <summary>
/// دستور تمدید اشتراک
/// </summary>
/// <param name="AdditionalDays">روزهای اضافی</param>
/// <param name="PaymentReference">مرجع پرداخت</param>
public record ExtendSubscriptionCommand(
    int AdditionalDays,
    string? PaymentReference = null
) : IRequest<bool>
{
    /// <summary>
    /// شناسه اشتراک
    /// </summary>
    public Guid SubscriptionId { get; set; }
}
