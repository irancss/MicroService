using ProductService.Domain.Common;
using ProductService.Domain.Events;
using ProductService.Domain.ValueObjects;

namespace ProductService.Domain.Models // Or ProductService.Domain.Models
{
    public class Product : AuditableEntity
    {
        private readonly List<DomainEvent> _domainEvents = new List<DomainEvent>();
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public ProductName Name { get; private set; }
        public string? ProductType { get; set; } // e.g., "Physical", "Digital", "Service"

        public string Description { get; private set; }
        public decimal BasePrice { get; set; } // Default price if no variants or variants don't override
        public string Sku { get; private set; } // Consider a SkuValueObject
        public bool IsFeatured { get; set; } = false;

        private readonly List<string> _categories = new List<string>();
        public IReadOnlyCollection<string> Categories => _categories.AsReadOnly();

        private readonly List<string> _tags = new List<string>();
        public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

        // For Attributes, consider a more strongly-typed approach if possible
        private readonly Dictionary<string, object> _attributes = new Dictionary<string, object>();
        public IReadOnlyDictionary<string, object> Attributes => _attributes;


        private readonly List<MediaInfo> _media = new List<MediaInfo>();
        public IReadOnlyCollection<MediaInfo> Media => _media.AsReadOnly();

        public double AverageRating { get; private set; } // Should be calculated
        public int ReviewCount { get; private set; }    // Should be calculated

        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public int StockQuantity { get; private set; }
        public string Brand { get; private set; }
        // public List<string> Tags { get; private set; } // This was duplicated, ensure one definition
        public decimal? Weight { get; private set; }
        public ProductDimensions Dimensions { get; private set; } // Assuming ProductDimensions is a class/struct
        public string VendorId { get; private set; }
        public bool IsDeleted { get; private set; }
        
        private readonly List<ScheduledDiscount> _scheduledDiscounts = new List<ScheduledDiscount>();
        public IReadOnlyCollection<ScheduledDiscount> ScheduledDiscounts => _scheduledDiscounts.AsReadOnly();

        public virtual ICollection<ProductBrand> ProductBrands { get; set; } = new List<ProductBrand>();
        public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>(); // Images for the base product
        public virtual ICollection<ProductSpecification> Specifications { get; set; } = new List<ProductSpecification>();
        public virtual ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public virtual ICollection<ProductDescriptiveAttribute> DescriptiveAttributes { get; set; } = new List<ProductDescriptiveAttribute>();
        public List<Review> Reviews { get; set; }
        public List<Question> Questions { get; set; }

        public string? Manufacturer { get; private set; }

        public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
        // Private constructor for ORM
        private Product() { }

        public Product(ProductName name, string description, decimal price, string sku, string brand, int initialStock, string? manufacturer = null /* other essential params */)
        {
            if (price <= 0) throw new ArgumentOutOfRangeException(nameof(price), "Price must be positive.");
            if (initialStock < 0) throw new ArgumentOutOfRangeException(nameof(initialStock), "Stock quantity cannot be negative.");
            if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentNullException(nameof(sku));
            // Add other validations

            Name = name;
            Description = description;
            BasePrice = price;
            Sku = sku;
            Brand = brand;
            StockQuantity = initialStock;
            Manufacturer = manufacturer;

            IsActive = true;
            IsDeleted = false;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            // Tags = new List<string>(); // _tags is already initialized

            _domainEvents.Add(new ProductCreatedEvent(this)); // Assuming ProductCreatedEvent exists
        }

        public void UpdateDetails(ProductName newName, string newDescription /* other updatable fields */)
        {
            Name = newName;
            Description = newDescription;
            // Add validation
            UpdatedAt = DateTime.UtcNow;
            // Add ProductUpdatedEvent
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice <= 0) throw new ArgumentOutOfRangeException(nameof(newPrice), "Price must be positive.");
            BasePrice = newPrice;
            UpdatedAt = DateTime.UtcNow;
            // Add ProductPriceChangedEvent
        }

        public void AddCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category) || _categories.Contains(category)) return; // Or throw exception
            _categories.Add(category);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveCategory(string category)
        {
            if (_categories.Remove(category))
            {
                UpdatedAt = DateTime.UtcNow;
            }
        }
        
        public void AddTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag) || _tags.Contains(tag)) return;
            _tags.Add(tag);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveTag(string tag)
        {
            if (_tags.Remove(tag))
            {
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void AddMedia(MediaInfo mediaInfo)
        {
            // Add validation, e.g., prevent duplicates based on URL or an ID if MediaInfo has one
            _media.Add(mediaInfo);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveMedia(string mediaUrl) // Or by Id if MediaInfo has one
        {
            var mediaToRemove = _media.FirstOrDefault(m => m.Url == mediaUrl);
            if (mediaToRemove != null)
            {
                _media.Remove(mediaToRemove);
                UpdatedAt = DateTime.UtcNow;
            }
        }
        
        public void AddScheduledDiscount(ScheduledDiscount discount)
        {
            // Add validation (e.g., date ranges, discount value)
            _scheduledDiscounts.Add(discount);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveScheduledDiscount(string discountId) // Assuming ScheduledDiscount has an Id
        {
            var discountToRemove = _scheduledDiscounts.FirstOrDefault(d => d.Id == discountId); // Assuming Id property
            if (discountToRemove != null)
            {
                _scheduledDiscounts.Remove(discountToRemove);
                UpdatedAt = DateTime.UtcNow;
            }
        }


        public void Activate()
        {
            if (!IsActive)
            {
                IsActive = true;
                UpdatedAt = DateTime.UtcNow;
                // Add ProductActivatedEvent
            }
        }

        public void Deactivate()
        {
            if (IsActive)
            {
                IsActive = false;
                UpdatedAt = DateTime.UtcNow;
                // Add ProductDeactivatedEvent
            }
        }

        public void MarkAsDeleted()
        {
            if (!IsDeleted)
            {
                IsDeleted = true;
                IsActive = false; // Typically, deleted products are also inactive
                UpdatedAt = DateTime.UtcNow;
                // Add ProductDeletedEvent
            }
        }
        
        // Method to update stock (example)
        public void AddStock(int quantity)
        {
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity to add must be positive.");
            StockQuantity += quantity;
            UpdatedAt = DateTime.UtcNow;
            // Add StockUpdatedEvent
        }

        public void RemoveStock(int quantity)
        {
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity to remove must be positive.");
            if (StockQuantity < quantity) throw new InvalidOperationException("Not enough stock.");
            StockQuantity -= quantity;
            UpdatedAt = DateTime.UtcNow;
            // Add StockUpdatedEvent
        }


        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}