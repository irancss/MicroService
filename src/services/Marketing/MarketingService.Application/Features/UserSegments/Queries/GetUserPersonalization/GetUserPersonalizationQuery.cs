using MarketingService.Application.Common;
using MarketingService.Application.DTOs;

namespace MarketingService.Application.Features.UserSegments.Queries.GetUserPersonalization;

public record GetUserPersonalizationQuery(Guid UserId) : IQuery<UserPersonalizationDto>;
