namespace IdentityServer.Models
{
    public class UsersListViewModel
    {
        public List<UserViewModel> Users { get; set; } = new();
        public int TotalCount { get; set; }
        public int TotalUsers => TotalCount; // For backward compatibility with views
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public string? SearchTerm { get; set; }
        public bool? IsActiveFilter { get; set; }
    }

    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}".Trim();
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool IsMobileVerified { get; set; }
        public DateTime? LockoutEndDate { get; set; }
    }
}
