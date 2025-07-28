using Ardalis.GuardClauses;
using BuildingBlocks.Domain.Entities;
using System;

namespace ProductService.Domain.Models
{
    public class Brand : AuditableEntity<Guid> // ارث‌بری از AggregateRoot برای قابلیت‌های پایه
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public bool IsActive { get; private set; }

        // سازنده برای EF Core
        private Brand() { Name = string.Empty; }

        public ICollection<ProductBrand> ProductBrands { get; set; }

        public static Brand Create(string name, string? description)
        {
            var brand = new Brand
            {
                Name = Guard.Against.NullOrWhiteSpace(name, nameof(name)),
                Description = description,
                IsActive = true
            };

            // می‌توانیم یک رویداد دامنه BrandCreatedEvent هم اینجا اضافه کنیم

            return brand;
        }

        public void Update(string newName, string? newDescription)
        {
            Name = Guard.Against.NullOrWhiteSpace(newName, nameof(newName));
            Description = newDescription;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}