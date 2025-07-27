namespace ProductService.Domain.Models
{
    public class ProductTag
    {
        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = null!;

        public Guid TagId { get; private set; }
        public Tag Tag { get; private set; } = null!;

        // سازنده برای EF Core
        private ProductTag() { }

        // سازنده عمومی برای ایجاد نمونه جدید
        public ProductTag(Guid productId, Guid tagId)
        {
            ProductId = productId;
            TagId = tagId;
        }
    }
}
