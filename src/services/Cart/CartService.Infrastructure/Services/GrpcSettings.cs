namespace Cart.Infrastructure.Services;

public class GrpcSettings
{
    public string InventoryServiceUrl { get; set; } = "https://localhost:5001";
    public string CatalogServiceUrl { get; set; } = "https://localhost:5002";
}