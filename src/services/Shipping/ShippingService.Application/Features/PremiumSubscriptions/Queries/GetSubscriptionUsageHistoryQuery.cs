using MediatR;
using ShippingService.Application.DTOs;

namespace ShippingService.Application.Features.PremiumSubscriptions.Queries;

/// <summary>
/// کوئری دریافت تاریخچه استفاده از اشتراک
/// </summary>
/// <param name="SubscriptionId">شناسه اشتراک</param>
public record GetSubscriptionUsageHistoryQuery(Guid SubscriptionId) : IRequest<IEnumerable<SubscriptionUsageLogDto>>;
