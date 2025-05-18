using OrderService.Core.Enums;

namespace OrderService.Core.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }

        public decimal TotalPrice { get; set; }
        public decimal TotalDiscount { get; set; }
        public OrderStatus Status { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
        public string LastUpdatedBy { get; set; }

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
