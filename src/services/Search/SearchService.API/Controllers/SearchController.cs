using MediatR;
using Microsoft.AspNetCore.Mvc;
using SearchService.Application.Queries.ProductSearch;
using SearchService.Application.Queries.SearchSuggestions;
using SearchService.Domain.Enums;
using SearchService.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace SearchService.API.Controllers;

/// <summary>
/// Main search controller providing product search, filtering, and suggestions
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SearchController> _logger;

    public SearchController(IMediator mediator, ILogger<SearchController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Search for products with advanced filtering, sorting, and personalization
    /// </summary>
    /// <param name="query">Search query text (optional)</param>
    /// <param name="userId">User ID for personalization (optional)</param>
    /// <param name="category">Category filter (optional)</param>
    /// <param name="brand">Brand filter (optional)</param>
    /// <param name="minPrice">Minimum price filter (optional)</param>
    /// <param name="maxPrice">Maximum price filter (optional)</param>
    /// <param name="minRating">Minimum rating filter (optional)</param>
    /// <param name="maxRating">Maximum rating filter (optional)</param>
    /// <param name="isAvailable">Availability filter (optional)</param>
    /// <param name="tags">Comma-separated tags filter (optional)</param>
    /// <param name="sortBy">Sort option</param>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="size">Page size (1-100)</param>
    /// <param name="includeFacets">Include facets in response</param>
    /// <param name="includeSuggestions">Include suggestions in response</param>
    /// <param name="enablePersonalization">Enable personalization boosting</param>
    /// <returns>Search results with products, facets, and metadata</returns>
    [HttpGet("products")]
    [ProducesResponseType(typeof(ProductSearchResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ProductSearchResponse>> SearchProducts(
        [FromQuery] string? query = null,
        [FromQuery] string? userId = null,
        [FromQuery] string? category = null,
        [FromQuery] string? brand = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] double? minRating = null,
        [FromQuery] double? maxRating = null,
        [FromQuery] bool? isAvailable = null,
        [FromQuery] string? tags = null,
        [FromQuery] SortBy sortBy = SortBy.Relevance,
        [FromQuery, Range(1, int.MaxValue)] int page = 1,
        [FromQuery, Range(1, 100)] int size = 20,
        [FromQuery] bool includeFacets = true,
        [FromQuery] bool includeSuggestions = true,
        [FromQuery] bool enablePersonalization = true)
    {
        try
        {
            var searchQuery = new ProductSearchQuery
            {
                Query = query,
                UserId = userId,
                Category = category,
                Brand = brand,
                PriceRange = CreatePriceRange(minPrice, maxPrice),
                RatingFilter = CreateRatingFilter(minRating, maxRating),
                IsAvailable = isAvailable,
                Tags = ParseTags(tags),
                SortBy = sortBy,
                Page = page,
                Size = size,
                IncludeFacets = includeFacets,
                IncludeSuggestions = includeSuggestions,
                EnablePersonalization = enablePersonalization
            };

            // Parse dynamic attribute filters from query string
            ParseAttributeFilters(searchQuery);

            var result = await _mediator.Send(searchQuery);

            _logger.LogInformation("Search completed: Query='{Query}', Results={ResultCount}, Time={TimeMs}ms",
                query ?? "[browse]", result.TotalResults, result.QueryTimeMs);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing search request");
            return StatusCode(500, new { error = "Internal server error occurred while processing search request" });
        }
    }

    /// <summary>
    /// Get auto-complete suggestions for search queries
    /// </summary>
    /// <param name="query">Partial query text</param>
    /// <param name="maxSuggestions">Maximum number of suggestions to return</param>
    /// <param name="includeProducts">Include product suggestions</param>
    /// <param name="includeCategories">Include category suggestions</param>
    /// <param name="includeBrands">Include brand suggestions</param>
    /// <param name="userId">User ID for personalized suggestions (optional)</param>
    /// <returns>List of search suggestions</returns>
    [HttpGet("suggestions")]
    [ProducesResponseType(typeof(GetSearchSuggestionsResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<GetSearchSuggestionsResponse>> GetSuggestions(
        [FromQuery, Required] string query,
        [FromQuery, Range(1, 50)] int maxSuggestions = 10,
        [FromQuery] bool includeProducts = true,
        [FromQuery] bool includeCategories = true,
        [FromQuery] bool includeBrands = true,
        [FromQuery] string? userId = null)
    {
        try
        {
            var suggestionsQuery = new GetSearchSuggestionsQuery
            {
                Query = query,
                MaxSuggestions = maxSuggestions,
                IncludeProducts = includeProducts,
                IncludeCategories = includeCategories,
                IncludeBrands = includeBrands,
                UserId = userId
            };

            var result = await _mediator.Send(suggestionsQuery);

            _logger.LogInformation("Suggestions completed: Query='{Query}', Results={ResultCount}, Time={TimeMs}ms",
                query, result.Suggestions.Count, result.QueryTimeMs);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing suggestions request");
            return StatusCode(500, new { error = "Internal server error occurred while processing suggestions request" });
        }
    }

    /// <summary>
    /// Get available facet values for dynamic filtering
    /// </summary>
    /// <param name="facetName">Name of the facet (category, brand, etc.)</param>
    /// <param name="search">Optional search term to filter facet values</param>
    /// <param name="limit">Maximum number of facet values to return</param>
    /// <returns>List of available facet values</returns>
    [HttpGet("facets/{facetName}")]
    [ProducesResponseType(typeof(IEnumerable<FacetOption>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<FacetOption>>> GetFacetValues(
        [FromRoute] string facetName,
        [FromQuery] string? search = null,
        [FromQuery, Range(1, 100)] int limit = 20)
    {
        try
        {
            // This would typically query Elasticsearch for unique facet values
            // For now, return a placeholder response
            var facetOptions = new List<FacetOption>
            {
                new() { Value = "sample1", DisplayValue = "Sample Value 1", Count = 10 },
                new() { Value = "sample2", DisplayValue = "Sample Value 2", Count = 5 }
            };

            return Ok(facetOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting facet values for: {FacetName}", facetName);
            return StatusCode(500, new { error = "Internal server error occurred while retrieving facet values" });
        }
    }

    private PriceRange? CreatePriceRange(decimal? minPrice, decimal? maxPrice)
    {
        if (minPrice.HasValue || maxPrice.HasValue)
        {
            return new PriceRange
            {
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };
        }
        return null;
    }

    private RatingFilter? CreateRatingFilter(double? minRating, double? maxRating)
    {
        if (minRating.HasValue || maxRating.HasValue)
        {
            return new RatingFilter
            {
                MinRating = minRating ?? 0,
                MaxRating = maxRating ?? 5
            };
        }
        return null;
    }

    private List<string> ParseTags(string? tags)
    {
        if (string.IsNullOrEmpty(tags))
            return new List<string>();

        return tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(tag => tag.Trim())
                  .Where(tag => !string.IsNullOrEmpty(tag))
                  .ToList();
    }

    private void ParseAttributeFilters(ProductSearchQuery searchQuery)
    {
        // Parse dynamic attribute filters from query string
        // Format: attr_{attributeName}=value1,value2
        foreach (var queryParam in Request.Query)
        {
            if (queryParam.Key.StartsWith("attr_", StringComparison.OrdinalIgnoreCase))
            {
                var attributeName = queryParam.Key.Substring(5); // Remove "attr_" prefix
                var values = queryParam.Value.ToString()
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => v.Trim())
                    .Where(v => !string.IsNullOrEmpty(v))
                    .ToList();

                if (values.Any())
                {
                    searchQuery.AttributeFilters[attributeName] = values;
                }
            }
        }
    }
}
