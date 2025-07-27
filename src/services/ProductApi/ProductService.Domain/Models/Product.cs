using Ardalis.GuardClauses;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Events;
using ProductService.Domain.Events;
using ProductService.Domain.ValueObjects;
using System;

namespace ProductService.Domain.Models // Or ProductService.Domain.Models
{
    public class Product : AggregateRoot
    {
        public ProductName Name { get; private set; }
        public string? Description { get; private set; }
        public ProductPrice Price { get; private set; }
        public Sku Sku { get; private set; }
        public int StockQuantity { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsFeatured { get; private set; }

        // Relationships
        public Guid? BrandId { get; private set; }
        public Brand? Brand { get; private set; }

        private readonly List<ProductCategory> _productCategories = new();
        public IReadOnlyCollection<ProductCategory> ProductCategories => _productCategories.AsReadOnly();

        private readonly List<ProductTag> _productTags = new();
        public IReadOnlyCollection<ProductTag> ProductTags => _productTags.AsReadOnly();

        // سازنده برای EF Core
        private Product() { }

        public static Product Create(
            ProductName name,
            ProductPrice price,
            Sku sku,
            int initialStock,
            string? description,
            Guid? brandId)
        {
            var product = new Product
            {
                Name = Guard.Against.Null(name, nameof(name)),
                Price = Guard.Against.Null(price, nameof(price)),
                Sku = Guard.Against.Null(sku, nameof(sku)),
                Description = description,
                StockQuantity = Guard.Against.Negative(initialStock, nameof(initialStock)),
                BrandId = brandId,
                IsActive = true, // محصولات جدید به طور پیش‌فرض فعال هستند
                IsFeatured = false
            };

            product.AddDomainEvent(new ProductCreatedDomainEvent(product.Id, product.Name, product.Sku, product.Price, product.StockQuantity));

            return product;
        }

        public void Update(
            ProductName newName,
            string? newDescription,
            ProductPrice newPrice,
            Guid? newBrandId)
        {
            Name = Guard.Against.Null(newName, nameof(newName));
            Description = newDescription;
            Price = Guard.Against.Null(newPrice, nameof(newPrice));
            BrandId = newBrandId;

            // رویداد دامنه برای به‌روزرسانی
            AddDomainEvent(new ProductUpdatedDomainEvent(this.Id));
        }

        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }

        public void UpdateStock(int newQuantity)
        {
            int oldQuantity = StockQuantity;
            StockQuantity = Guard.Against.Negative(newQuantity, nameof(newQuantity));

            // اگر محصول قبلا ناموجود بوده و الان موجود شده، رویداد منتشر کن
            if (oldQuantity == 0 && newQuantity > 0)
            {
                AddDomainEvent(new ProductBackInStockDomainEvent(this.Id, this.Name));
            }
        }

        public void AddCategory(Guid categoryId)
        {
            Guard.Against.Default(categoryId, nameof(categoryId));
            if (!_productCategories.Any(pc => pc.CategoryId == categoryId))
            {
                _productCategories.Add(new ProductCategory(this.Id, categoryId));
            }
        }

        public void RemoveCategory(Guid categoryId)
        {
            var productCategory = _productCategories.FirstOrDefault(pc => pc.CategoryId == categoryId);
            if (productCategory != null)
            {
                _productCategories.Remove(productCategory);
            }
        }
        public void UpdatePrice(ProductPrice newPrice)
        {
            // اگر قیمت تغییر نکرده، کاری انجام نده
            if (this.Price == newPrice) return;

            var oldPrice = this.Price;
            this.Price = Guard.Against.Null(newPrice, nameof(newPrice));

            // رویداد دامنه را برای انتشار بعدی اضافه کن
            this.AddDomainEvent(new ProductPriceUpdatedDomainEvent(this.Id, newPrice, oldPrice));
        }

        public void SetTags(IEnumerable<Guid> tagIds)
        {
            _productTags.Clear();
            var tagsToAdd = tagIds.Distinct().Select(tagId => new ProductTag(this.Id, tagId));
            _productTags.AddRange(tagsToAdd);
        }
    }
}