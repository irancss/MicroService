using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CartApi.Models
{
    public class Cart
    {
        // کلید اصلی در Redis همان UserId خواهد بود
        [JsonIgnore] // UserId بخشی از کلید است، نه خود داده JSON در Redis
        public string UserId { get; private set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal TotalPrice => Items.Sum(item => item.UnitPrice * item.Quantity);
        public decimal DiscountAmount { get; set; } = 0; // مبلغ تخفیف اعمال شده
        public string AppliedDiscountCode { get; set; } // کد تخفیف اعمال شده
        public decimal FinalPrice => TotalPrice - DiscountAmount; // قیمت نهایی

        public Cart(string userId)
        {
            UserId = userId;
        }
    }

    public class CartItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; } // بهتر است از سرویس محصول گرفته شود
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // قیمت در زمان افزودن به سبد
        public string ImageUrl { get; set; } // (اختیاری)
    }

    public record AddItemRequest
    {
        [Required]
        public string ProductId { get; set; }
        [Range(1, 100)] // محدودیت تعداد
        public int Quantity { get; set; }
        // قیمت و نام محصول باید از سرویس دیگری (مثل Product Service) گرفته شود
    }

    public class ApplyDiscountRequest
    {
        public string DiscountCode { get; set; }
    }
}
