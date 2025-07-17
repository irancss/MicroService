namespace SearchService.Domain.ValueObjects;

/// <summary>
/// Represents a search facet (filter) with available options and counts
/// </summary>
public class SearchFacet
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<FacetOption> Options { get; set; } = new();
}

public class FacetOption
{
    public string Value { get; set; } = string.Empty;
    public string DisplayValue { get; set; } = string.Empty;
    public long Count { get; set; }
    public bool IsSelected { get; set; }
}

/// <summary>
/// Represents a search suggestion
/// </summary>
public class SearchSuggestion
{
    public string Text { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "query", "category", "product", "brand"
    public double Score { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Represents pagination information
/// </summary>
public class PaginationInfo
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 20;
    public long TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / Size);
    public bool HasNext => Page < TotalPages;
    public bool HasPrevious => Page > 1;
}

/// <summary>
/// Represents a price range filter
/// </summary>
public class PriceRange
{
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    public bool IsValid => MinPrice.HasValue || MaxPrice.HasValue;
    public bool HasBounds => MinPrice.HasValue && MaxPrice.HasValue && MinPrice <= MaxPrice;
}

/// <summary>
/// Represents a rating filter
/// </summary>
public class RatingFilter
{
    public double MinRating { get; set; } = 0;
    public double MaxRating { get; set; } = 5;

    public bool IsValid => MinRating >= 0 && MaxRating <= 5 && MinRating <= MaxRating;
}
