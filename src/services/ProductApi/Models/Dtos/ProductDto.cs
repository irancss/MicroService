namespace ProductApi.Models.Dtos
{
    public class ProductDto
    {
        public string Id { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Categories { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountedPrice { get; set; } // این فیلد توسط فراخوانی Discount Service پر می‌شود
        public Dictionary<string, object> Attributes { get; set; }
        public List<MediaInfoDto> Media { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public bool IsActive { get; set; }

        // Optional: Add a constructor for easier initialization
        public ProductDto()
        {
            Categories = new List<string>();
            Attributes = new Dictionary<string, object>();
            Media = new List<MediaInfoDto>();
        }
    }

    public class MediaInfoDto
    {
        public string Id { get; set; }
        public string Url { get; set; } // آدرس CDN
        public string MediaType { get; set; }
        public string AltText { get; set; }
        public int Order { get; set; }
    }

    
}
