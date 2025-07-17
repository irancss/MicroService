using System; // For ArgumentNullException
using ProductService.Domain.ValueObjects; // Assuming a base ValueObject class exists

namespace ProductService.Domain.Models
{
    public enum MediaTypeOption // Example Enum
    {
        Image,
        Video,
        Document
    }

    public class MediaInfo : ValueObject // Inherit from base ValueObject for equality
    {
        public string Id { get; set; }
        public string Url { get; }
        public MediaTypeOption MediaType { get; } // Using Enum
        public string AltText { get; }
        public int Order { get; }
        public string? Description { get; }
        public string? FileSize { get; set; } // Assuming FileSize is a class or struct defined elsewhere
        public DateTime CreatedAt { get; set; }

        public MediaInfo(string url, MediaTypeOption mediaType, string altText, int order, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrWhiteSpace(altText) && mediaType == MediaTypeOption.Image) // Alt text important for images
                throw new ArgumentNullException(nameof(altText), "Alt text is required for images.");
            if (order < 0)
                throw new ArgumentOutOfRangeException(nameof(order), "Order cannot be negative.");

            Url = url;
            MediaType = mediaType;
            AltText = altText;
            Order = order;
            Description = description;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Url;
            yield return MediaType;
            yield return AltText;
            yield return Order;
            yield return Description;
        }
    }
}