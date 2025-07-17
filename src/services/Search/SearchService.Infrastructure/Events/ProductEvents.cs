namespace SearchService.Infrastructure.Events;

/// <summary>
/// Event published when a product is created
/// </summary>
public class ProductCreatedEvent
{
    public string ProductId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public bool IsAvailable { get; set; }
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
    public Dictionary<string, object> Attributes { get; set; } = new();
    public string Slug { get; set; } = string.Empty;
    public double? Weight { get; set; }
}

/// <summary>
/// Event published when a product is updated
/// </summary>
public class ProductUpdatedEvent
{
    public string ProductId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public bool IsAvailable { get; set; }
    public int StockQuantity { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
    public Dictionary<string, object> Attributes { get; set; } = new();
    public string Slug { get; set; } = string.Empty;
    public double? Weight { get; set; }
}

/// <summary>
/// Event published when a product is deleted
/// </summary>
public class ProductDeletedEvent
{
    public string ProductId { get; set; } = string.Empty;
    public DateTime DeletedAt { get; set; }
}
