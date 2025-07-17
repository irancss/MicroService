
using ProductService.Application.DTOs.Product;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductVariantStock _productVariantStock;

        public ProductService(IProductRepository productRepository, IProductVariantStock productVariantStock)
        {
            _productRepository = productRepository;
            _productVariantStock = productVariantStock;
        }

        public async Task<string> CreateProductAsync(CreateProductCommand command)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateProductAsync(UpdateProductRequest command)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteProductAsync(string productId)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductDto> GetProductByIdAsync(string productId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProductVariantDto>> GetProductVariantsAsync(string productId)
        {
            throw new NotImplementedException();
        }

        public async Task AddProductVariantAsync(AddProductVariantCommand command)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateProductVariantAsync(UpdateProductVariantCommand command)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteProductVariantAsync(string variantId)
        {
            throw new NotImplementedException();
        }
    }
} 
