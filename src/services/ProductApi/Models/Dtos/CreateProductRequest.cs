using System.ComponentModel.DataAnnotations;
using ProductApi.Models.Entities;

namespace ProductApi.Models.Dtos
{
    // DTO برای دریافت داده هنگام ایجاد محصول جدید
    public class CreateProductRequest
    {
        // [Required] و سایر annotation های اولیه حذف می‌شوند چون از FluentValidation استفاده می‌کنیم
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Categories { get; set; }
        public decimal Price { get; set; }
        public Dictionary<string, object> Attributes { get; set; }
        public bool IsActive { get; set; } = true;
        // StockQuantity و ScheduledDiscounts حذف شدند
    }

    // UpdateProductRequest مشابه CreateProductRequest (با تغییرات لازم)
}
