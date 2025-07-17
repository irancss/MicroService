using MarketingService.Application.Common;
using MarketingService.Application.DTOs;

namespace MarketingService.Application.Features.LandingPages.Commands.UpdateLandingPage;

public record UpdateLandingPageCommand(
    Guid Id,
    string? Title,
    string? Content,
    string? MetaDescription,
    string? MetaKeywords,
    string? CustomCss,
    string? CustomJs) : ICommand<LandingPageDto>;
