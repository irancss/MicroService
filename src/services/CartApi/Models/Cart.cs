using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CartApi.Models
{
    public class Cart
    {
        // Redis key is the UserId
        [JsonIgnore] // UserId is part of the key, not the JSON data in Redis
        public string UserId { get; private set; }

        public List<CartItem> Items { get; private set; } = new List<CartItem>();

        public decimal TotalPrice => Items.Sum(item => item.UnitPrice * item.Quantity);

        public decimal DiscountAmount { get; private set; } = 0;

        public string? AppliedDiscountCode { get; private set; }

        public decimal FinalPrice => Math.Max(0, TotalPrice - DiscountAmount);

        public Cart(string userId)
        {
            UserId = userId;
        }

        public void AddItem(CartItem item)
        {
            var existing = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }

        public void RemoveItem(string productId)
        {
            var item = Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                Items.Remove(item);
            }
        }

        public void ApplyDiscount(string discountCode, decimal discountAmount)
        {
            AppliedDiscountCode = discountCode;
            DiscountAmount = Math.Min(discountAmount, TotalPrice);
        }

        public void ClearDiscount()
        {
            AppliedDiscountCode = null;
            DiscountAmount = 0;
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
