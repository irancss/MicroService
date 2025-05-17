using System.ComponentModel.DataAnnotations;

namespace ProductApi.Models.Dtos
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
