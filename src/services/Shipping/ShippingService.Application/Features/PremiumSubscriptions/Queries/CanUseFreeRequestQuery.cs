using MediatR;

namespace ShippingService.Application.Features.PremiumSubscriptions.Queries;

/// <summary>
/// کوئری بررسی امکان استفاده از درخواست رایگان
/// </summary>
/// <param name="UserId">شناسه کاربر</param>
public record CanUseFreeRequestQuery(string UserId) : IRequest<bool>;
