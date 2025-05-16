using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace User.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; } // Optional phone number field

        public string[] Roles { get; set; } = Array.Empty<string>();
        public string[] FavoriteCategories { get; set; } = Array.Empty<string>();
        public string[] FavoriteProducts { get; set; } = Array.Empty<string>();
        public string[] FavoriteBrands { get; set; } = Array.Empty<string>();

        public string NotificationSettingsJson { get; set; } = "{}"; // JSON ذخیره‌شده
        public string SearchHistoryJson { get; set; } = "[]";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        #region Optional Fields
        public string? Address { get; set; }
        public DateTime? UpdatedAt { get; set; } = null;
        public DateTime? DeletedAt { get; set; } = null;
        public string? ProfilePicture { get; set; } // URL or path to the profile picture
        public string? NationalCode { get; set; }
        public string? Email { get; set; }
        public string? Job { get; set; }
        public DateTime? Birthday { get; set; }
        public string? EconomicCode { get; set; }
        #endregion

    }
}
