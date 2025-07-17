using MediatR;
using Microsoft.Extensions.Logging;
using SearchService.Application.Services;
using SearchService.Domain.Interfaces;
using System.Diagnostics;

namespace SearchService.Application.Queries.ProductSearch;

public class ProductSearchQueryHandler : IRequestHandler<ProductSearchQuery, ProductSearchResponse>
{
    private readonly IElasticsearchQueryService _elasticsearchQueryService;
    private readonly IUserPersonalizationService _personalizationService;
    private readonly ILogger<ProductSearchQueryHandler> _logger;

    public ProductSearchQueryHandler(
        IElasticsearchQueryService elasticsearchQueryService,
        IUserPersonalizationService personalizationService,
        ILogger<ProductSearchQueryHandler> logger)
    {
        _elasticsearchQueryService = elasticsearchQueryService;
        _personalizationService = personalizationService;
        _logger = logger;
    }

    public async Task<ProductSearchResponse> Handle(ProductSearchQuery request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Processing product search query: {Query}, User: {UserId}", 
                request.Query, request.UserId);

            // Get user personalization data if personalization is enabled and user is provided
            var personalizationData = await GetPersonalizationDataAsync(request, cancellationToken);

            // Execute the search with personalization
            var searchResult = await _elasticsearchQueryService.SearchProductsAsync(request, personalizationData, cancellationToken);

            stopwatch.Stop();
            searchResult.QueryTimeMs = stopwatch.Elapsed.TotalMilliseconds;
            searchResult.HasPersonalization = personalizationData != null;

            _logger.LogInformation("Search completed in {ElapsedMs}ms, found {TotalResults} results", 
                searchResult.QueryTimeMs, searchResult.TotalResults);

            return searchResult;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error processing product search query: {Query}", request.Query);
            
            return new ProductSearchResponse
            {
                Products = new List<Domain.Entities.ProductDocument>(),
                Pagination = new Domain.ValueObjects.PaginationInfo
                {
                    Page = request.Page,
                    Size = request.Size,
                    TotalItems = 0
                },
                QueryTimeMs = stopwatch.Elapsed.TotalMilliseconds
            };
        }
    }

    private async Task<Domain.Entities.UserPersonalizationData?> GetPersonalizationDataAsync(
        ProductSearchQuery request, 
        CancellationToken cancellationToken)
    {
        if (!request.EnablePersonalization || string.IsNullOrEmpty(request.UserId))
            return null;

        try
        {
            return await _personalizationService.GetUserPersonalizationAsync(request.UserId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to retrieve personalization data for user: {UserId}", request.UserId);
            return null;
        }
    }
}
