using System.ComponentModel.DataAnnotations;

namespace SearchService.Domain.Entities;

/// <summary>
/// Elasticsearch product document that supports full-text search, faceting, and personalization
/// </summary>
public class ProductDocument
{
    /// <summary>
    /// Unique product identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Product name - optimized for full-text search
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Product description - searchable content
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Product category - used for faceting and boosting
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Brand name - used for faceting and personalization boosting
    /// </summary>
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// Product price - used for range filtering and sorting
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Average customer rating - used for sorting and filtering
    /// </summary>
    public double AverageRating { get; set; }

    /// <summary>
    /// Number of reviews
    /// </summary>
    public int ReviewCount { get; set; }

    /// <summary>
    /// Product availability status
    /// </summary>
    public bool IsAvailable { get; set; }

    /// <summary>
    /// Stock quantity
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Product creation date - used for "newest arrivals" sorting
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Product tags for enhanced search capabilities
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Product images
    /// </summary>
    public List<string> ImageUrls { get; set; } = new();

    /// <summary>
    /// Dynamic product attributes for flexible faceting (color, size, material, etc.)
    /// </summary>
    public Dictionary<string, object> Attributes { get; set; } = new();

    /// <summary>
    /// SEO-friendly URL slug
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Product weight (for shipping calculations)
    /// </summary>
    public double? Weight { get; set; }

    /// <summary>
    /// Product dimensions
    /// </summary>
    public ProductDimensions? Dimensions { get; set; }

    /// <summary>
    /// Search relevance score (calculated by Elasticsearch)
    /// </summary>
    public double? Score { get; set; }
}

public class ProductDimensions
{
    public double Length { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public string Unit { get; set; } = "cm";
}
