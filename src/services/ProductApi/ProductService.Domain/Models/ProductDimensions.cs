using BuildingBlocks.Domain.Entities;

/// <summary>
/// Represents the dimensions of a product.
/// </summary>
public class ProductDimensions : AuditableEntity
{
    /// <summary>
    /// Gets or sets the length of the product.
    /// </summary>
    public decimal? Length { get; set; }

    /// <summary>
    /// Gets or sets the width of the product.
    /// </summary>
    public decimal? Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the product.
    /// </summary>
    public decimal? Height { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductDimensions"/> class.
    /// </summary>
    public ProductDimensions()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductDimensions"/> class with specified dimensions.
    /// </summary>
    /// <param name="length">The length of the product.</param>
    /// <param name="width">The width of the product.</param>
    /// <param name="height">The height of the product.</param>
    public ProductDimensions(decimal? length, decimal? width, decimal? height)
    {
        Length = length;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string representation of the product dimensions, e.g., "Length: 10.5, Width: 5.0, Height: N/A".</returns>
    public override string ToString()
    {
        return $"Length: {Length?.ToString() ?? "N/A"}, Width: {Width?.ToString() ?? "N/A"}, Height: {Height?.ToString() ?? "N/A"}";
    }
}