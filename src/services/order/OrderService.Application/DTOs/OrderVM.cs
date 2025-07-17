using OrderService.Core.Enums;
using OrderService.Core.Models;

namespace OrderService.Application.DTOs
{
    public class OrderVM
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
        public List<OrderItemVM> Items { get; set; } = new List<OrderItemVM>();
    }
    public class OrderItemVM
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; }
    }
}
