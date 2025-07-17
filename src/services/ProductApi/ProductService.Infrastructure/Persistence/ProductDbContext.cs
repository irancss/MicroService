using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Models;
using ProductService.Infrastructure.Domain;

namespace ProductService.Infrastructure.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options)
        {

            
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Review> ProductReviews { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductSpecification> ProductSpecifications { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
        public DbSet<ProductVariantImage> ProductVariantImages { get; set; }
        public DbSet<ProductVariantPrice> ProductVariantPrices { get; set; }
        public DbSet<ProductVariantStock> ProductVariantStocks { get; set; }
        public DbSet<ProductComment> ProductComments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductEntityTypeConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AnswerEntityTypeConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BrandEntityTypeConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CategoryEntityTypeConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductCommentEntityTypeConfiguration).Assembly);
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(MediaEntityTypeConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductAttributeEntityTypeConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductAttributeValueEntityTypeConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductBrandEntityTypeConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductCategoryEntityTypeConfiguration).Assembly);
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductDescriptionEntityTypeConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductImageEntityTypeConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductSpecificationEntityTypeConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductTagEntityTypeConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductVariantEntityTypeConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductVariantImageEntityTypeConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductVariantPriceEntityTypeConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductVariantStockEntityTypeConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(QuestionEntityTypeConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReviewEntityTypeConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TagEntityTypeConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ScheduledDiscountEntityTypeConfiguration).Assembly);
        }
    }
}
