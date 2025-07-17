using MediatR;
using SearchService.Domain.Entities;
using SearchService.Domain.Enums;
using SearchService.Domain.ValueObjects;

namespace SearchService.Application.Queries.ProductSearch;

/// <summary>
/// Main product search query with full-text search, faceting, sorting, and personalization
/// </summary>
public class ProductSearchQuery : IRequest<ProductSearchResponse>
{
    /// <summary>
    /// Search query text
    /// </summary>
    public string? Query { get; set; }

    /// <summary>
    /// User ID for personalization (optional)
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Category filter
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Brand filter
    /// </summary>
    public string? Brand { get; set; }

    /// <summary>
    /// Price range filter
    /// </summary>
    public PriceRange? PriceRange { get; set; }

    /// <summary>
    /// Rating filter
    /// </summary>
    public RatingFilter? RatingFilter { get; set; }

    /// <summary>
    /// Availability filter
    /// </summary>
    public bool? IsAvailable { get; set; }

    /// <summary>
    /// Dynamic attribute filters (color, size, material, etc.)
    /// </summary>
    public Dictionary<string, List<string>> AttributeFilters { get; set; } = new();

    /// <summary>
    /// Tags filter
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Sorting option
    /// </summary>
    public SortBy SortBy { get; set; } = SortBy.Relevance;

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Page size
    /// </summary>
    public int Size { get; set; } = 20;

    /// <summary>
    /// Include facets in response
    /// </summary>
    public bool IncludeFacets { get; set; } = true;

    /// <summary>
    /// Include suggestions for spell correction
    /// </summary>
    public bool IncludeSuggestions { get; set; } = true;

    /// <summary>
    /// Enable personalization boosting
    /// </summary>
    public bool EnablePersonalization { get; set; } = true;
}

public class ProductSearchResponse
{
    public List<ProductDocument> Products { get; set; } = new();
    public PaginationInfo Pagination { get; set; } = new();
    public List<SearchFacet> Facets { get; set; } = new();
    public List<SearchSuggestion> Suggestions { get; set; } = new();
    public string? CorrectedQuery { get; set; }
    public long TotalResults { get; set; }
    public double QueryTimeMs { get; set; }
    public bool HasPersonalization { get; set; }
}
