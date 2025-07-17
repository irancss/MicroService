using MediatR;
using ShippingService.Application.DTOs;

namespace ShippingService.Application.Features.PremiumSubscriptions.Queries;

/// <summary>
/// کوئری دریافت اشتراک فعال کاربر
/// </summary>
/// <param name="UserId">شناسه کاربر</param>
public record GetActiveSubscriptionQuery(string UserId) : IRequest<PremiumSubscriptionDto?>;
