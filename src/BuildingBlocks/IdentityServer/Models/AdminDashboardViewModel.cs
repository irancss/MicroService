namespace IdentityServer.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int VerifiedUsers { get; set; } // For backward compatibility
        public int TodayRegistrations { get; set; }
        public List<RecentUser> RecentUsers { get; set; } = new();
        public List<SystemLog> RecentLogs { get; set; } = new();
    }

    public class RecentUser
    {
        public string Id { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}".Trim();
        public DateTime RegisteredAt { get; set; }
        public DateTime CreatedAt => RegisteredAt; // For backward compatibility
        public bool IsActive { get; set; }
        public bool IsMobileVerified { get; set; }
    }

    public class SystemLog
    {
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? Exception { get; set; }
    }
}
