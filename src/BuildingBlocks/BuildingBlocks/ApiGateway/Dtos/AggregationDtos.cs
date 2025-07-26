namespace BuildingBlocks.ApiGateway.Dtos
{
    // نکته: Namespace را به محل جدید فایل تغییر دادیم.

    #region Data Models for Aggregation
    
    public class DashboardData
    {
        public UserData? User { get; set; }
        public List<OrderData> RecentOrders { get; set; } = new();
        public List<ProductData> Recommendations { get; set; } = new();
    }

    public class OrderDetailsData
    {
        public OrderData? Order { get; set; }
        public ShippingData? Shipping { get; set; }
        public List<ProductData> Products { get; set; } = new();
    }

    public class UserData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class OrderData
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<OrderItemData> Items { get; set; } = new();
    }

    public class OrderItemData
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class ProductData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class ShippingData
    {
        public string TrackingNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime EstimatedDelivery { get; set; }
    }

    #endregion
}