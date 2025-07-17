using MediatR;
using SearchService.Domain.ValueObjects;

namespace SearchService.Application.Queries.SearchSuggestions;

/// <summary>
/// Query for getting search auto-complete suggestions
/// </summary>
public class GetSearchSuggestionsQuery : IRequest<GetSearchSuggestionsResponse>
{
    /// <summary>
    /// Partial query text for auto-complete
    /// </summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// Maximum number of suggestions to return
    /// </summary>
    public int MaxSuggestions { get; set; } = 10;

    /// <summary>
    /// Include product suggestions
    /// </summary>
    public bool IncludeProducts { get; set; } = true;

    /// <summary>
    /// Include category suggestions
    /// </summary>
    public bool IncludeCategories { get; set; } = true;

    /// <summary>
    /// Include brand suggestions
    /// </summary>
    public bool IncludeBrands { get; set; } = true;

    /// <summary>
    /// User ID for personalized suggestions (optional)
    /// </summary>
    public string? UserId { get; set; }
}

public class GetSearchSuggestionsResponse
{
    public List<SearchSuggestion> Suggestions { get; set; } = new();
    public double QueryTimeMs { get; set; }
}
