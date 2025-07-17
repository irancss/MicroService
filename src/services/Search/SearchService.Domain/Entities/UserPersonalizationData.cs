namespace SearchService.Domain.Entities;

/// <summary>
/// User personalization data used for boosting search results
/// </summary>
public class UserPersonalizationData
{
    /// <summary>
    /// User identifier
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Categories the user has shown interest in (with weights)
    /// </summary>
    public Dictionary<string, double> PreferredCategories { get; set; } = new();

    /// <summary>
    /// Brands the user has shown affinity for (with weights)
    /// </summary>
    public Dictionary<string, double> PreferredBrands { get; set; } = new();

    /// <summary>
    /// Price range preferences based on user's purchase history
    /// </summary>
    public PriceRangePreference? PriceRangePreference { get; set; }

    /// <summary>
    /// User's preferred product attributes
    /// </summary>
    public Dictionary<string, List<string>> PreferredAttributes { get; set; } = new();

    /// <summary>
    /// Recently viewed product IDs
    /// </summary>
    public List<string> RecentlyViewedProducts { get; set; } = new();

    /// <summary>
    /// Last time this data was updated
    /// </summary>
    public DateTime LastUpdated { get; set; }
}

public class PriceRangePreference
{
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public double Weight { get; set; } = 1.0;
}
