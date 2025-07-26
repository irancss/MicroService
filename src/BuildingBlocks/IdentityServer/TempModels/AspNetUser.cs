using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.TempModels;

public partial class AspNetUser
{
    public string Id { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime? LastLoginDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? MobileVerificationCode { get; set; }

    public DateTime? MobileVerificationCodeExpiry { get; set; }

    public bool IsMobileVerified { get; set; }

    public int FailedLoginAttempts { get; set; }

    public DateTime? LockoutEndDate { get; set; }

    public string? UserName { get; set; }

    public string? NormalizedUserName { get; set; }

    public string? Email { get; set; }

    public string? NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTime? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }
}


public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "شماره موبایل الزامی است")]
    [Phone(ErrorMessage = "فرمت شماره موبایل صحیح نیست")]
    public string PhoneNumber { get; set; } = "";
}

public class ResetPasswordRequest
{
    [Required(ErrorMessage = "شماره موبایل الزامی است")]
    public string PhoneNumber { get; set; } = "";

    [Required(ErrorMessage = "کد تایید الزامی است")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "کد تایید باید 6 رقم باشد")]
    public string Code { get; set; } = "";

    [Required(ErrorMessage = "رمز عبور جدید الزامی است")]
    [DataType(DataType.Password)]
    [StringLength(100, ErrorMessage = "رمز عبور باید حداقل {2} کاراکتر باشد", MinimumLength = 8)]
    // می‌توانید Regex هم برای پیچیدگی رمز اضافه کنید
    public string NewPassword { get; set; } = "";
}

public class CreateRoleViewModel
{
    [Required(ErrorMessage = "نام Role الزامی است")]
    [Display(Name = "نام Role")]
    public string RoleName { get; set; }
}