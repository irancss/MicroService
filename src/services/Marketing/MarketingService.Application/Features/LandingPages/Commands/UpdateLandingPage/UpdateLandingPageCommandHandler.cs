using MarketingService.Application.Common;
using MarketingService.Application.DTOs;
using MarketingService.Domain.Interfaces;

namespace MarketingService.Application.Features.LandingPages.Commands.UpdateLandingPage;

public class UpdateLandingPageCommandHandler : ICommandHandler<UpdateLandingPageCommand, LandingPageDto>
{
    private readonly ILandingPageRepository _landingPageRepository;

    public UpdateLandingPageCommandHandler(ILandingPageRepository landingPageRepository)
    {
        _landingPageRepository = landingPageRepository ?? throw new ArgumentNullException(nameof(landingPageRepository));
    }

    public async Task<LandingPageDto> Handle(UpdateLandingPageCommand request, CancellationToken cancellationToken)
    {
        var landingPage = await _landingPageRepository.GetByIdAsync(request.Id, cancellationToken);
        if (landingPage == null)
        {
            throw new InvalidOperationException($"Landing page with ID '{request.Id}' not found");
        }

        // Update content if provided
        if (!string.IsNullOrEmpty(request.Content))
        {
            landingPage.UpdateContent(request.Content);
        }

        // Update meta data if provided
        if (!string.IsNullOrEmpty(request.MetaDescription) || !string.IsNullOrEmpty(request.MetaKeywords))
        {
            landingPage.UpdateMetaData(
                request.MetaDescription ?? landingPage.MetaDescription,
                request.MetaKeywords ?? landingPage.MetaKeywords);
        }

        // Update custom styling if provided
        if (request.CustomCss != null || request.CustomJs != null)
        {
            landingPage.UpdateCustomStyling(request.CustomCss, request.CustomJs);
        }

        await _landingPageRepository.UpdateAsync(landingPage, cancellationToken);

        return new LandingPageDto(
            landingPage.Id,
            landingPage.Title,
            landingPage.Slug,
            landingPage.Content,
            landingPage.MetaDescription,
            landingPage.MetaKeywords,
            landingPage.Status,
            landingPage.CustomCss,
            landingPage.CustomJs,
            landingPage.CreatedAt,
            landingPage.UpdatedAt);
    }
}
