
using ProductService.Application.DTOs.Product;

public interface IProductService
{
    Task<string> CreateProductAsync(CreateProductCommand command);
    Task UpdateProductAsync(UpdateProductRequest command);
    Task DeleteProductAsync(string productId);
    Task<ProductDto> GetProductByIdAsync(string productId);
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<IEnumerable<ProductVariantDto>> GetProductVariantsAsync(string productId);
    //Task AddProductVariantAsync(AddProductVariantCommand command);
    //Task UpdateProductVariantAsync(UpdateProductVariantCommand command);
    Task DeleteProductVariantAsync(string variantId);
}