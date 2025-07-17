using ReportingService.Domain.Entities;

namespace ReportingService.Domain.Entities;

/// <summary>
/// Fact table for storing processed order data for analytics
/// This represents the "T" and "L" in ETL - Transformed and Loaded data
/// </summary>
public class OrderFact : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public int TotalItems { get; private set; }
    
    // Dimension keys for star schema
    public Guid ProductDimensionId { get; private set; }
    public Guid CustomerDimensionId { get; private set; }
    public Guid DateDimensionId { get; private set; }
    
    // Measures for aggregation
    public decimal Revenue { get; private set; }
    public decimal Tax { get; private set; }
    public decimal Discount { get; private set; }

    private OrderFact() { } // EF Core

    public OrderFact(
        Guid orderId, 
        Guid customerId, 
        DateTime orderDate, 
        decimal totalAmount, 
        string currency, 
        string status, 
        int totalItems,
        Guid productDimensionId,
        Guid customerDimensionId,
        Guid dateDimensionId,
        decimal revenue,
        decimal tax,
        decimal discount)
    {
        OrderId = orderId;
        CustomerId = customerId;
        OrderDate = orderDate;
        TotalAmount = totalAmount;
        Currency = currency;
        Status = status;
        TotalItems = totalItems;
        ProductDimensionId = productDimensionId;
        CustomerDimensionId = customerDimensionId;
        DateDimensionId = dateDimensionId;
        Revenue = revenue;
        Tax = tax;
        Discount = discount;
    }

    public void UpdateAmount(decimal newAmount, decimal revenue, decimal tax, decimal discount)
    {
        TotalAmount = newAmount;
        Revenue = revenue;
        Tax = tax;
        Discount = discount;
        SetUpdatedAt();
    }
}
