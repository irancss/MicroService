using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(50)]
    public string? FirstName { get; set; }
    
    [MaxLength(50)]
    public string? LastName { get; set; }
    
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    public DateTime? LastLoginDate { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // برای ذخیره کد تایید موبایل
    public string? MobileVerificationCode { get; set; }
    
    public DateTime? MobileVerificationCodeExpiry { get; set; }
    
    public bool IsMobileVerified { get; set; } = false;
    
    // تعداد تلاش‌های ناموفق
    public int FailedLoginAttempts { get; set; } = 0;
    
    public DateTime? LockoutEndDate { get; set; }
}
