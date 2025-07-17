using System.ComponentModel.DataAnnotations;

namespace ProductService.Application.DTOs.Product
{
    public class DeleteProductRequest
    {
        /// <summary>
        /// Stock Keeping Unit (unique identifier for the product).
        /// </summary>
        [Required]
        public string Sku { get; set; }
    }
}
