using System.ComponentModel.DataAnnotations;

namespace ProductService.Application.DTOs.Product
{
    /// <summary>
    /// DTO for receiving data when creating a new product.
    /// </summary>
    public class CreateProductRequest
    {
        /// <summary>
        /// Stock Keeping Unit (unique identifier for the product).
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Name of the product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the product.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// List of category names or IDs the product belongs to.
        /// </summary>
        public List<string> Categories { get; set; } = new();

        /// <summary>
        /// Price of the product.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Additional attributes for the product (e.g., color, size).
        /// </summary>
        public Dictionary<string, object> Attributes { get; set; } = new();

        /// <summary>
        /// Indicates if the product is active.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }

}
