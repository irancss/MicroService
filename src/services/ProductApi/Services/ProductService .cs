using AutoMapper;
using ProductApi.Models.Dtos;
using ProductApi.Models.Entities;
using ProductApi.Repositories;
using ProductApi.Services.Caching;
using Polly;

namespace ProductApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;
        private readonly IMediaService _mediaService;
        private readonly ICachingService _cache; // تزریق سرویس کشینگ
        private readonly IDiscountServiceClient _discountServiceClient; // تزریق کلاینت سرویس تخفیف

        // تعریف کلیدهای کش
        private const string ProductIdCachePrefix = "product:id:";
        private const string ProductSkuCachePrefix = "product:sku:";
        private static readonly TimeSpan DefaultCacheExpiration = TimeSpan.FromMinutes(5);


        public ProductService(
            IProductRepository productRepository,
            IMapper mapper,
            ILogger<ProductService> logger,
            IMediaService mediaService,
            ICachingService cache, // دریافت از DI
            IDiscountServiceClient discountServiceClient // دریافت از DI
        )
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
            _mediaService = mediaService;
            _cache = cache; // ذخیره اینجکشن
            _discountServiceClient = discountServiceClient;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductRequest productRequest)
        {
            _logger.LogInformation("Attempting to create product with SKU: {Sku}", productRequest.Sku);

            // بررسی عدم وجود SKU تکراری
            var existingProduct = await _productRepository.GetBySkuAsync(productRequest.Sku);
            if (existingProduct != null)
            {
                _logger.LogWarning("Product creation failed: SKU '{Sku}' already exists.", productRequest.Sku);
                throw new ArgumentException($"Product with SKU '{productRequest.Sku}' already exists.");
            }

            var productEntity = _mapper.Map<Product>(productRequest);
            // مقادیر اولیه و تاریخ‌ها توسط ریپازیتوری مقداردهی می‌شوند

            await _productRepository.CreateAsync(productEntity);
            _logger.LogInformation("Product created with ID: {ProductId}, SKU: {Sku}", productEntity.Id, productEntity.Sku);

            // --- تبدیل به DTO و افزودن اطلاعات مدیا (مانند قبل) ---
            var productDto = await MapProductToDtoAsync(productEntity); // متد کمکی جدید

            // --- کش کردن محصول جدید ---
            await CacheProductDtoAsync(productDto); // متد کمکی جدید

            return productDto;
        }
        public async Task<bool> DeleteProductAsync(string id)
        {
            _logger.LogInformation("Attempting to delete product with ID: {ProductId}", id);
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product deletion failed: Product with ID '{ProductId}' not found.", id);
                return false; // یا throw KeyNotFoundException
            }

            var success = await _productRepository.DeleteAsync(id); // Soft delete

            if (success)
            {
                _logger.LogInformation("Product soft deleted successfully: {ProductId}", id);
                // حذف از کش در صورت نیاز
                await _cache.RemoveAsync(ProductIdCachePrefix + id);
                await _cache.RemoveAsync(ProductSkuCachePrefix + product.Sku);
            }
            else
            {
                _logger.LogError("Failed to soft delete product with ID: {ProductId}", id);
            }
            return success;
        }

        public async Task<PagedResult<ProductDto>> GetProductsAsync(int page, int pageSize, string? category = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            _logger.LogInformation("Fetching products - Page: {Page}, PageSize: {PageSize}, Category: {Category}", page, pageSize, category ?? "None");
            var products = await _productRepository.GetAllAsync(null, pageSize, category);
            var totalCount = await _productRepository.GetTotalCountAsync(category); // فیلترها باید یکسان باشند

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            // محاسبه URL و تخفیف برای هر محصول
            foreach (var dto in productDtos)
            {
                var originalProduct = products.FirstOrDefault(p => p.Id == dto.Id); // پیدا کردن Entity اصلی
                if (originalProduct != null)
                {
                    //dto.Media = _mediaService.GetMediaUrls(originalProduct.Media);
                    dto.DiscountedPrice = CalculateCurrentDiscount(originalProduct);
                }
            }


            return new PagedResult<ProductDto>
            {
                Items = productDtos,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ProductDto?> GetProductByIdAsync(string id)
        {
            _logger.LogDebug("Fetching product by ID: {ProductId}", id);
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            var productDto = _mapper.Map<ProductDto>(product);
            //productDto.Media = _mediaService.GetMediaUrls(product.Media);
            productDto.DiscountedPrice = CalculateCurrentDiscount(product);
            return productDto;
        }

        public async Task<ProductDto?> GetProductBySkuAsync(string sku)
        {
            _logger.LogDebug("Fetching product by SKU: {Sku}", sku);
            var product = await _productRepository.GetBySkuAsync(sku);
            if (product == null) return null;

            var productDto = _mapper.Map<ProductDto>(product);
            //productDto.Media = _mediaService.GetMediaUrls(product.Media);
            productDto.DiscountedPrice = CalculateCurrentDiscount(product);
            return productDto;
        }

        public async Task<bool> UpdateProductAsync(string id, UpdateProductRequest productRequest)
        {
            _logger.LogInformation("Attempting to update product with ID: {ProductId}", id);
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                _logger.LogWarning("Product update failed: Product with ID '{ProductId}' not found.", id);
                return false; // یا throw KeyNotFoundException
            }

            // بررسی SKU اگر تغییر کرده و تکراری است
            if (!string.IsNullOrEmpty(productRequest.Sku) && existingProduct.Sku != productRequest.Sku)
            {
                var conflictingProduct = await _productRepository.GetBySkuAsync(productRequest.Sku);
                if (conflictingProduct != null)
                {
                    _logger.LogWarning("Product update failed: New SKU '{Sku}' already exists.", productRequest.Sku);
                    throw new ArgumentException($"Product with SKU '{productRequest.Sku}' already exists.");
                }
            }

            // مپ کردن تغییرات روی Entity موجود (از طریق AutoMapper یا دستی)
            _mapper.Map(productRequest, existingProduct);
            // existingProduct.UpdatedAt = DateTime.UtcNow; // ریپازیتوری انجام می‌دهد

            var success = await _productRepository.UpdateAsync(id, existingProduct);

            if (success)
            {
                _logger.LogInformation("Product updated successfully: {ProductId}", id);
                // حذف از کش در صورت نیاز
                await _cache.RemoveAsync(ProductIdCachePrefix + id);
                await _cache.RemoveAsync(ProductSkuCachePrefix + existingProduct.Sku);
            }
            else
            {
                _logger.LogError("Failed to update product with ID: {ProductId}", id);
            }

            return success;
        }

        // متد کمکی برای محاسبه تخفیف فعال فعلی
        private decimal? CalculateCurrentDiscount(Product product)
        {
            var now = DateTime.UtcNow;
            var activeDiscount = product.ScheduledDiscounts?
                .Where(d => d.IsActive &&
                            (!d.StartDate.HasValue || d.StartDate.Value <= now) &&
                            (!d.EndDate.HasValue || d.EndDate.Value >= now))
                .OrderByDescending(d => d.GetEffectivePercentage(product.Price)) // اولویت با تخفیف بیشتر
                .FirstOrDefault();

            if (activeDiscount != null)
            {
                var percent = activeDiscount.DiscountPercent;
                var fixedAmount = activeDiscount.FixedAmount ?? 0m;
                if (percent > 0)
                {
                    return product.Price * (1 - (percent / 100m));
                }
                if (fixedAmount > 0)
                {
                    return Math.Max(0, product.Price - fixedAmount); // قیمت منفی نشود
                }
            }
            return null; // بدون تخفیف
        }

        // متد کمکی فرضی برای تبدیل Product به ProductDto و افزودن اطلاعات مدیا
        private async Task<ProductDto> MapProductToDtoAsync(Product product)
        {
            var dto = _mapper.Map<ProductDto>(product);
            //dto.Media = _mediaService.GetMediaUrls(product.Media);
            dto.DiscountedPrice = CalculateCurrentDiscount(product);
            return dto;
        }

        // متد کمکی فرضی برای کش کردن ProductDto
        private async Task CacheProductDtoAsync(ProductDto productDto)
        {
            await _cache.SetAsync(ProductIdCachePrefix + productDto.Id, productDto, DefaultCacheExpiration);
            await _cache.SetAsync(ProductSkuCachePrefix + productDto.Sku, productDto, DefaultCacheExpiration);
        }
    }
    // سرویس‌های ReviewService و QnAService نیز مشابه با تزریق Repository مربوطه و AutoMapper پیاده‌سازی می‌شوند.
    // ReviewService مسئولیت آپدیت AverageRating و ReviewCount در Product را نیز دارد.

}
