using SearchService.Application.Queries.ProductSearch;
using SearchService.Application.Queries.SearchSuggestions;
using SearchService.Domain.Entities;

namespace SearchService.Application.Services;

/// <summary>
/// Service interface for complex Elasticsearch query operations
/// </summary>
public interface IElasticsearchQueryService
{
    /// <summary>
    /// Execute a complex product search with faceting, sorting, and personalization
    /// </summary>
    Task<ProductSearchResponse> SearchProductsAsync(
        ProductSearchQuery query, 
        UserPersonalizationData? personalizationData, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get search suggestions and auto-complete
    /// </summary>
    Task<GetSearchSuggestionsResponse> GetSuggestionsAsync(
        GetSearchSuggestionsQuery query, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get spell correction suggestions
    /// </summary>
    Task<List<string>> GetSpellCorrectionSuggestionsAsync(
        string query, 
        CancellationToken cancellationToken = default);
}
