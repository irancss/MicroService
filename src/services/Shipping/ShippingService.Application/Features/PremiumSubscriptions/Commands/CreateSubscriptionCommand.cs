using MediatR;
using ShippingService.Application.DTOs;

namespace ShippingService.Application.Features.PremiumSubscriptions.Commands;

/// <summary>
/// دستور ایجاد اشتراک جدید
/// </summary>
/// <param name="UserId">شناسه کاربر</param>
/// <param name="SubscriptionType">نوع اشتراک</param>
/// <param name="PaymentReference">مرجع پرداخت</param>
public record CreateSubscriptionCommand(
    string UserId,
    string SubscriptionType,
    string? PaymentReference = null
) : IRequest<PremiumSubscriptionDto>;
