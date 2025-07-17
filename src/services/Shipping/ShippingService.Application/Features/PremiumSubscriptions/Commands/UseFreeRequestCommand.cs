using MediatR;

namespace ShippingService.Application.Features.PremiumSubscriptions.Commands;

/// <summary>
/// دستور استفاده از درخواست رایگان
/// </summary>
/// <param name="UserId">شناسه کاربر</param>
/// <param name="ShipmentId">شناسه مرسوله</param>
/// <param name="SavedAmount">مبلغ صرفه‌جویی</param>
/// <param name="Notes">توضیحات</param>
public record UseFreeRequestCommand(
    string UserId,
    Guid ShipmentId,
    decimal SavedAmount,
    string? Notes = null
) : IRequest<bool>;
