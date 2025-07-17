namespace ProductService.Domain.Models
{
    public class ProductTag
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public Product Product { get; set; }
        public string TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
