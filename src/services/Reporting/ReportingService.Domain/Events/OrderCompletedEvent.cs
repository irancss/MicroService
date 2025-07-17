using MediatR;

namespace ReportingService.Domain.Events;

/// <summary>
/// Event published when an order is completed and needs to be processed for reporting
/// This represents data from external microservices
/// </summary>
public record OrderCompletedEvent : INotification
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public DateTime OrderDate { get; init; }
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int TotalItems { get; init; }
    public decimal Tax { get; init; }
    public decimal Discount { get; init; }
    
    // Product details
    public List<OrderItem> Items { get; init; } = new();
    
    // Customer details
    public CustomerInfo Customer { get; init; } = new();
}

public record OrderItem
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string SubCategory { get; init; } = string.Empty;
    public string Brand { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Quantity { get; init; }
    public decimal Total { get; init; }
}

public record CustomerInfo
{
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Segment { get; init; } = string.Empty;
    public DateTime RegistrationDate { get; init; }
}
