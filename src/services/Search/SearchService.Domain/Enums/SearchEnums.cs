namespace SearchService.Domain.Enums;

/// <summary>
/// Available sorting options for search results
/// </summary>
public enum SortBy
{
    Relevance,
    PriceLowToHigh,
    PriceHighToLow,
    NewestArrivals,
    HighestRated,
    MostReviewed,
    BestSelling,
    NameAsc,
    NameDesc
}

/// <summary>
/// Product-specific sorting options
/// </summary>
public enum ProductSortBy
{
    Relevance,
    Price,
    Rating,
    CreatedDate,
    Name,
    Popularity
}

/// <summary>
/// Sort direction
/// </summary>
public enum SortDirection
{
    Ascending,
    Descending
}

/// <summary>
/// Search operation types for analytics
/// </summary>
public enum SearchOperationType
{
    FullTextSearch,
    CategoryBrowse,
    BrandSearch,
    AttributeFilter,
    AutoComplete,
    Suggestion
}

/// <summary>
/// Product index operations
/// </summary>
public enum IndexOperation
{
    Index,
    Update,
    Delete
}
